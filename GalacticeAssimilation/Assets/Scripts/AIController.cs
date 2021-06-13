using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private float _arriveDistance = 5f;
    [SerializeField] private float _collisionAvoidanceDistance = 20f;
    [SerializeField] private float _shootAngleThreshold = 5f;
    [SerializeField] private LayerMask _collisionAvoidanceLayerMask;

    private Spaceship _targetSpaceship;
    private Spaceship _currentSpaceship;
    private Weapon _weapon;

    private void Start()
    {
        _targetSpaceship = PlayerController.Instance.GetPlayerSpaceship();
        _currentSpaceship = GetComponent<Spaceship>();
        _weapon = GetComponent<Weapon>();
    }

    private void Update()
    {
        _targetSpaceship = PlayerController.Instance.GetPlayerSpaceship();
        if (_targetSpaceship == null)
        {
            return;
        }

        SeekTarget();
    }

    private void SeekTarget()
    {
        var desiredDirection = (Vector2)(_targetSpaceship.transform.position - _currentSpaceship.transform.position);
        AvoidCollision(ref desiredDirection);
        MoveTowardsDirection(desiredDirection);
        ShootTarget();
    }

    private void MoveTowardsDirection(Vector2 desiredDirection)
    {
        var currentDirection = _currentSpaceship.transform.up;
        var angle = Vector2.SignedAngle(currentDirection, desiredDirection);
        _currentSpaceship.SetTorque(angle * Mathf.Abs(angle) * 0.001f);

        if (Vector2.Distance(_targetSpaceship.transform.position, _currentSpaceship.transform.position) > _arriveDistance &&
            Mathf.Abs(angle) < 30f)
        {
            _currentSpaceship.ActivateBoost();
        }
        else
        {
            _currentSpaceship.DisableBoost();
        }
    }

    private void AvoidCollision(ref Vector2 desiredDirection)
    {
        var originalDesiredDirection = desiredDirection;
        var hit = RayCast(desiredDirection);
        var rotationIncrement = 10f;
        var rotationAmount = 0f;
        var index = 0;
        var maxIterations = 100;

        while (hit.collider != null)
        {
            var sign = 1;
            if (index % 2 == 0)
            {
                rotationAmount += rotationIncrement;
            }
            else
            {
                sign = -1;
            }

            desiredDirection = RotateVector(originalDesiredDirection, rotationAmount * sign);

            hit = RayCast(desiredDirection);
            index++;

            if (index >= maxIterations)
            {
                Debug.LogError("Reached max iterations for collision avoidance.");
                return;
            }
        }
    }

    private RaycastHit2D RayCast(Vector2 direction)
    {
        return Physics2D.Raycast(transform.position, direction, _collisionAvoidanceDistance, ~_collisionAvoidanceLayerMask);
    }

    /// <see cref="https://sushanta1991.blogspot.com/2016/08/how-to-rotate-2d-vector-in-unity.html"/>
    private Vector2 RotateVector(Vector2 v, float angle)
    {
        var radian = angle * Mathf.Deg2Rad;
        var x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        var y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(x, y);
    }

    private void ShootTarget()
    {
        var currentDirection = _currentSpaceship.transform.up;
        var targetDirection = (Vector2)(_targetSpaceship.transform.position - _currentSpaceship.transform.position);
        var angle = Vector2.Angle(currentDirection, targetDirection);

        if (angle < _shootAngleThreshold)
        {
            _weapon.Fire();
        }
    }
}
