using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Spaceship))]
[RequireComponent(typeof(GrapplingHook))]
public class PlayerController : MonoBehaviour
{
    public delegate void Capture(Spaceship spaceship);
    public static Capture OnCapture;

    public static PlayerController Instance { get; private set; }

    [SerializeField] private float _additionalSpacecraftTorqueMultiplier = 2f;
    [SerializeField] private GameObject _turretObject;
    [SerializeField] private GameObject _hookObject;

    private Spaceship _playerSpaceship;
    private GrapplingHook _grapplingHook;
    private Weapon _weapon;
    private Camera _camera;
    private List<Spaceship> _attachedSpaceships = new List<Spaceship>();
    private SoundManager _soundManager;

    private void OnEnable()
    {
        Spaceship.OnDeath += OnSpaceshipDeath;
    }

    private void OnDisable()
    {
        Spaceship.OnDeath += OnSpaceshipDeath;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _camera = Camera.main;
        _playerSpaceship = GetComponent<Spaceship>();
        _grapplingHook = GetComponent<GrapplingHook>();
        _weapon = GetComponent<Weapon>();
        _soundManager = SoundManager.GetInstance();
    }

    private void Update()
    {
        UpdateTurretRotation();
        ShootHook();
        ShootWeapon();
        CleanupDestroyedSpaceships();
    }

    private void FixedUpdate()
    {
        var boostIsActive = ApplyMovementForces(_playerSpaceship);
        foreach (var spaceship in _attachedSpaceships)
        {
            if (spaceship != null)
            {
                boostIsActive = ApplyMovementForces(spaceship) || boostIsActive;
            }
        }

        ApplyTorqueForces();

        if (boostIsActive)
        {
            _soundManager.Play(Constants.ENGINE_SOUND, true);
        }
        else
        {
            _soundManager.Stop(Constants.ENGINE_SOUND);
        }
    }

    private bool ApplyMovementForces(Spaceship spaceship)
    {
        var boostIsActive = false;
        var spaceshipIsFacingForward = SpaceshipIsFacingForward(spaceship);
        if (Input.GetKey(KeyCode.W) && spaceshipIsFacingForward)
        {
            boostIsActive = true;
            spaceship.ActivateBoost();
        }
        else if (Input.GetKey(KeyCode.S) && !spaceshipIsFacingForward)
        {
            boostIsActive = true;
            spaceship.ActivateBoost();
        }
        else
        {
            spaceship.DisableBoost();
        }

        return boostIsActive;
    }

    private void ApplyTorqueForces()
    {
        var totalTorque = _playerSpaceship.GetMaxTorque();
        foreach (var spaceship in _attachedSpaceships)
        {
            if (spaceship != null)
            {
                totalTorque += spaceship.GetMaxTorque() * _additionalSpacecraftTorqueMultiplier;
            }
        }

        _playerSpaceship.SetTorque(0);
        if (Input.GetKey(KeyCode.A))
        {
            _playerSpaceship.SetTorqueUnclamped(totalTorque);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _playerSpaceship.SetTorqueUnclamped(-totalTorque);
        }
    }

    private bool SpaceshipIsFacingForward(Spaceship spaceship)
    {
        return Mathf.Abs(Mathf.DeltaAngle(_playerSpaceship.transform.eulerAngles.z, spaceship.transform.eulerAngles.z)) < 90;
    }

    private void UpdateTurretRotation()
    {
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y);

        _turretObject.transform.up = direction;
    }

    private void ShootHook()
    {
        if (Input.GetMouseButton(0))
        {
            _grapplingHook.ShootHook((targetSpaceship) =>
            {
                AttachSpaceship(targetSpaceship);
            });
        }
    }

    private void AttachSpaceship(Spaceship spaceship)
    {
        if (spaceship == null)
        {
            return;
        }

        spaceship.gameObject.layer = Constants.PLAYER_SPACESHIP_LAYER;

        var joint = spaceship.AddComponent<FixedJoint2D>();
        joint.connectedBody = gameObject.GetComponent<Rigidbody2D>();

        var aiController = spaceship.GetComponent<AIController>();
        if (aiController != null)
        {
            aiController.enabled = false;
        }

        _attachedSpaceships.Add(spaceship);
        OnCapture?.Invoke(spaceship);
    }

    private void ShootWeapon()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _weapon?.Fire();
            foreach (var spaceship in _attachedSpaceships)
            {
                if (spaceship != null)
                {
                    var weapon = spaceship.GetComponent<Weapon>();
                    weapon?.Fire();
                }
            }
        }
    }

    public Spaceship GetPlayerSpaceship()
    {
        return _playerSpaceship;
    }

    private void OnSpaceshipDeath(Spaceship spaceship)
    {
        var newAttachedSpaceships = new List<Spaceship>();
        foreach (var attachedSpaceship in _attachedSpaceships)
        {
            if (attachedSpaceship.Id != spaceship.Id)
            {
                newAttachedSpaceships.Add(attachedSpaceship);
            }
        }

        _attachedSpaceships = newAttachedSpaceships;
    }

    public int GetCapturedSpaceshipCount()
    {
        return _attachedSpaceships.Count;
    }

    private void CleanupDestroyedSpaceships()
    {
        var newAttachedSpaceships = new List<Spaceship>();
        var deadSpaceshipsExist = false;
        foreach (var spaceship in _attachedSpaceships)
        {
            if (spaceship != null)
            {
                newAttachedSpaceships.Add(spaceship);
            }
            else
            {
                deadSpaceshipsExist = true;
            }
        }

        if (deadSpaceshipsExist)
        {
            _attachedSpaceships = newAttachedSpaceships;
        }
    }
}
