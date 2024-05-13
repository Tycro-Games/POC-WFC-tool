using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private UnityEvent onTimerFinished = null;
        [SerializeField] private UnityEvent onTimerEnabled = null;
        [SerializeField] private UnityEvent onTimerDisabled = null;

        [SerializeField] private float timeToWait = 0;
        [SerializeField] private bool loop = false;


        private void OnDisable()
        {
            onTimerDisabled?.Invoke();

            StopAllCoroutines();
        }

        private void OnEnable()
        {
            onTimerEnabled?.Invoke();

            StartCoroutine(TimerLoop());
        }

        private IEnumerator TimerLoop()
        {
            yield return new WaitForSeconds(timeToWait);
            onTimerFinished?.Invoke();

            if (loop)
                StartCoroutine(TimerLoop());
        }
    }
}