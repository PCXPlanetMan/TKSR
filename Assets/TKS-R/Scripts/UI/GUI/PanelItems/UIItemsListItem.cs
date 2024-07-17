using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using SuperScrollView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class UIItemsListItem : MonoBehaviour
    {
        public Image mIcon;
        public TextMeshProUGUI mNameText;
        public TextMeshProUGUI mDescText;
        public TextMeshProUGUI mCount;
        public Image mSubPanel;
        public Image highlightBg;

        [HideInInspector]
        public IItemDataInterface parentInterface;
        int mItemDataIndex = -1;
        [HideInInspector]
        public LoopListView2 mLoopListView;

        public void Init()
        {
            ClickEventListener listener = ClickEventListener.Get(this.gameObject);
            listener.SetClickEventHandler(delegate(GameObject obj) { OnClickItemContent(); });
            listener.SetDoubleClickEventHandler(delegate(GameObject o) { OnDoubleClickItemContent(); });
            listener.SetPointerDownHandler(delegate(GameObject o) { OnPointDownItemContent(); });
            listener.SetPointerUpHandler(delegate(GameObject o) { OnPointUpItemContent(); });
        }

        void OnDisable()
        {
            MakeItemHighlighted(false);
        }

        void OnClickItemContent()
        {
            InfoItemData data = parentInterface.GetItemDataByIndex(mItemDataIndex);
            if (data == null)
            {
                return;
            }
            Debug.Log($"[TKSR] Click Item by Index = {mItemDataIndex}");
        }
        
        void OnDoubleClickItemContent()
        {
            InfoItemData data = parentInterface.GetItemDataByIndex(mItemDataIndex);
            if (data == null)
            {
                return;
            }
            Debug.Log($"[TKSR] Double Click Item by Index = {mItemDataIndex}");
        }
        
        void OnPointDownItemContent()
        {
            Debug.Log($"[TKSR] PointDown Item by Index = {mItemDataIndex}");
            m_isTouchBegan = true;
            m_TouchTime = 0f;
            parentInterface.SetItemSelected(mItemDataIndex);
        }
        
        void OnPointUpItemContent()
        {
            m_isTouchBegan = false;
            m_TouchTime = 0f;
            ShowDescriptionPanel(false);
            Debug.Log($"[TKSR] PointUp Item by Index = {mItemDataIndex}");
        }

        public void SetItemData(InfoItemData itemData, int itemIndex)
        {
            mItemDataIndex = itemIndex;
            mIcon.sprite = ItemsController.Instance.LoadItemIconSpriteById(itemData.mIcon);
            mNameText.GetComponent<Localize>().SetTerm(itemData.mName);
            mDescText.GetComponent<Localize>().SetTerm(itemData.mDesc);
            mCount.text = itemData.mItemCount.ToString();
            MakeItemHighlighted(false);
        }

        // 判断长按以触发显示Description
        private bool m_isTouchBegan = false;
        private float m_TouchTime = 0f;
        void Update()
        {
            if (m_isTouchBegan)
            {
                m_TouchTime += Time.deltaTime;
                if (m_TouchTime > 0.5f) // 判断长按的时间阈值
                {
                    ShowDescriptionPanel(true);
                    m_isTouchBegan = false;
                    m_TouchTime = 0f;
                }
            }
        }

        // 单个Item的固定高度,需要根据美术资源调整
        private readonly float FIXED_ITEM_HEIGHT = 80;
        // 修正Description的显示位置的经验值
        private readonly int CHK_SHOW_DESC_INDEX = 5;

        /// <summary>
        /// 根据经验值修正Description的显示位置
        /// </summary>
        /// <param name="show"></param>
        private void FixedDescriptionShowSide(bool show)
        {
            if (show)
            {
                var subPanelPos = mSubPanel.rectTransform.anchoredPosition;
                if (subPanelPos.y < -FIXED_ITEM_HEIGHT * CHK_SHOW_DESC_INDEX)
                {
                    Vector2 newSubPanelPos = new Vector2(subPanelPos.x, subPanelPos.y + FIXED_ITEM_HEIGHT + 400); // 400是Description面板的高度,需要根据UI资源进行调整
                    mSubPanel.rectTransform.anchoredPosition = newSubPanelPos;
                }
            }
            else
            {
                var subPanelPos = mSubPanel.rectTransform.anchoredPosition;
                Vector2 newSubPanelPos = new Vector2(subPanelPos.x, -FIXED_ITEM_HEIGHT);
                mSubPanel.rectTransform.anchoredPosition = newSubPanelPos;
            }
        }
        
        private void ShowDescriptionPanel(bool show)
        {
            mSubPanel.gameObject.SetActive(show);
            if (mLoopListView != null)
            {
                if (show)
                {
                    mSubPanel.transform.SetParent(mLoopListView.transform);
                }
                else
                {
                    mSubPanel.transform.SetParent(this.transform);
                }

                FixedDescriptionShowSide(show);
            }
        }

        public void MakeItemHighlighted(bool highlight)
        {
            if (highlight)
            {
                highlightBg.gameObject.SetActive(true);
                mNameText.color = Color.red;
                mCount.color = Color.red;
            }
            else
            {
                highlightBg.gameObject.SetActive(false);
                mNameText.color = Color.white;
                mCount.color = Color.white;
            }
        }
    }
}