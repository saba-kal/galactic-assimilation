using UnityEngine;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nextWaveText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _gameOverScreen;

    private void Start()
    {
        _nextWaveText.gameObject.SetActive(false);
        _scoreText.text = "Score: 0";
        _gameOverScreen.SetActive(false);
    }

    public void GameOver()
    {
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
}
