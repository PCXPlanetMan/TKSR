using UnityEngine;

namespace TKSR
{
    public class UISettingInfoSubMedicPanel : IItemDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            mItemDataList.Clear();
            if (document.Medics == null)
            {
                Debug.LogWarning($"[TKSR] There is no Medics inited.");
                return;
            }

            if (document.Medics.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Medics Count is Zero.");
                return;
            }
            
            m_totalDataCount = document.Medics.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoItemData tData = new InfoItemData();
                var medic = document.Medics[i];
                tData.mId = medic.ItemId;
                BaseItems baseItem;
                var type = ItemsController.Instance.LookupItemType(medic.ItemId, out baseItem);
                tData.mName = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_MEDICS + baseItem.I2Name;
                tData.mDesc = tData.mName + "_Desc";
                tData.mIcon = baseItem.Icon;
                tData.mItemCount = medic.Count;
                mItemDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}

