using System.Collections;
using UnityEngine;

namespace Jam.Utility
{
    public class LookAtPlayer : MonoBehaviour
    {
        private Transform playerTransform = null;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            Vector3 target = playerTransform.position;
            transform.LookAt(target);
        }
    }
}