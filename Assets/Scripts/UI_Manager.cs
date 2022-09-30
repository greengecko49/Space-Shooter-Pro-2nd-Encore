using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [Header("TextMeshProUGUI field for Score")]
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _ammoText;
    [SerializeField]
    private TextMeshProUGUI _waveText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private TextMeshProUGUI _gameOverText;
    [SerializeField]
    private TextMeshProUGUI _restartText;
    [SerializeField]
    private Slider _thrusterSlider;

    private GameManager _gameManager;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateWave(int waveCount)
    {
        _waveText.text = "Wave " + waveCount.ToString();
        StartCoroutine(WaveTextRoutine());
    }

    public void UpdateAmmo(int playerAmmo)
    {
        _ammoText.text = "Ammo " + playerAmmo + "/30";
    }

    public void UpdateThrusterFuel(float value)
    {
        _thrusterSlider.value = value;
    }

    public void UpdateLives(int currentlives)
    {
        if (currentlives >= 0)
        {

            _LivesImg.sprite = _liveSprites[currentlives];

            if (currentlives == 0)
            {
                GameOverSequence();
            }
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());

    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator WaveTextRoutine()
    {
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _waveText.gameObject.SetActive(false);   
    }

    
}
