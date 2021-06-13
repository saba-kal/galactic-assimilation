using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifetime = 5f;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private List<Transform> _projectileSpawnPoints;

    private float _timeSinceLastShot = 0f;

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    public void Fire()
    {
        if (_timeSinceLastShot < _timeBetweenShots)
        {
            return;
        }

        foreach (var spawnPoint in _projectileSpawnPoints)
        {
            var projectile = Instantiate(_projectilePrefab);
            projectile.transform.position = spawnPoint.position;
            projectile.transform.rotation = transform.rotation;
            projectile.layer = gameObject.layer;

            var projectileRigidBody = projectile.GetComponent<Rigidbody2D>();
            projectileRigidBody.velocity = transform.up * _projectileSpeed;

            Destroy(projectile, _projectileLifetime);
        }

        _timeSinceLastShot = 0f;
    }
}
