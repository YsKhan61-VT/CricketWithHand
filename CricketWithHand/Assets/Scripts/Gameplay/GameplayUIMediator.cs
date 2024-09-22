using Doozy.Runtime.UIManager.Components;
using CricketWithHand.UI;
using UnityEngine;
using CricketWithHand.Utility;
using System;
using UnityEngine.SocialPlatforms.Impl;


namespace CricketWithHand.Gameplay
{
    public class GameplayUIMediator : MonoBehaviour
    {
        [SerializeField]
        IntDataContainerSO _totalOversDataContainer;

        /// <summary>
        /// Called everytime the owner total score gets updated.
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerTotalScoreContainer;

        /// <summary>
        /// Called everytime the other player's total score gets updated.
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherTotalScoreContainer;

        [SerializeField]
        UISlider _turnDurationSlider;

        [SerializeField]
        ClientUI _ownerClientUI;

        [SerializeField]
        ClientUI _otherClientUI;

        [SerializeField]
        OwnerClientInputUI _ownerClientInputUI;

        [SerializeField]
        OverScoreUI _overScoreUI;

        [SerializeField]
        ResultUI _resultUI;

        private void OnEnable()
        {
            _totalOversDataContainer.OnValueUpdated += OnTotalOversUpdated;
            _ownerTotalScoreContainer.OnValueUpdated += OnOwnerTotalScoreUpdated;
            _otherTotalScoreContainer.OnValueUpdated += OnOtherTotalScoreUpdated;
        }

        private void OnDisable()
        {
            _totalOversDataContainer.OnValueUpdated -= OnTotalOversUpdated;
            _ownerTotalScoreContainer.OnValueUpdated -= OnOwnerTotalScoreUpdated;
            _otherTotalScoreContainer.OnValueUpdated -= OnOtherTotalScoreUpdated;
        }

        public void UpdateTurnDurationSlider(float value)
        {
            if (_turnDurationSlider == null) return;

            _turnDurationSlider.value = value;
        }

        public void ResetOverScoreUI() =>
            _overScoreUI.ResetBallUIs();

        public void UpdateOverScoreUI(int currentBall, int score, bool isOut) =>
            _overScoreUI.UpdateScoreOnBallUI(currentBall, score, isOut);

        public void ToggleUserInputInteraction(bool enable) =>
            _ownerClientInputUI.ToggleActiveInputButtons(enable);

        public void UpdateUIToOwnerIsBatting() =>
            _ownerClientUI.UpdatePlayingStateText(true);

        public void UpdateUIToOwnerIsBalling() =>
            _ownerClientUI.UpdatePlayingStateText(false);

        public void UpdateUIToOtherIsBatting() =>
            _otherClientUI.UpdatePlayingStateText(true);

        public void UpdateUIToOtherIsBalling() =>
            _otherClientUI.UpdatePlayingStateText(false);

        public void UpdateOwnerInputScoreUI(int score) =>
            _ownerClientUI.ShowInputScore(score);

        public void UpdateOtherInputScoreUI(int score) =>
            _otherClientUI.ShowInputScore(score);

        public void UpdateOwnerTotalScoreUI(int score) =>
            _ownerClientUI.UpdateTotalScore(score);

        public void UpdateOtherTotalScoreUI(int score) =>
            _otherClientUI.UpdateTotalScore(score);

        public void UpdateOwnerTotalWicketsUI(int wicketLost, int totalWickets) =>
            _ownerClientUI.UpdateTotalWickets(wicketLost, totalWickets);

        public void UpdateOtherTotalWicketsUI(int wicketLost, int totalWickets) =>
            _otherClientUI.UpdateTotalWickets(wicketLost, totalWickets);

        public void UpdateOwnerTotalOversUI(int over, int balls, int totalOvers) =>
            _ownerClientUI.UpdateTotalOversText(over, balls, totalOvers);

        public void UpdateOtherTotalOversUI(int over, int balls, int totalOvers) =>
            _otherClientUI.UpdateTotalOversText(over, balls, totalOvers);

        public void ShowWinText() =>
            _resultUI.ShowWinText();

        public void ShowLossText() =>
            _resultUI.ShowLossText();

        public void ShowDrawText() =>
            _resultUI.ShowDrawText();

        private void OnOwnerTotalScoreUpdated()
        {
            _ownerClientUI.UpdateTotalScore(_ownerTotalScoreContainer.Value);
        }

        private void OnOtherTotalScoreUpdated()
        {
            _otherClientUI.UpdateTotalScore(_ownerTotalScoreContainer.Value);
        }

        private void OnTotalOversUpdated()
        {
            _ownerClientUI.UpdateTotalOversText(0, 0, _totalOversDataContainer.Value);
            _otherClientUI.UpdateTotalOversText(0, 0, _totalOversDataContainer.Value);
        }
    }
}
