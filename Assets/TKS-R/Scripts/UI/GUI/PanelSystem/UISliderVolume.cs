using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public enum EnumSliderType
    {
        BGM,
        AUDIO,
    }

    public class UISliderVolume : MonoBehaviour
    {
        private Animator m_SliderAnim;
        private Slider m_Slider;
        public EnumSliderType type;

        private static Dictionary<EnumSliderType, string> s_dictPrefs = new Dictionary<EnumSliderType, string>()
        {
            { EnumSliderType.BGM, ResourceUtils.PREFS_SYSTEM_BGM_VOLUME },
            { EnumSliderType.AUDIO, ResourceUtils.PREFS_SYSTEM_AUDIO_VOLUME }
        };

        private readonly int m_HashVolumePara = Animator.StringToHash("Volume");

        private int m_curVolumeInt = 0;
        private string m_keyPref;

        void Awake()
        {
            m_Slider = GetComponent<Slider>();
            m_SliderAnim = GetComponent<Animator>();

            m_Slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(m_Slider); });

            m_keyPref = s_dictPrefs[type];
        }

        void OnEnable()
        {
            if (!string.IsNullOrEmpty(m_keyPref))
            {
                m_curVolumeInt = PlayerPrefs.GetInt(m_keyPref, 0);
                m_SliderAnim.SetInteger(m_HashVolumePara, m_curVolumeInt);
            }
            else
            {
                Debug.LogError($"[TKSR] Empty key pref of GameObject = {gameObject.name}");
            }
        }

        private void OnSliderValueChanged(Slider s)
        {
            int intValue = (int)(s.value * 100);
            int animVolumeInt = 0;
            if (intValue == 0)
            {
                animVolumeInt = 0;
            }
            else
            {
                animVolumeInt = Mathf.CeilToInt(intValue / 20f);
            }

            m_SliderAnim.SetInteger(m_HashVolumePara, animVolumeInt);

            if (m_curVolumeInt != animVolumeInt)
            {
                m_curVolumeInt = animVolumeInt;
                PlayerPrefs.SetInt(m_keyPref, m_curVolumeInt);
            }
        }
    }
}