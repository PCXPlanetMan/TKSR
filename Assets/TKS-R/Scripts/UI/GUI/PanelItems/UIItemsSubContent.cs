using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TKSR
{
    public class InfoItemData
    {
        public int mId;
        public string mIcon;
        public string mName;
        public int mItemCount;
        public string mDesc;
    }
    
    public class IItemDataInterface : MonoBehaviour
    {
        private readonly string DEFAULT_PREFAB_NAME = "ItemPrefab";
        
        public LoopListView2 mLoopListView;
        
        protected List<InfoItemData> mItemDataList = new List<InfoItemData>();
        
        protected int m_totalDataCount = 0;
        protected bool m_isListViewInited = false;

        private void Awake()
        {
            m_isListViewInited = false;
        }
        
        private void OnEnable()
        {
            DoRefreshDataSource();
        }

        protected virtual void DoRefreshDataSource()
        {
            if (!m_isListViewInited)
            {
                mLoopListView.InitListView(m_totalDataCount, OnGetItemByIndex);
                m_isListViewInited = true;
            }
        }

        public InfoItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= mItemDataList.Count)
            {
                return null;
            }
            return mItemDataList[index];
        }

        public InfoItemData GetItemDataById(int itemId)
        {
            int count = mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                if(mItemDataList[i].mId == itemId)
                {
                    return mItemDataList[i];
                }
            }
            return null;
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= m_totalDataCount)
            {
                return null;
            }

            InfoItemData itemData = GetItemDataByIndex(index);
            if(itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem(DEFAULT_PREFAB_NAME);
            UIItemsListItem itemScript = item.GetComponent<UIItemsListItem>();
            item.name = DEFAULT_PREFAB_NAME + index;
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.parentInterface = this;
            itemScript.mLoopListView = mLoopListView;
            itemScript.SetItemData(itemData,index);
            return item;
        }

        private int m_selectedIndex = -1;
        public void SetItemSelected(int index)
        {
            int oldSelectedIndex = m_selectedIndex;
            m_selectedIndex = index;
            if (oldSelectedIndex >= 0)
            {
                var item = mLoopListView.GetShownItemByItemIndex(oldSelectedIndex);
                if (item != null)
                {
                    UIItemsListItem itemScript = item.GetComponent<UIItemsListItem>();
                    itemScript.MakeItemHighlighted(false);
                }
            }

            if (m_selectedIndex >= 0)
            {
                var item = mLoopListView.GetShownItemByItemIndex(m_selectedIndex);
                UIItemsListItem itemScript = item.GetComponent<UIItemsListItem>();
                itemScript.MakeItemHighlighted(true);
            }
        }

        // void OnJumpBtnClicked()
        // {
        //     int itemIndex = 0;
        //     mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        // }
        //
        // void OnAddItemBtnClicked()
        // {
        //     if (mLoopListView.ItemTotalCount < 0)
        //     {
        //         return;
        //     }
        //     int count = 0;
        //     
        //     count = mLoopListView.ItemTotalCount + count;
        //     if (count < 0 || count > mTotalDataCount)
        //     {
        //         return;
        //     }
        //     mLoopListView.SetListItemCount(count, false);
        // }
        //
        // void OnSetItemCountBtnClicked()
        // {
        //     int count = 0;
        //     
        //     if (count < 0 || count > mTotalDataCount)
        //     {
        //         return;
        //     }
        //     mLoopListView.SetListItemCount(count, false);
        // }
    }
    
    public class UIItemsSubContent : MonoBehaviour
    {
        public EnumUIItemsType panelType;
    }
}

