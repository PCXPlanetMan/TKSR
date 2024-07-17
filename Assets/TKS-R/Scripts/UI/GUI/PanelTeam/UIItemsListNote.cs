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
    
    
    public class UIItemsListNote : MonoBehaviour
    {
        public TextMeshProUGUI mNoteText;

        [HideInInspector]
        public INoteDataInterface parentInterface;
        int mItemDataIndex = -1;
        [HideInInspector]
        public LoopListView2 mLoopListView;

        public void Init()
        {
            // Do Nothing
        }
        
        public void SetItemData(InfoNoteData noteData, int itemIndex)
        {
            mItemDataIndex = itemIndex;
            mNoteText.GetComponent<Localize>().SetTerm(noteData.mNoteI2);
        }
    }
}