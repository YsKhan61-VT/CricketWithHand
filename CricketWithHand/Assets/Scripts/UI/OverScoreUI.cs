using CricketWithHand.Gameplay;
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
        private PlayerStatsSO _ownerStats;

        [SerializeField]
        private PlayerStatsSO _otherStats;

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
            _ownerStats.OversCount.OnValueUpdated += ResetBallUIs;
            _otherStats.OversCount.OnValueUpdated += ResetBallUIs;

            _ownerStats.OnBallPlayed += UpdateUI;
            _otherStats.OnBallPlayed += UpdateUI;
        }

        private void OnDisable()
        {
            _ownerStats.OversCount.OnValueUpdated -= ResetBallUIs;
            _otherStats.OversCount.OnValueUpdated -= ResetBallUIs;

            _ownerStats.OnBallPlayed -= UpdateUI;
            _otherStats.OnBallPlayed -= UpdateUI;
        }

        /// <summary>
        /// This should be called at the start of every over
        /// </summary>
        private void ResetBallUIs()
        {
            foreach (var ball in _ballScoreUIs)
            {
                ball.BackgroundImage.color = _initialBallColor;
                ball.ScoreText.text = "";
            }
        }

        private void UpdateUI()
        {
            PlayerStatsSO playerStats = _gameData.IsOwnerBatting ? _ownerStats : _otherStats;

            int index = Mathf.Max(0, playerStats.BallsCount.Value - 1);
            int scoreInThisBall = playerStats.ScoreInThisBall.Value;

            if (scoreInThisBall == 0 && playerStats.IsOutInThisBall)
            {
                _ballScoreUIs[index].BackgroundImage.color = _wicketColor;
                _ballScoreUIs[index].ScoreText.text = "W";
            }
            else
            {
                _ballScoreUIs[index].BackgroundImage.color = _currentBallColor;
                _ballScoreUIs[index].ScoreText.text = scoreInThisBall.ToString();
            }
        }
    }
}
