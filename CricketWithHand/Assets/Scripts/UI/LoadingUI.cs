using Doozy.Runtime.UIManager.Containers;
using PlayFab.Internal;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class LoadingUI : SingletonMonoBehaviour<LoadingUI>
    {
        [SerializeField]
        private UIView _uiView;

        public void Show()
        {
            _uiView.Show();
        }
        public void Hide()
        {
            _uiView.Hide();
        }
    }
}
