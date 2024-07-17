using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TKSR
{
    public enum EnumUIItemsType
    {
        Medic,
        Prop,
        Weapon,
        Armor,
        Accessory,
        Special
    }
    
    public class UIItemsTypeTab : MonoBehaviour
    {
        public EnumUIItemsType tabType;
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

