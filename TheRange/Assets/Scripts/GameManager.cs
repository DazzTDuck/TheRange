using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameTimeInSeconds;
    [Header("Texts")]
    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private TMP_Text _highscoreText;
    [SerializeField] private TMP_Text _timerText;

    public static GameManager Instance;

    private const string HIGHSCORE_STRING = "Highscore";
    private int _points;
    private float _timer;
    private bool _gameStarted = false;  

    private void Start()
    {
        //If there is an instance, and it's not this, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _highscoreText.text = PlayerPrefs.GetInt(HIGHSCORE_STRING, 0).ToString();
        _timerText.text = UpdateTimerText(gameTimeInSeconds);
    }

    private void Update()
    {
        if (_gameStarted)
        {
            _timer -= Time.deltaTime;
            _timerText.text = UpdateTimerText(_timer);

            if(_timer <= 0)
            {
                //end game and save score
                _gameStarted = false;
                _timer = 0;
                _timerText.text = UpdateTimerText(gameTimeInSeconds);

                if (_points > PlayerPrefs.GetInt(HIGHSCORE_STRING, 0))
                {
                    //save highscore
                    PlayerPrefs.SetInt(HIGHSCORE_STRING, _points); 
                    _highscoreText.text = _points.ToString();
                }
                    
            }

        }
    }

    private string UpdateTimerText(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds % 3600 / 60);
        seconds %= 60;
        return string.Format("{0:00}:{1:00}", mins, seconds);
    }

    public void StartGame()
    {
        if (_gameStarted)
            return;

        _gameStarted = true;
        _timer = gameTimeInSeconds;
        _points = 0;
    }

    public void AddPoints(int toAdd)
    {
        if (!_gameStarted)
            return;

        _points += toAdd;
        _pointsText.text = _points.ToString();
    }

}
