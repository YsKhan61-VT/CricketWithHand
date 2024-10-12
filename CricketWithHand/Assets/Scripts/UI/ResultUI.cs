using CricketWithHand.Gameplay;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField]
        private GameDataSO _gameData;

        [SerializeField]
        TMP_Text _resultMessageText;

        private void OnEnable()
        {
            _gameData.GameResult.OnValueUpdated += OnGameResultUpdated;
        }

        private void OnDisable()
        {
            _gameData.GameResult.OnValueUpdated -= OnGameResultUpdated;
        }

        private void OnGameResultUpdated()
        {
            switch (_gameData.GameResult.Value)
            {
                case GameResultEnum.DRAW:
                    _resultMessageText.text = _gameData.GameConfig.DrawMessage;
                    break;

                case GameResultEnum.LOST:
                    _resultMessageText.text = _gameData.GameConfig.LossMessage;
                    break;

                case GameResultEnum.WON:
                    _resultMessageText.text = _gameData.GameConfig.WinMessage;
                    break;
            }
        }
    }
}
