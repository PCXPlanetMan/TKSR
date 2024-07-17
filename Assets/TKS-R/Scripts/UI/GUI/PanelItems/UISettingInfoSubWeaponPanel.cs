using UnityEngine;

namespace TKSR
{
    public class UISettingInfoSubWeaponPanel : IItemDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            if (document.Weapons == null)
            {
                Debug.LogWarning($"[TKSR] There is no Weapons inited.");
                return;
            }

            if (document.Weapons.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Weapons Count is Zero.");
                return;
            }
            m_totalDataCount = document.Weapons.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoItemData tData = new InfoItemData();
                var weapon = document.Weapons[i];
                tData.mId = weapon.ItemId;
                BaseItems baseItem;
                var type = ItemsController.Instance.LookupItemType(weapon.ItemId, out baseItem);
                tData.mName = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_WEAPONS + baseItem.I2Name;
                tData.mDesc = tData.mName + "_Desc";
                tData.mIcon = baseItem.Icon;
                tData.mItemCount = weapon.Count;
                mItemDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}

