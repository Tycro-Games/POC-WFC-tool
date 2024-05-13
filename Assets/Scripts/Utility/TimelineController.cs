using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineController : MonoBehaviour
    {
        private PlayableDirector playableDirector;
        public List<TimelineAsset> timelines = null;
        private int index;

        public static event Action onTextchange = null;

        private void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        public void PlayNextTimeline()
        {
            if (timelines.Count <= index) return;
            PlayTimeline();
            index++;
        }

        public void PlayTimeline()
        {
            if (index < timelines.Count)
            {
                playableDirector.Play(timelines[index]);

                onTextchange?.Invoke();
            }
            else
            {
                Debug.LogError("NoMoreTimelines");
            }
        }

        public void End()
        {
            playableDirector.Stop();
        }
    }
}