using System.Collections;
using System.Collections.Generic;
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

    private Rigidbody2D _rigidbody;
    private bool _boostEnabled = false;
    private float _torque;
    private Health _health;
    private bool _isAlive = true;

    private void Awake()
    {
        Id = System.Guid.NewGuid();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _boostSprite.SetActive(false);
    }

    private void Update()
    {
        if (_isAlive && _health.GetCurrentHealth() <= 0)
        {
            _isAlive = false;
            OnDeath(this);
            Destroy(gameObject, _deathDelay);
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
    }

    public void DisableBoost()
    {
        _boostEnabled = false;
        _boostSprite.SetActive(false);
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
}
