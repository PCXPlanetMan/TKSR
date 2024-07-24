using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UIShowToastItem : MonoBehaviour
    {
        public TextMeshProUGUI toastContent;
        [HideInInspector]
        public bool isPop = false;
        

        private float m_toastShowDuration = 0f;
        
        public void DisplayToastItem(string strItem, float showDuration)
        {
            m_toastShowDuration = showDuration;
            gameObject.SetActive(true);
            m_timeTick = 0f;
            isPop = true;
            
            toastContent.gameObject.GetComponent<Localize>().SetTerm(ResourceUtils.I2FORMAT_GOT_ITEM);
            var localManager = toastContent.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_FORMAT_ITEMNAME, strItem);
        }

        public void DisplayGoldValue(int goldCount, float showDuration)
        {
            m_toastShowDuration = showDuration;
            gameObject.SetActive(true);
            m_timeTick = 0f;
            isPop = true;
            
            toastContent.gameObject.GetComponent<Localize>().SetTerm(ResourceUtils.I2FORMAT_GOT_MONEY);
            var localManager = toastContent.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_FORMAT_GOLDCOUNT, goldCount.ToString());
        }

        public void DisplayToastNotEnoughGoldWarning(int gold, float showDuration)
        {
            m_toastShowDuration = showDuration;
            gameObject.SetActive(true);
            m_timeTick = 0f;
            isPop = true;
            
            toastContent.gameObject.GetComponent<Localize>().SetTerm(ResourceUtils.I2FORMAT_GOLD_NOT_ENOUGH);
            var localManager = toastContent.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_FORMAT_GOLDCOUNT, gold.ToString());
        }
        
        public void DisplayToast(string strI2Format, float showDuration)
        {
            m_toastShowDuration = showDuration;
            gameObject.SetActive(true);
            m_timeTick = 0f;
            isPop = true;

            strI2Format = "Format/" + strI2Format;
            toastContent.gameObject.GetComponent<Localize>().SetTerm(strI2Format);
        }

        private float m_timeTick = 0f;

        void Update()
        {
            m_timeTick += Time.deltaTime;
            if (m_toastShowDuration > 0f && m_timeTick > m_toastShowDuration)
            {
                gameObject.SetActive(false);
                isPop = false;
            }
        }
    }
}