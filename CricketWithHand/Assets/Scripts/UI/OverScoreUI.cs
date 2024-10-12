using CricketWithHand.Gameplay;
using CricketWithHand.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CricketWithHand.UI
{
    public class OverScoreUI : MonoBehaviour
    {
        [Serializable]
        class BallScoreUI
        {
            public Image BackgroundImage;
            public TMP_Text ScoreText;
        }

        [SerializeField]
        private GameDataSO _gameData;

        [SerializeField]
        BallScoreUI[] _ballScoreUIs;

        [SerializeField]
        Color _currentBallColor;

        [SerializeField]
        Color _initialBallColor;

        [SerializeField]
        Color _wicketColor;

        private void OnEnable()
        {
            _gameData.OwnerBallDataContainer.OnValueUpdated += UpdateUI;
            _gameData.OtherBallDataContainer.OnValueUpdated += UpdateUI;
        }

        private void OnDisable()
        {
            _gameData.OwnerBallDataContainer.OnValueUpdated -= UpdateUI;
            _gameData.OtherBallDataContainer.OnValueUpdated -= UpdateUI;
        }

        public void ResetBallUIs()
        {
            foreach (var ball in _ballScoreUIs)
            {
                ball.BackgroundImage.color = _initialBallColor;
                ball.ScoreText.text = "";
            }
        }

        private void UpdateUI()
        {
            BallData ballData;

            if (_gameData.IsOwnerBatting)
                ballData = _gameData.OwnerBallDataContainer.Value;
            else
                ballData = _gameData.OtherBallDataContainer.Value;

            int index = ballData.BallNumber - 1;
            if (index < 0 || index >= _ballScoreUIs.Length)
            {
                // index can be -1 when the first ball of the game is bowled.
                return;
            }

            if (ballData.Score == 0 && ballData.IsWicketLost)
            {
                _ballScoreUIs[index].BackgroundImage.color = _wicketColor;
                _ballScoreUIs[index].ScoreText.text = "W";
            }
            else
            {
                _ballScoreUIs[index].BackgroundImage.color = _currentBallColor;
                _ballScoreUIs[index].ScoreText.text = ballData.Score.ToString();
            }
        }
    }
}
