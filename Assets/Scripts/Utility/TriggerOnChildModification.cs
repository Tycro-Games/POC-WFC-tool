using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utility
{
    [ExecuteInEditMode]
    public class TriggerOnChildModification : MonoBehaviour
    {
        private int previousChildCount;
        [SerializeField] private UnityEvent onChildModification;

        private void Start()
        {
            // Initialize the previousChildCount on Start
            previousChildCount = transform.hierarchyCount;
        }

        private void Update()
        {
            // Check if child count has changed
            if (transform.hierarchyCount != previousChildCount)
            {
                Debug.Log("Children were added or deleted.");
                onChildModification?.Invoke();
                // Update the previousChildCount with the current child count
                previousChildCount = transform.hierarchyCount;
            }
        }
    }
}