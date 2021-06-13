using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Spaceship _playerSpaceship;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private List<GameObject> _enemyWaves;

    private void OnEnable()
    {
        Spaceship.OnDeath += OnSpaceshipDeath;
    }

    private void OnDisable()
    {
        Spaceship.OnDeath -= OnSpaceshipDeath;
    }

    private void Start()
    {
        _gameOverScreen.SetActive(false);
    }

    private void OnSpaceshipDeath(Spaceship spaceship)
    {
        if (spaceship.Id == _playerSpaceship.Id)
        {
            _gameOverScreen.SetActive(true);
        }
    }
}
