using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UIAutoSetFontSpace : MonoBehaviour
    {
        private TextMeshProUGUI m_text;

        // Start is called before the first frame update
        void Awake()
        {
            m_text = this.gameObject.GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// 动态调整文字字符间隔
        /// </summary>
        public void CallbackSetLocalizedText()
        {
            if (m_text != null && !string.IsNullOrEmpty(m_text.text))
            {
                int charCount = m_text.text.Length;
                if (charCount >= 5)
                {
                    m_text.characterSpacing = 0;
                }
                else if (charCount == 4)
                {
                    m_text.characterSpacing = 20;
                }
                else if (charCount == 3)
                {
                    m_text.characterSpacing = 40;
                }
                else
                {
                    m_text.characterSpacing = 100;
                }
            }
        }
    }
}