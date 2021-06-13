using UnityEngine;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nextWaveText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private TextMeshProUGUI _gameOverTitle;
    [SerializeField] private TextMeshProUGUI _gameOverScoreBreakdownText;
    [SerializeField] private TextMeshProUGUI _gameOverScoreText;

    private void Start()
    {
        _nextWaveText.gameObject.SetActive(false);
        _scoreText.text = "Score: 0";
        _gameOverScreen.SetActive(false);
    }

    public void GameOver(bool isWin, int runningScore, int spaceshipMultiplier, int spaceshipCount)
    {
        if (isWin)
        {
            _gameOverTitle.text = "You Win!";
        }
        else
        {
            _gameOverTitle.text = "Game Over!";
            spaceshipCount = 0;
        }

        _gameOverScoreBreakdownText.text = $"Score: {runningScore}\n" +
            $"Captured Spacecraft: {spaceshipCount}\n" +
            $"Final Score: {spaceshipCount} * {spaceshipMultiplier} + {runningScore} =";
        _gameOverScoreText.text = $"{spaceshipCount * spaceshipMultiplier + runningScore}";

        _gameOverScreen.SetActive(true);
    }

    public void DisplayNextWaveMessage(int waveNumber, float showTime)
    {
        _nextWaveText.text = $"Wave {waveNumber} Incoming";
        _nextWaveText.gameObject.SetActive(true);
        StartCoroutine(HideNextWaveMessage(showTime));
    }

    private IEnumerator HideNextWaveMessage(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _nextWaveText.gameObject.SetActive(false);
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = $"Score: {score}";
    }
}
