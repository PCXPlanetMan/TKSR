using UnityEngine;

namespace TKSR
{
    public class UISettingInfoSubSpecialPanel : IItemDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            mItemDataList.Clear();
            if (document.Specials == null)
            {
                Debug.LogWarning($"[TKSR] There is no Specials inited.");
                return;
            }

            if (document.Specials.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Specials Count is Zero.");
                return;
            }
            m_totalDataCount = document.Specials.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoItemData tData = new InfoItemData();
                var special = document.Specials[i];
                tData.mId = special.ItemId;
                BaseItems baseItem;
                var type = ItemsController.Instance.LookupItemType(special.ItemId, out baseItem);
                tData.mName = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_SPECIALS + baseItem.I2Name;
                tData.mDesc = tData.mName + "_Desc";
                tData.mIcon = baseItem.Icon;
                tData.mItemCount = special.Count;
                mItemDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}

