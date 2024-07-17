using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TKSR
{
    public enum EnumNoteExtraFormat
    {
        None = 0,
        UseMainName,
    }
    
    public class InfoNoteData
    {
        public string mNoteI2;
        public EnumNoteExtraFormat ExtraFormat = EnumNoteExtraFormat.None;
    }
    
    public class INoteDataInterface : MonoBehaviour
    {
        private readonly string DEFAULT_PREFAB_NAME = "NotePrefab";
        
        public LoopListView2 mLoopListView;
        
        protected List<InfoNoteData> mNoteDataList = new List<InfoNoteData>();
        
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

        public InfoNoteData GetNoteDataByIndex(int index)
        {
            if (index < 0 || index >= mNoteDataList.Count)
            {
                return null;
            }
            return mNoteDataList[index];
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= m_totalDataCount)
            {
                return null;
            }

            InfoNoteData itemData = GetNoteDataByIndex(index);
            if(itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem(DEFAULT_PREFAB_NAME);
            UIItemsListNote itemScript = item.GetComponent<UIItemsListNote>();
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
    }
    
    public class UITeamSubContent : MonoBehaviour
    {
        public EnumUITeamType panelType;
    }
}

