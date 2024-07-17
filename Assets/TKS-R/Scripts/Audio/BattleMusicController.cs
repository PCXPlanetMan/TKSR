using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class BattleMusicController : MonoBehaviour
    {
        [Header("Opening Settings")]
        [Range(0f, 1f)]
        public float openingVolume = 1f;
        
        private AudioSource m_MusicAudioSource;
        void Awake()
        {
            m_MusicAudioSource = gameObject.GetComponent<AudioSource> ();
            m_MusicAudioSource.loop = false;
            m_MusicAudioSource.volume = openingVolume;

            m_MusicAudioSource.time = 0f;
            m_MusicAudioSource.Play();
        }

        private float m_openingDuration = 0f;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(CheckOpeningFinished());
        }

        void Update()
        {
            m_openingDuration += Time.deltaTime;
        }

        private IEnumerator CheckOpeningFinished()
        {
            while (m_MusicAudioSource.isPlaying && m_openingDuration < 4f)
            {
                yield return new WaitForSeconds(0.1f);
            }

            PlayBackgroundMusic();
        }

        private void PlayBackgroundMusic()
        {
            BackgroundMusicPlayer.Instance.PlayJustAmbient();
            this.gameObject.SetActive(false);
        }
    }
}
