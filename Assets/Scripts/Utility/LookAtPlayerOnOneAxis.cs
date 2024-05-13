using System.Collections;
using UnityEngine;

namespace Jam.Utility
{
    public class LookAtPlayerOnOneAxis : MonoBehaviour
    {
        private Transform playerTransform = null;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            Vector3 target = playerTransform.position;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }
}