using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour
{
    public delegate void Death(Spaceship spaceship);
    public static Death OnDeath;

    public System.Guid Id { get; private set; }

    [SerializeField] private float _boostPower = 10f;
    [SerializeField] private float _maxTorque = 1f;
    [SerializeField] private float _deathDelay = 1f;
    [SerializeField] private GameObject _boostSprite;
    [SerializeField] private GameObject _boostParticleSystems;
    [SerializeField] private ParticleSystem _fireParticleSystem;
    [SerializeField] private GameObject _explosionParticleSystemPrefab;

    private Rigidbody2D _rigidbody;
    private bool _boostEnabled = false;
    private float _torque;
    private Health _health;
    private bool _isAlive = true;
    private SoundManager _soundManager;

    private void Awake()
    {
        Id = System.Guid.NewGuid();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _soundManager = SoundManager.GetInstance();
        _fireParticleSystem.gameObject.SetActive(false);
        DisableBoost();
    }

    private void Update()
    {
        if (_isAlive && _health.GetCurrentHealth() <= 0)
        {
            DestroySpaceship();
        }
    }

    private void FixedUpdate()
    {
        ApplyMovementForces();
    }

    public void ActivateBoost()
    {
        _boostEnabled = true;
        _boostSprite.SetActive(true);
        SetParticlesActive(true);
    }

    public void DisableBoost()
    {
        _boostEnabled = false;
        _boostSprite.SetActive(false);
        SetParticlesActive(false);
    }

    public void SetTorque(float torque)
    {
        _torque = Mathf.Clamp(torque, -_maxTorque, _maxTorque);
    }

    public void SetTorqueUnclamped(float torque)
    {
        _torque = torque;
    }

    private void ApplyMovementForces()
    {
        if (_boostEnabled)
        {
            _rigidbody.AddForce(transform.up * _boostPower);
            _boostSprite.SetActive(true);
        }
        else
        {
            _boostSprite.SetActive(false);
        }

        _rigidbody.AddTorque(_torque);
    }

    public float GetMaxTorque()
    {
        return _maxTorque;
    }

    public int GetCurrentHealth()
    {
        return _health.GetCurrentHealth();
    }

    private void RemoveHook()
    {
        foreach (Transform child in transform)
        {
            var hook = child.GetComponent<Hook>();
            if (hook != null)
            {
                hook.transform.parent = null;
            }
        }
    }

    private void SetParticlesActive(bool isActive)
    {
        foreach (Transform child in _boostParticleSystems.transform)
        {
            var particleSystem = child.GetComponent<ParticleSystem>();
            if (!isActive)
            {
                particleSystem?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            else
            {
                particleSystem?.Play();
            }
        }
    }

    private void DestroySpaceship()
    {
        _isAlive = false;
        RemoveHook();
        OnDeath(this);
        StartCoroutine(PlayDeathSounds());
        StartCoroutine(CreateDeathEffects());
        Destroy(gameObject, _deathDelay);
    }

    private IEnumerator PlayDeathSounds()
    {
        _soundManager.Play(Constants.FIRE_SOUND);

        yield return new WaitForSeconds(_deathDelay - 0.1f);

        _soundManager.Stop(Constants.FIRE_SOUND);
        _soundManager.Play(Constants.EXPLOSION_SOUND);
    }

    private IEnumerator CreateDeathEffects()
    {
        _fireParticleSystem.gameObject.SetActive(true);

        yield return new WaitForSeconds(_deathDelay - 0.1f);

        var explosion = Instantiate(_explosionParticleSystemPrefab);
        explosion.transform.position = transform.position;
        Destroy(explosion, 5f);
    }
}
