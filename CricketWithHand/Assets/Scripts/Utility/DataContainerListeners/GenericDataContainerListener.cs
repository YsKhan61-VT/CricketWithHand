using UnityEngine;
using UnityEngine.Events;

namespace CricketWithHand.Utility
{
    public abstract class GenericDataContainerListener<T> : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<T> _onContainerValueUpdated;

        protected GenericDataContainerSO<T> container;

        private void OnEnable()
        {
            container.OnValueUpdated += OnContainerValueUpdated;
        }

        private void OnDisable()
        {
            container.OnValueUpdated -= OnContainerValueUpdated;
        }

        private void OnContainerValueUpdated() =>
            _onContainerValueUpdated?.Invoke(container.Value);
    }
}

