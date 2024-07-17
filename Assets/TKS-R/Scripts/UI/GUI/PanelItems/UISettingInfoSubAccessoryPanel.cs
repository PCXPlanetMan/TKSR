using UnityEngine;

namespace TKSR
{
    public class UISettingInfoSubAccessoryPanel : IItemDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            mItemDataList.Clear();
            if (document.Accessories == null)
            {
                Debug.LogWarning($"[TKSR] There is no Accessories inited.");
                return;
            }

            if (document.Accessories.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Accessories Count is Zero.");
                return;
            }
            m_totalDataCount = document.Accessories.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoItemData tData = new InfoItemData();
                var accessory = document.Accessories[i];
                tData.mId = accessory.ItemId;
                BaseItems baseItem;
                var type = ItemsController.Instance.LookupItemType(accessory.ItemId, out baseItem);
                tData.mName = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_ACCESSORIES + baseItem.I2Name;
                tData.mDesc = tData.mName + "_Desc";
                tData.mIcon = baseItem.Icon;
                tData.mItemCount = accessory.Count;
                mItemDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}

