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
        InputPanelButtonUI _ownerInputButtonUI;

        [SerializeField]
        InputPanelButtonUI _otherInputButtonUI;

        [SerializeField]
        OverScoreUI _overScoreUI;

        [SerializeField]
        ResultUI _resultUI;

        public void UpdateTurnDurationSlider(float value) =>
            _turnDurationSlider.value = value;

        public void ResetOverScoreUI() =>
            _overScoreUI.ResetBallUIs();

        public void UpdateOverScoreUI(int currentBall, int score) =>
            _overScoreUI.UpdateScoreOnBallUI(currentBall, score);

        public void ToggleUserInputInteraction(bool enable) =>
            _ownerInputButtonUI.ToggleActiveInputButtons(enable);

        public void UpdateUIToOwnerIsBatting() =>
            _ownerClientUI.UpdatePlayingStateText(true);

        public void UpdateUIToOwnerIsBalling() =>
            _ownerClientUI.UpdatePlayingStateText(false);

        public void UpdateUIToOtherIsBatting() =>
            _ownerClientUI.UpdatePlayingStateText(true);

        public void UpdateUIToOtherIsBalling() =>
            _ownerClientUI.UpdatePlayingStateText(false);

        public void UpdateOwnerTurnScoreUI(int score) =>
            _ownerClientUI. ShowInputScore(score);

        public void UpdateOtherTurnScoreUI(int score) =>
            _otherClientUI.ShowInputScore(score);

        public void UpdateOwnerTotalScoreUI(int score) =>
            _ownerClientUI.UpdateTotalScore(score);

        public void UpdateOtherTotalScoreUI(int score) =>
            _otherClientUI.UpdateTotalScore(score);

        public void ShowWinText() =>
            _resultUI.ShowWinText();

        public void ShowLossText() =>
            _resultUI.ShowLossText();

        public void ShowDrawText() =>
            _resultUI.ShowDrawText();
    }
}
