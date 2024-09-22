using System;
using UnityEngine;


namespace CricketWithHand.Utility
{
    public abstract class GenericDataContainerSO<T> : ScriptableObject where T : struct
    {
        public Action OnValueUpdated;

        public T Value { get; private set; }
        public virtual void UpdateData(T value)
        {
            Value = value;
            OnValueUpdated?.Invoke();
        }
    }
}

