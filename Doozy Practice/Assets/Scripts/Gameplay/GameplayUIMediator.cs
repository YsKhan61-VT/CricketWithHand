using Doozy.Runtime.UIManager.Components;
using DoozyPractice.UI;
using UnityEngine;


namespace DoozyPractice.Gameplay
{
    public class GameplayUIMediator : MonoBehaviour
    {
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

        public void UpdateOwnerTotalScoreUI(int score, int wicket) =>
            _ownerClientUI.UpdateTotalScoreAndWicket(score, wicket);

        public void UpdateOtherTotalScoreUI(int score, int wicket) =>
            _otherClientUI.UpdateTotalScoreAndWicket(score, wicket);

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
    }
}
