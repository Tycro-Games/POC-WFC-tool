using System.Collections;
using UnityEngine;

namespace Jam.Utility
{
    public class MusicLoop : MonoBehaviour
    {
        public AudioSource musicSource;
        public AudioClip musicStart;
        public AudioClip musicLoop;

        private IEnumerator Start()
        {
            musicSource.PlayOneShot(musicStart);
            while (musicSource.isPlaying)
                yield return null;
            musicSource.loop = true;
            musicSource.clip = musicLoop;
            musicSource.Play();
        }
    }
}