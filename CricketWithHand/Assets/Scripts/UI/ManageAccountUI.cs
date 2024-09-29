using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ManageAccountUI : MonoBehaviour
    {
        [SerializeField]
        Register_LoginUIMediator _registerLoginUIMediator;

        [SerializeField]
        UIButton _linkWithGoogleButton;

        [SerializeField]
        TMP_Text _displayName;

        private void OnEnable()
        {
            _linkWithGoogleButton.onClickEvent.AddListener(OnLinkWithGoogleButtonClicked);
        }

        private void Start()
        {
            ShowDisplayName("");
        }

        private void OnDisable()
        {
            _linkWithGoogleButton.onClickEvent.RemoveListener(OnLinkWithGoogleButtonClicked);
        }

        public void LogOut()
        {
            _registerLoginUIMediator.LogOut();
        }

        public void ShowDisplayName(string displayName) =>
            _displayName.text = displayName;

        public void ConfigureLinkWithGoogleButton()
        {
            if (_registerLoginUIMediator == null) return;

            if (!_registerLoginUIMediator.IsLoggedIn) return;

            if (!_registerLoginUIMediator.IsLinkedInWithGoogle)
                _linkWithGoogleButton.gameObject.SetActive(true);
            else
                _linkWithGoogleButton.gameObject.SetActive(false);
        }

        private void OnLinkWithGoogleButtonClicked()
        {
            _registerLoginUIMediator.LinkAccountWithGoogle();
        }
    }
}
