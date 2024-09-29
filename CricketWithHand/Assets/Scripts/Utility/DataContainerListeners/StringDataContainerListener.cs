
using UnityEngine;

namespace CricketWithHand.Utility
{
    public class StringDataContainerListener : GenericDataContainerListener<string> 
    {
        [SerializeField]
        private StringDataContainerSO _container;

        private void Awake()
        {
            container = _container;
        }
    }
}

