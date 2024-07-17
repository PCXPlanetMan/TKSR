using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TKSR
{
    public class UIInfoPanel : MonoBehaviour
    {
        public UIInfoTab[] btnTabs;
        public UIMemberSlot[] memberSlots;
        public List<UIInfoContent> contentPanels;

        private GameUI m_parentGameUI;
        private SwipeGestureRecognizer swipeGesture;
        private TapGestureRecognizer twoFingersTap;
        
        void Awake()
        {
            m_parentGameUI = GetComponentInParent<GameUI>();
            
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
            
            HideTeamMembers();
            HideAllContentPanels();

            CreateSwipeGesture();
            CreateTwoFingersTapGesture();
        }
        
        private void OnEnable()
        {
            InitAllTabsStatus();
            UpdateTeamMembersInfo();

            if (PlayerCharacter.PlayerInstance != null)
            {
                PlayerCharacter.PlayerInstance.PausePlayer(true);
            }
            
            // FingersScript.Instance.AddGesture(swipeGesture);
            FingersScript.Instance.AddGesture(twoFingersTap);
        }

        private void OnDisable()
        {
            UnInitAllTabsStatus();
            
            if (PlayerCharacter.PlayerInstance != null)
            {
                PlayerCharacter.PlayerInstance.PausePlayer(false);
            }
            
            if (FingersScript.HasInstance)
            {
                // FingersScript.Instance.RemoveGesture(swipeGesture);
                FingersScript.Instance.RemoveGesture(twoFingersTap);
            }
        }
        
        private void CreateSwipeGesture()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
        }
        
        private void CreateTwoFingersTapGesture()
        {
            twoFingersTap = new TapGestureRecognizer();
            twoFingersTap.MinimumNumberOfTouchesToTrack = twoFingersTap.MaximumNumberOfTouchesToTrack = 2;
            twoFingersTap.StateUpdated += TwoTapGestureCallback;
        }
        
        private void TwoTapGestureCallback(DigitalRubyShared.GestureRecognizer tapGesture)
        {
            if (tapGesture.State == GestureRecognizerState.Ended)
            {
                int touchCount = (tapGesture as TapGestureRecognizer).TapTouches.Count;
                Debug.LogFormat("TwoTapGestureCallback touchCount: {0}", touchCount);
                if (touchCount == 2)
                {
                    m_parentGameUI.ShowUIPanel(EnumUIPanelType.None);
                }
            }
        }
        
        private void SwipeGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                SwipeGestureRecognizer swipe = gesture as SwipeGestureRecognizer;
                float angle = Mathf.Atan2(swipe.DistanceY, swipe.DistanceX) * Mathf.Rad2Deg;
                Debug.LogFormat("Swipe dir: {0}, angle = {1}", swipe.EndDirection, angle);
                // Debug.LogFormat("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}", gesture.StartFocusX, gesture.StartFocusY, gesture.FocusX, gesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY);

                if (swipe.EndDirection == SwipeGestureRecognizerDirection.Right)
                {
                    m_parentGameUI.ShowUIPanel(EnumUIPanelType.None);
                }
            }
        }
        
        

        #region Tab Buttons
        private UIInfoTab m_selectedTab;
        private void OnTabButtonClicked(UIInfoTab btnTab)
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
                if (tab.tabType == EnumUIInfoType.Status)
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

        #region Members Info
        private void HideTeamMembers()
        {
            foreach (var slot in memberSlots)
            {
                slot.gameObject.SetActive(false);
            }
        }
        
        private int _selectTeamMemberIndex;
        [HideInInspector]
        public int SelectTeamMemberIndex
        {
            get => _selectTeamMemberIndex;
            private set => _selectTeamMemberIndex = value;
        }

        private void UpdateTeamMembersInfo()
        {
            if (memberSlots == null || memberSlots.Length == 0)
            {
                Debug.LogError("[TKSR] There is no Member slot UI Component existed.");
                return;
            }
            
            HideTeamMembers();
            
            var currentDocument = DocumentDataManager.Instance.GetCurrentDocument();
            if (currentDocument != null && memberSlots.Length > 0)
            {
                var slot = memberSlots[_selectTeamMemberIndex];
                if (slot != null)
                {
                    slot.UpdateMemberInfo(currentDocument.MainRoleInfo.CharName, currentDocument.MainRoleInfo.HP, currentDocument.MainRoleInfo.MP);
                    slot.gameObject.SetActive(true);
                }
            }
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

        public void OnClickPanelSystemSave()
        {
            m_parentGameUI.ShowUIPanel(EnumUIPanelType.Save, EnumUIPanelShowMode.Pop);
        }

        public void OnClickPanelSystemLoad()
        {
            m_parentGameUI.ShowUIPanel(EnumUIPanelType.Load, EnumUIPanelShowMode.Pop);
        }

        public void OnClickPanelSystemExit()
        {
            m_parentGameUI.ShowUIPanel(EnumUIPanelType.MainTitle);
            SceneManager.LoadScene(0);
        }
        
        #endregion
    }
}