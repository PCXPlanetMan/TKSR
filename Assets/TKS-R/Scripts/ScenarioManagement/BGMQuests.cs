using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 通过判断Quest来判断播放BGM
    /// </summary>
    public class BGMQuests : QuestsChecker
    {
        public AudioClip newAudio;

        void Start()
        {
            if (AreConditionsSatisfied())
            {
                var musicPlayer = gameObject.GetComponent<BackgroundMusicPlayer>();
                if (newAudio != null)
                    musicPlayer.PlayChangeAmbient(newAudio);
                else
                {
                    musicPlayer.StopJustAmbient();
                }
            }
        }
    }
}