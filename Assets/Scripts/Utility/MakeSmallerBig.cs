using System.Collections;
using UnityEngine;

namespace Jam.Utility
{
    public class MakeSmallerBig : MonoBehaviour
    {
        [SerializeField]
        private float howSmall = 0.1f;

        [SerializeField]
        private float speed = 1.0f;

        [SerializeField]
        private AnimationCurve speedCurve = null;

        private float currentTime = 0;

        [SerializeField]
        private float duration = 1.0f;

        public void MakeSmallAndBig()
        {
            StopAllCoroutines();
            transform.localScale = Vector3.one * howSmall;
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            while (transform.localScale != Vector3.one)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one,
                     Time.deltaTime * speed * speedCurve.Evaluate(TimeManagement()));//magic animation curve

                yield return null;
            }
        }

        private float TimeManagement()
        {
            currentTime += Time.deltaTime;//currentTime=currentTime+Time.deltaTime
            return currentTime / duration;//return values from 0 to 1
        }
    }
}