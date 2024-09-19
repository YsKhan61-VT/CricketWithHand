using PlayFab.Internal;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace YSK.Utilities
{
    public class LogUI : SingletonMonoBehaviour<LogUI>
    {
        [SerializeField]
        TMP_Text _debugText;

        [SerializeField]
        private int _maxMessageCount;

        private Queue<string> _messages = new();

        public void AddStatusText(string text)
        {
            if (_messages.Count >= _maxMessageCount)
            {
                _messages.Dequeue();
            }
            _messages.Enqueue(text);
            string txt = "";
            foreach (string s in _messages)
            {
                txt += "\n" + s;
            }
            _debugText.text = txt;
        }
    }
}
