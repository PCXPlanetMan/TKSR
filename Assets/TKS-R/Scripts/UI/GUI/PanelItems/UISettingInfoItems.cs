using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TKSR
{
    public class UISettingInfoItems : MonoBehaviour
    {
        public UIItemsTypeTab[] btnTabs;
        public List<UIItemsSubContent> contentPanels;

        public TextMeshProUGUI goldenValue;
        
        void Awake()
        {
            foreach (var tab in btnTabs)
            {
                var btn = tab.gameObject.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(delegate
                    {
                        OnTabButtonClicked(tab);
                    });
                }
            }
        }
        
        private void OnEnable()
        {
            InitAllTabsStatus();
            RefreshGoldenValue();
        }

        private void OnDisable()
        {
            UnInitAllTabsStatus();
        }

        #region Tab Buttons
        private UIItemsTypeTab m_selectedTab;
        private void OnTabButtonClicked(UIItemsTypeTab btnTab)
        {
            if (m_selectedTab == btnTab)
            {
                Debug.Log($"[TKSR] Tab button already clicked : {m_selectedTab.tabType.ToString()}, Do Nothing.");
                return;
            }
            
            if (m_selectedTab != null)
            {
                m_selectedTab.isOn = false;
            }
            m_selectedTab = btnTab;
            Debug.Log($"[TKSR] Info Panel Tab button is pressed : {m_selectedTab.tabType.ToString()}");
            SelectTabBtnAndUpdateContent();
        }
        
        /// <summary>
        ///  每次打开Info面板将重置第一个Tab(Status)被选中状态
        /// </summary>
        private void InitAllTabsStatus()
        {
            foreach (var tab in btnTabs)
            {
                if (tab.tabType == EnumUIItemsType.Medic)
                {
                    m_selectedTab = tab;
                    break;
                }
            }
            
            SelectTabBtnAndUpdateContent();
        }

        private void SelectTabBtnAndUpdateContent()
        {
            if (m_selectedTab != null)
            {
                m_selectedTab.isOn = true;
                Debug.Log($"[TKSR] Update and Show content by type = {m_selectedTab.tabType}");

                HideAllContentPanels();
                
                if (contentPanels != null)
                {
                    var targetPanel = contentPanels.Find(x => x != null && x.panelType == m_selectedTab.tabType);
                    if (targetPanel != null)
                    {
                        targetPanel.gameObject.SetActive(true);
                    }
                }
            }
        }
        
        /// <summary>
        /// 关闭Info面板时清除状态
        /// </summary>
        private void UnInitAllTabsStatus()
        {
            foreach (var tab in btnTabs)
            {
                tab.isOn = false;
            }

            m_selectedTab = null;
            
            HideAllContentPanels();
        }
        #endregion
        

        #region All Content Panels
        private void HideAllContentPanels()
        {
            foreach (var panel in contentPanels)
            {
                if (panel != null)
                    panel.gameObject.SetActive(false);
            }
        }
        
        #endregion

        void RefreshGoldenValue()
        {
            var curDocument = DocumentDataManager.Instance.GetCurrentDocument();
            goldenValue.text = curDocument.Gold.ToString();
        }
    }
}