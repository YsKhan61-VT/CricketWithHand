using System;
using UnityEngine;


namespace CricketWithHand.Utility
{
    public abstract class GenericDataContainerSO<T> : ScriptableObject
    {
        [SerializeField, TextArea(2, 2)]
        private string _logArea;

        public Action OnValueUpdated;

        public T Value { get; private set; }
        public virtual void UpdateData(T value)
        {
            Value = value;
            OnValueUpdated?.Invoke();

            Log();
        }

        protected virtual void Log()
        {
            _logArea = Value.ToString();
        }
    }
}

