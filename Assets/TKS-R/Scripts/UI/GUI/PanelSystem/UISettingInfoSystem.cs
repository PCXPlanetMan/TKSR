using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class UISettingInfoSystem : MonoBehaviour
    {
        public Slider bgmSlider;
        public Slider audioSlider;

        public Image bgmOnMask;
        public Image bgmOffMask;
        public Image audioOnMask;
        public Image audioOffMask;
        
        void OnEnable()
        {
            UpdatePrefsOfBGM();
            UpdatePrefsOfAudio();
        }

        private void UpdatePrefsOfBGM()
        {
            int bgmSwitchInt = PlayerPrefs.GetInt(ResourceUtils.PREFS_SYSTEM_BGM_ONOFF, 1);
            bool isBGMSwitchOn = bgmSwitchInt == 1;
            bgmOnMask.gameObject.SetActive(isBGMSwitchOn);
            bgmOffMask.gameObject.SetActive(!isBGMSwitchOn);
            bgmSlider.interactable = isBGMSwitchOn;
        }

        private void UpdatePrefsOfAudio()
        {
            int audioSwitchInt = PlayerPrefs.GetInt(ResourceUtils.PREFS_SYSTEM_AUDIO_ONOFF, 1);
            bool isAudioSwitchOn = audioSwitchInt == 1;
            audioOnMask.gameObject.SetActive(isAudioSwitchOn);
            audioOffMask.gameObject.SetActive(!isAudioSwitchOn);
            audioSlider.interactable = isAudioSwitchOn;
        }

        public void OnClickBGMOnOff(bool on)
        {
            PlayerPrefs.SetInt(ResourceUtils.PREFS_SYSTEM_BGM_ONOFF, on ? 1 : 0);
            UpdatePrefsOfBGM();
        }

        public void OnClickAudioOnOff(bool on)
        {
            PlayerPrefs.SetInt(ResourceUtils.PREFS_SYSTEM_AUDIO_ONOFF, on ? 1 : 0);
            UpdatePrefsOfAudio();
        }
    }
}