using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using PlayFab.Internal;
using System;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ConfirmPopupUI : SingletonMonoBehaviour<ConfirmPopupUI>
    {
        [SerializeField]
        private UIPopup _uiPopup;

        [SerializeField]
        private UIButton _leftButton;

        [SerializeField]
        private TMP_Text _leftButtonText;

        [SerializeField]
        private UIButton _rightButton;

        [SerializeField]
        private TMP_Text _rightButtonText;

        private Payload _lastSentPayload;


        private void Start()
        {
            _uiPopup.Hide();
        }
        public void ShowPopup(Payload payload)
        {
            _lastSentPayload = payload;;

            _leftButton.onClickEvent.AddListener(OnLeftButtonClicked);
            _rightButton.onClickEvent.AddListener(OnRightButtonClicked);

            _uiPopup.SetTexts(
                payload.Title, 
                payload.Message,
                payload.LeftButtonLabel,
                payload.RightButtonLabel);


            _uiPopup.Show();
        }

        void OnLeftButtonClicked()
        {
            _leftButton.onClickEvent.RemoveListener(OnLeftButtonClicked);
            _lastSentPayload.OnLeftButtonClickedCallback?.Invoke();
            _uiPopup.Hide();
            _lastSentPayload.Dispose();
        }

        void OnRightButtonClicked()
        {
            _rightButton.onClickEvent.RemoveListener(OnRightButtonClicked);
            _lastSentPayload.OnRightButtonClickedCallback?.Invoke();
            _uiPopup.Hide();
            _lastSentPayload.Dispose();
        }

        public class Payload : IDisposable
        {
            public string Title;
            public string Message;
            public string LeftButtonLabel;
            public string RightButtonLabel;
            public Action OnLeftButtonClickedCallback;
            public Action OnRightButtonClickedCallback;

            public void Dispose()
            {
                GC.Collect();
            }
        }
    }
}
