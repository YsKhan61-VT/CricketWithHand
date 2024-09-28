using Doozy.Runtime.UIManager.Containers;
using PlayFab.Internal;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class PopupUI : SingletonMonoBehaviour<PopupUI>
    {
        [SerializeField]
        UIPopup _uiPopup;

        private void Start()
        {
            _uiPopup.InstantHide();
        }

        public void ShowMessage(string title, string message)
        {
            _uiPopup.SetTexts(title, message);
            _uiPopup.Show();
        }
    }
}
