using CricketWithHand.Gameplay;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField]
        private GameConfigSO _gameConfig;

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
                    _resultMessageText.text = _gameConfig.DrawMessage;
                    break;

                case PlayerType.OTHER:
                    _resultMessageText.text = _gameConfig.LossMessage;
                    break;

                case PlayerType.OWNER:
                    _resultMessageText.text = _gameConfig.WinMessage;
                    break;
            }
        }
    }
}
