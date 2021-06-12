using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Spaceship))]
[RequireComponent(typeof(GrapplingHook))]
public class PlayerController : MonoBehaviour
{
    private const int PLAYER_SPACESHIP_LAYER = 6;

    [SerializeField] private GameObject _turretObject;
    [SerializeField] private GameObject _hookObject;

    private Spaceship _playerSpaceship;
    private GrapplingHook _grapplingHook;
    private Weapon _weapon;
    private Camera _camera;
    private List<Spaceship> _attachedSpaceships = new List<Spaceship>();

    private void Start()
    {
        _camera = Camera.main;
        _playerSpaceship = GetComponent<Spaceship>();
        _grapplingHook = GetComponent<GrapplingHook>();
        _weapon = GetComponent<Weapon>();
    }

    private void Update()
    {
        UpdateTurretRotation();
        ShootHook();
        ShootWeapon();
    }

    private void FixedUpdate()
    {
        ApplyMovementForces(_playerSpaceship);
        foreach (var spaceship in _attachedSpaceships)
        {
            ApplyMovementForces(spaceship);
        }
    }

    private void ApplyMovementForces(Spaceship spaceship)
    {
        var spaceshipIsFacingForward = SpaceshipIsFacingForward(spaceship);
        if (Input.GetKey(KeyCode.W) && spaceshipIsFacingForward)
        {
            spaceship.ActivateBoost();
        }
        else if (Input.GetKey(KeyCode.S) && !spaceshipIsFacingForward)
        {
            spaceship.ActivateBoost();
        }
        else
        {
            spaceship.DisableBoost();
        }

        spaceship.SetTorqueDirection(0);
        if (Input.GetKey(KeyCode.A))
        {
            spaceship.SetTorqueDirection(1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            spaceship.SetTorqueDirection(-1);
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
        if (Input.GetMouseButton(1))
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

        spaceship.gameObject.layer = PLAYER_SPACESHIP_LAYER;

        var joint = spaceship.AddComponent<FixedJoint2D>();
        joint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
        _attachedSpaceships.Add(spaceship);
    }

    private void ShootWeapon()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _weapon?.Fire();
            foreach (var spaceship in _attachedSpaceships)
            {
                var weapon = spaceship.GetComponent<Weapon>();
                weapon?.Fire();
            }
        }
    }
}
