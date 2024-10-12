using UnityEngine;

namespace CricketWithHand.Utility
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfigSO")]
    public class GameConfigSO : ScriptableObject
    {
        [SerializeField]
        private int _ballsPerOver;
        public int BallsPerOver => _ballsPerOver;

        [SerializeField]
        private int _turnDurationInSecs;
        public int TurnDurationInSecs => _turnDurationInSecs;

        [SerializeField]
        private int _waitForSecsBeforeNextTurn;
        public int WaitForSecsBeforeNextTurn => _waitForSecsBeforeNextTurn;

        [SerializeField, Range(0f, 1f)]
        private float _ownerWinningChance;
        public float OwnerWinningChance => _ownerWinningChance;

        [SerializeField, Range(0f, 1f)]
        private float _aiBattingChance;
        public float AIBattingChance => _aiBattingChance;

        [SerializeField]
        private string _ownerWinTossMessage = "You won the toss!";
        public string OwnerWinTossMessage => _ownerWinTossMessage;

        [SerializeField]
        private string _ownerLostTossMessage = "You lost the toss!";
        public string OwnerLostTossMessage => _ownerLostTossMessage;

        [SerializeField]
        private string _ownerWillBatMessage = "You will be batting.";
        public string OwnerWillBatMessage => _ownerWillBatMessage;

        [SerializeField]
        private string _ownerWillBallMessage = "You will be balling.";
        public string OwnerWillBallMessage => _ownerWillBallMessage;

        [SerializeField]
        private string _winMessage;
        public string WinMessage => _winMessage;

        [SerializeField]
        private string _lossMessage;
        public string LossMessage => _lossMessage;

        [SerializeField]
        private string _drawMessage;
        public string DrawMessage => _drawMessage;

    }
}

