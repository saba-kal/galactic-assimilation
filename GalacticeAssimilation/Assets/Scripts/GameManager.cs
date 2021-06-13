using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _timeBetweenWaves = 3f;
    [SerializeField] private Spaceship _playerSpaceship;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private List<GameObject> _enemyWaves;

    private Queue<GameObject> _enemyWaveQueue;
    private int _currentWave = 0;
    private LinkedList<Spaceship> _activeEnemyShips = new LinkedList<Spaceship>();

    private void OnEnable()
    {
        Spaceship.OnDeath += OnSpaceshipDeath;
        PlayerController.OnCapture += OnSpaceshipCapture;
    }

    private void OnDisable()
    {
        Spaceship.OnDeath -= OnSpaceshipDeath;
        PlayerController.OnCapture -= OnSpaceshipCapture;
    }

    private void Start()
    {
        _enemyWaveQueue = new Queue<GameObject>(_enemyWaves);
        ActivateNextWave();
    }

    private void OnSpaceshipDeath(Spaceship spaceship)
    {
        if (spaceship.Id == _playerSpaceship.Id)
        {
            _uiManager.GameOver();
        }
        else
        {
            RemoveSpaceshipFromActiveList(spaceship);
        }
    }

    private void OnSpaceshipCapture(Spaceship spaceship)
    {
        RemoveSpaceshipFromActiveList(spaceship);
    }

    private void ActivateNextWave()
    {
        if (_enemyWaveQueue.Count <= 0)
        {
            _uiManager.GameOver();
            return;
        }

        _currentWave++;
        _uiManager.DisplayNextWaveMessage(_currentWave, _timeBetweenWaves);
        StartCoroutine(DelayActivateNextWave());
    }

    private IEnumerator DelayActivateNextWave()
    {
        yield return new WaitForSeconds(_timeBetweenWaves);

        var wave = _enemyWaveQueue.Dequeue();
        wave.SetActive(true);
        GetEnemyShips(wave);
    }

    private void RemoveSpaceshipFromActiveList(Spaceship spaceship)
    {
        var currentNode = _activeEnemyShips.First;
        while (currentNode != null)
        {
            if (currentNode.Value.Id == spaceship.Id)
            {
                _activeEnemyShips.Remove(currentNode);
            }
            currentNode = currentNode.Next;
        }

        if (_activeEnemyShips.Count <= 0)
        {
            ActivateNextWave();
        }
    }

    private void GetEnemyShips(GameObject wave)
    {
        _activeEnemyShips = new LinkedList<Spaceship>();
        foreach (Transform child in wave.transform)
        {
            var spaceship = child.GetComponent<Spaceship>();
            if (spaceship != null)
            {
                _activeEnemyShips.AddLast(spaceship);
            }
        }
    }
}
