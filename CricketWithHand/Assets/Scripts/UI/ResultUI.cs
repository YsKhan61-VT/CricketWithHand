using CricketWithHand.Gameplay;
using CricketWithHand.Utility;
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
            _gameData.Winner.OnValueUpdated += OnGameResultUpdated;
        }

        private void OnDisable()
        {
            _gameData.Winner.OnValueUpdated -= OnGameResultUpdated;
        }

        public void OnGameResultUpdated()
        {
            switch (_gameData.Winner.Value)
            {
                case PlayerType.NONE:
                    _resultMessageText.text = _gameData.GameConfig.DrawMessage;
                    break;

                case PlayerType.OTHER:
                    _resultMessageText.text = _gameData.GameConfig.LossMessage;
                    break;

                case PlayerType.OWNER:
                    _resultMessageText.text = _gameData.GameConfig.WinMessage;
                    break;
            }
        }
    }
}
