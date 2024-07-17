using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace TKSR
{
    public class UIToastPanel : MonoBehaviour
    {
        public UIShowToastItem[] toasts;

        
        public void ShowToastForItem(int itemId, int itemCount, float showDuration)
        {
            var itemToast = PopToastItem();
            if (itemToast == null)
            {
                Debug.LogError("Want to show toast but there is no any toast ui existed.");
                return;
            }
            
            string i2NamePath = ItemsController.Instance.ParseItemI2LocalName(itemId);
            string strItemLocName = I2.Loc.LocalizationManager.GetTranslation(i2NamePath);
            if (itemCount == 1)
            {
                itemToast.DisplayToastItem(strItemLocName, showDuration);
            }
            if (itemCount == -1)
            {
                itemToast.DisplayToastItem(strItemLocName, showDuration);
            }
            else if (itemCount > 1)
            {
                // [TODO] 物品x数量
            }
            else if (itemCount < -1)
            {
                
            }
        }

        public void ShowToastForMoney(int gold, float showDuration)
        {
            var itemToast = PopToastItem();
            if (itemToast == null)
            {
                Debug.LogError("Want to show toast but there is no any toast ui existed.");
                return;
            }
            
            if (gold > 0)
                itemToast.DisplayGoldValue(gold, showDuration);
            else if (gold < 0)
            {
                
            }
        }

        public void ShowToastNotEnoughMoney(int gold, float showDuration)
        {
            var itemToast = PopToastItem();
            if (itemToast == null)
            {
                Debug.LogError("Want to show toast but there is no any toast ui existed.");
                return;
            }
            
            if (gold > 0)
                itemToast.DisplayToastNotEnoughGoldWarning(gold, showDuration);
            else if (gold < 0)
            {
                
            }
        }

        private UIShowToastItem PopToastItem()
        {
            for (int i = 0; i < toasts.Length; i++)
            {
                if (!toasts[i].isPop)
                {
                    return toasts[i];
                }
            }

            return null;
        }
    }
}