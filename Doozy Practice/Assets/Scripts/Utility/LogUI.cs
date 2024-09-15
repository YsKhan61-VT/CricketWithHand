using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace YSK.Utilities
{
    public class LogUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _debugText;

        public static LogUI Instance;

        private Queue<string> _messages = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
            }
        }

        public void AddStatusText(string text)
        {
            if (_messages.Count == 5)
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
