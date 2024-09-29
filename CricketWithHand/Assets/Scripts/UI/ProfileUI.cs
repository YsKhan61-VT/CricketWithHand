using Doozy.Runtime.UIManager.Components;
using System;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ProfileUI : MonoBehaviour
    {
        [SerializeField]
        Register_LoginUIMediator _registerLoginUIMediator;

        [SerializeField]
        TMP_Text _displayName;


        private void Start()
        {
            ShowDisplayName("");
        }

        public void ShowDisplayName(string displayName) =>
            _displayName.text = displayName;
    }
}
