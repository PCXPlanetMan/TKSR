using UnityEngine;

namespace TKSR
{
    public class UISettingInfoSubArmorPanel : IItemDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            mItemDataList.Clear();
            if (document.Armors == null)
            {
                Debug.LogWarning($"[TKSR] There is no Armors inited.");
                return;
            }

            if (document.Armors.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Armors Count is Zero.");
                return;
            }
            m_totalDataCount = document.Armors.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoItemData tData = new InfoItemData();
                var armor = document.Armors[i];
                tData.mId = armor.ItemId;
                BaseItems baseItem;
                var type = ItemsController.Instance.LookupItemType(armor.ItemId, out baseItem);
                tData.mName = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_ARMORS + baseItem.I2Name;
                tData.mDesc = tData.mName + "_Desc";
                tData.mIcon = baseItem.Icon;
                tData.mItemCount = armor.Count;
                mItemDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}

