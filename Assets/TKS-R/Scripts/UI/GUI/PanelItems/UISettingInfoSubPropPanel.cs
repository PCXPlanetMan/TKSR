using UnityEngine;

namespace TKSR
{
    public class UISettingInfoSubPropPanel : IItemDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            mItemDataList.Clear();
            if (document.Props == null)
            {
                Debug.LogWarning($"[TKSR] There is no Props inited.");
                return;
            }

            if (document.Props.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Props Count is Zero.");
                return;
            }
            m_totalDataCount = document.Props.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoItemData tData = new InfoItemData();
                var prop = document.Props[i];
                tData.mId = prop.ItemId;
                BaseItems baseItem;
                var type = ItemsController.Instance.LookupItemType(prop.ItemId, out baseItem);
                tData.mName = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_PROPS + baseItem.I2Name;
                tData.mDesc = tData.mName + "_Desc";
                tData.mIcon = baseItem.Icon;
                tData.mItemCount = prop.Count;
                mItemDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}

