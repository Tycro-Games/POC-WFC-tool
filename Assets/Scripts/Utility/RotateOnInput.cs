using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class RotateOnInput : MonoBehaviour
    {
        private Quaternion desiredRotation = Quaternion.identity;


        public void RotateTo(Transform transf)
        {
            desiredRotation = Quaternion.LookRotation(transf.position - transform.position);


            InstantRotation();
        }

        public void RotateTo(Vector3 pos)
        {
            desiredRotation = Quaternion.LookRotation(pos - transform.position);


            InstantRotation();
        }

        public void InstantRotation()
        {
            transform.rotation = desiredRotation;
        }
    }
}