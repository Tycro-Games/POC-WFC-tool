using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Utility
{
    public class PredefinedPathMover : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnStartPath;
        [SerializeField] private TransformEvent OnReachWaypoint;
        [SerializeField] private UnityEvent OnEndPath;
        [SerializeField] private bool loop = false;

        public Color anchorCol = Color.red;

        public Color selectedSegmentCol = Color.yellow;
        public float anchorDiameter = .1f;

        public bool displayControlPoints = true;

        [Header("Path Settings")] //
        [SerializeField]
        private Transform pathHolder = null;

        [SerializeField] private AnimationCurve deviationCurve = null;
        [SerializeField] private Vector3 deviationAxis = Vector3.zero;
        [SerializeField] private float deviationMax = 3.0f;


        //[SerializeField] private float closeDistance = 0.2f;

        [Header("Object movement")] //
        [SerializeField]
        private AnimationCurve speedCurve = null;

        [SerializeField] private float maxAccelerationTime = 2.0f;
        [SerializeField] private float waitTime = .3f;


        //[SerializeField]
        [Header("Gizmos settings")] //
        [SerializeField]
        private int resolution = 50;

        [SerializeField] private bool displayBoxes;

        [SerializeField] private bool gizmoActive = true;

        [SerializeField] private Color color;

        [SerializeField] private bool randomColor = true;


        public List<Transform> waypoints = new();


        private int currentIndex = 0;

        private Transform targetWaypoint;


        private float currentTime = 0;

        public enum States
        {
            Idleing,
            Walking
        }

        private int LoopIndex(int i)
        {
            return (i + waypoints.Count) % waypoints.Count;
        }


        public void RandomizeDirection()
        {
            deviationAxis = Random.insideUnitSphere;
        }

        private void Awake()
        {
            if (randomColor)
                color = Random.ColorHSV();

            GetWayPoints();
            SetUp();
        }


        public void Resume()
        {
            FollowPath();
        }

        public void GetWayPoints()
        {
            waypoints.Clear();
            for (var i = 0; i < pathHolder.childCount; i++) waypoints.Add(pathHolder.GetChild(i));
        }

        private void SetUp()
        {
            deviationAxis = deviationAxis.normalized;


            currentTime = 0.0f;
        }


        public Vector3[] GetPointsInSegment(int i)
        {
            return new[]
            {
                waypoints[i * 3].position, waypoints[i * 3 + 1].position, waypoints[i * 3 + 2].position,
                waypoints[LoopIndex(i * 3 + 3)].position
            };
        }

        public void MovePoint(int i, Vector3 pos)
        {
            var deltaMove = pos - waypoints[i].position;


            if (i + 1 < waypoints.Count) deviationAxis += deltaMove;
            if (i - 1 >= 0) deviationAxis += deltaMove;


            deviationAxis = deviationAxis.normalized;
        }

        private void Update()
        {
        }

        private float TimeManagement()
        {
            currentTime += Time.deltaTime;
            return currentTime / maxAccelerationTime; //return values from 0 to 1
        }

        private IEnumerator MoveToTarget(Transform target)
        {
            //this function just moves a thing from point A to B
            var initialPosition = transform.position;
            while (transform.position != target.position)
            {
                var time = TimeManagement();


                //set

                var pos = Vector3.Lerp(initialPosition, target.position,
                    speedCurve.Evaluate(time)); //magic animation curve


                var deviationLen = Mathf.Lerp(0.0f, deviationMax, deviationCurve.Evaluate(time));
                var deviation = deviationAxis * deviationLen;
                // Visualize the path using Gizmos.DrawLine
                //Debug.Log(deviation);
                transform.position = pos + deviation;
                if (time >= 1.0f)
                    break;
                yield return null;
            }
        }

        private void FollowPath()
        {
            if (currentIndex >= waypoints.Count)
                return;
            targetWaypoint = waypoints[currentIndex];

            StartCoroutine(ReachWaypoint());
        }

        private IEnumerator ReachWaypoint()
        {
            yield return StartCoroutine(MoveToTarget(targetWaypoint));

            if (loop)
            {
                currentIndex = (currentIndex + 1) % waypoints.Count;
            }
            else
            {
                currentIndex++;
                if (currentIndex >= waypoints.Count)
                {
                    OnEndPath?.Invoke();
                    currentIndex = 0;
                }
            }

            yield return new WaitForSeconds(waitTime);
            currentTime = 0;
            OnReachWaypoint?.Invoke(targetWaypoint);
        }

        private float gizmoAnimationTime = 0f; // Variable to control the animation time

        //TODO switch the boxes to something else
        private void OnDrawGizmos()

        {
            if (!gizmoActive)
                return;
            Gizmos.color = color;
            if (pathHolder.childCount == 0)
                return;
            var startPosition = pathHolder.GetChild(0).position;
            // Draw path with animation curves for movement
            var lastP = pathHolder.GetChild(0).position;
            for (var j = 1; j <= pathHolder.childCount; j++)
            {
                Vector3 nextP;
                if (j != pathHolder.childCount)
                    nextP = pathHolder.GetChild(j).position;
                else
                    nextP = startPosition;

                var pos = lastP;
                var initialPos = lastP;


                for (var i = 0; i <= resolution; i++)
                {
                    var t = i / (float)resolution;

                    // Use animation curves for speed and deviation
                    var speedT = speedCurve.Evaluate(t);
                    var deviationT = deviationCurve.Evaluate(t);

                    var nextPos = Vector3.Lerp(initialPos, nextP, speedT);

                    // Adjust position based on deviation
                    var deviationLen = Mathf.Lerp(0.0f, deviationMax, deviationT);
                    var deviation = deviationAxis * deviationLen;

                    // Draw line
                    // Apply animation to the cubes
                    if (displayBoxes)
                        Gizmos.DrawCube(nextPos + deviation,
                            Vector3.one * 0.1f);
                    Gizmos.DrawLine(pos, nextPos + deviation);
                    //nextPos += deviation;

                    // Update position for the next iteration
                    pos = nextPos + deviation;
                }


                lastP = nextP;
            }


            // Increment the animation time
            var previousPosition = startPosition;


            foreach (Transform waypoint in pathHolder)
            {
                Gizmos.DrawSphere(waypoint.position, .1f);
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }

            Gizmos.DrawLine(previousPosition, startPosition);
        }

        private float lastGizmoUpdateTime = 0.0f;

        private void OnDrawGizmosSelected()
        {
            if (!gizmoActive)
                return;

            Gizmos.color = color;

            if (pathHolder.childCount == 0)
                return;

            var startPosition = pathHolder.GetChild(0).position;
            var lastP = pathHolder.GetChild(0).position;

            for (var j = 1; j <= pathHolder.childCount; j++)
            {
                Vector3 nextP;
                if (j != pathHolder.childCount)
                    nextP = pathHolder.GetChild(j).position;
                else
                    nextP = startPosition;

                var pos = lastP;
                var initialPos = lastP;
                // Adjust position based on deviation
                var t = gizmoAnimationTime / maxAccelerationTime;
                var deviationT = deviationCurve.Evaluate(t);
                var deviationLen = Mathf.Lerp(0.0f, deviationMax, deviationT);
                var deviation = deviationAxis * deviationLen;
                Gizmos.DrawCube(
                    Vector3.Lerp(initialPos, nextP,
                        speedCurve.Evaluate(t)) + deviation,
                    Vector3.one * 0.3f);
                lastP = nextP;
            }

            gizmoAnimationTime += Time.realtimeSinceStartup - lastGizmoUpdateTime;

            if (maxAccelerationTime < gizmoAnimationTime)
                gizmoAnimationTime = -waitTime;

            lastGizmoUpdateTime = Time.realtimeSinceStartup;
        }
    }
}