using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class RotateOnTime : MonoBehaviour
    {
        [SerializeField] private AnimationCurve speedCurve = null;
        [SerializeField] private float duration = 1.0f;
        private Quaternion desiredRotation = Quaternion.identity;
        private float elapsedTime = 0;

        public void RotateTo(Transform transf)
        {
            if (transf.position == transform.position)
                return;
            desiredRotation = Quaternion.LookRotation(transf.position - transform.position);
            elapsedTime = 0;

            StopAllCoroutines();

            StartCoroutine(SmoothRotation());
        }

        public void RotateTo(Vector3 pos)
        {
            desiredRotation = Quaternion.LookRotation(pos - transform.position);
            elapsedTime = 0;

            StopAllCoroutines();

            StartCoroutine(SmoothRotation());
        }

        public IEnumerator SmoothRotation()
        {
            var startRotation = transform.rotation;
            while (transform.rotation != desiredRotation)
            {
                transform.rotation = Quaternion.Slerp(startRotation, desiredRotation,
                    speedCurve.Evaluate(elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}