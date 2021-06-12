using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour
{
    [SerializeField] private float _boostPower = 10f;
    [SerializeField] private float _torque = 10f;
    [SerializeField] private GameObject _boostSprite;

    private Rigidbody2D _rigidbody;
    private bool _boostEnabled = false;
    private int _torqueDirection = 0;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _boostSprite.SetActive(false);
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

    public void SetTorqueDirection(int torqueDirection)
    {
        _torqueDirection = torqueDirection;
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

        _rigidbody.AddTorque(_torqueDirection * _torque);
    }
}
