using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TKSR
{
    public enum EnumUIInfoType
    {
        Status,
        Skill,
        Item,
        Equipment,
        Team,
        System
    }
    
    public class UIInfoTab : MonoBehaviour
    {
        public EnumUIInfoType tabType;
        public Image highLighted;

        [HideInInspector]
        public bool isOn
        {
            set
            {
                m_isOn = value;
                if (highLighted != null)
                {
                    highLighted.gameObject.SetActive(m_isOn);
                }
            }
            get => m_isOn;
        }

        private bool m_isOn = false;
    }
}

