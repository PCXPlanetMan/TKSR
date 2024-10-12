using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using YooAsset;

namespace TKSR
{
    public enum EnumItemType
    {
        Invalid = 0,
        Medic,
        Prop,
        Weapon,
        Armor,
        Accessory,
        Special,
        LIMIT
    }
    
    public class ItemsController : MonoBehaviour
    {
        private GameObject charTemplatePrefab;


        public static ItemsController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindFirstObjectByType<ItemsController>();

                if (instance != null)
                    return instance;

                Create();

                return instance;
            }
        }


        protected static ItemsController instance;

        public static ItemsController Create()
        {
            GameObject manager = new GameObject("ItemsController");
            instance = manager.AddComponent<ItemsController>();

            return instance;
        }


        void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private readonly List<AssetHandle> m_requests = new List<AssetHandle>();
        private SchemaItems m_schemaItems;
        
        public IEnumerator LoadItemsSchemaAsync()
        {
            AssetHandle textAssetHandleItems = YooAssets.LoadAssetAsync<TextAsset>(ResourceUtils.AB_SCHEMA_ITEMS);
            yield return textAssetHandleItems;

            if (textAssetHandleItems.Status == EOperationStatus.Succeed)
            {
                var jsonPropsSchema = textAssetHandleItems.AssetObject as TextAsset;
                if (jsonPropsSchema != null)
                {
                    try
                    {
                        m_schemaItems = JsonConvert.DeserializeObject<SchemaItems>(jsonPropsSchema.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[TKSR] Read Items Schema failed with exception = {e.ToString()}");
                    }
                }

                ParseAllItemsFromSchema();
            
                textAssetHandleItems.Release();
            }
            else
            {
                Debug.LogError($"[TKSR] YooAssets Load Text Failed : {textAssetHandleItems.LastError}");
            }
        }

        public void UnloadAllAssets()
        {
            foreach (var request in m_requests) request.Release();
            m_requests.Clear();
        }

        private Dictionary<int, BaseItems> m_dictItemLookups = new Dictionary<int, BaseItems>();
        private Dictionary<int, ItemMedicConfig> m_dictMedics = new Dictionary<int, ItemMedicConfig>();
        private Dictionary<int, ItemPropConfig> m_dictProps = new Dictionary<int, ItemPropConfig>();
        private Dictionary<int, ItemWeaponConfig> m_dictWeapons = new Dictionary<int, ItemWeaponConfig>();
        private Dictionary<int, ItemArmorConfig> m_dictArmors = new Dictionary<int, ItemArmorConfig>();
        private Dictionary<int, ItemAccessoryConfig> m_dictAccessories = new Dictionary<int, ItemAccessoryConfig>();
        private Dictionary<int, ItemSpecialConfig> m_dictSpecials = new Dictionary<int, ItemSpecialConfig>();

        public Dictionary<int, BaseItems> dictAllItems
        {
            get => m_dictItemLookups;
            private set => m_dictItemLookups = value;
        }

        private void ParseAllItemsFromSchema()
        {
            if (m_schemaItems == null)
            {
                return;
            }

            foreach (var baseItem in m_schemaItems.BaseItems)
            {
                m_dictItemLookups.Add(baseItem.Id, baseItem);
            }

            foreach (var medicItem in m_schemaItems.Medics)
            {
                int id = medicItem.Id;
                var baseData = m_schemaItems.BaseItems.Find(x => x.Id == id);
                var medicData = new ItemMedicConfig()
                {
                    Id = id,
                    Authority = baseData.Authority,
                    I2Name = baseData.I2Name,
                    Icon = baseData.Icon,
                    Price = baseData.Price,
                    MedicData = medicItem
                };
                m_dictMedics.Add(id, medicData);
            }
            
            foreach (var propItem in m_schemaItems.Props)
            {
                int id = propItem.Id;
                var baseData = m_schemaItems.BaseItems.Find(x => x.Id == id);
                var propData = new ItemPropConfig()
                {
                    Id = id,
                    Authority = baseData.Authority,
                    I2Name = baseData.I2Name,
                    Icon = baseData.Icon,
                    Price = baseData.Price,
                    PropData = propItem
                };
                m_dictProps.Add(id, propData);
            }
            
            foreach (var weaponItem in m_schemaItems.Weapons)
            {
                int id = weaponItem.Id;
                var baseData = m_schemaItems.BaseItems.Find(x => x.Id == id);
                var weaponData = new ItemWeaponConfig()
                {
                    Id = id,
                    Authority = baseData.Authority,
                    I2Name = baseData.I2Name,
                    Icon = baseData.Icon,
                    Price = baseData.Price,
                    WeaponData = weaponItem
                };
                m_dictWeapons.Add(id, weaponData);
            }
            
            foreach (var armorItem in m_schemaItems.Armors)
            {
                int id = armorItem.Id;
                var baseData = m_schemaItems.BaseItems.Find(x => x.Id == id);
                var armorData = new ItemArmorConfig()
                {
                    Id = id,
                    Authority = baseData.Authority,
                    I2Name = baseData.I2Name,
                    Icon = baseData.Icon,
                    Price = baseData.Price,
                    ArmorData = armorItem
                };
                m_dictArmors.Add(id, armorData);
            }
            
            foreach (var accessoryItem in m_schemaItems.Accessories)
            {
                int id = accessoryItem.Id;
                var baseData = m_schemaItems.BaseItems.Find(x => x.Id == id);
                var accessoryData = new ItemAccessoryConfig()
                {
                    Id = id,
                    Authority = baseData.Authority,
                    I2Name = baseData.I2Name,
                    Icon = baseData.Icon,
                    Price = baseData.Price,
                    AccessoryData = accessoryItem
                };
                m_dictAccessories.Add(id, accessoryData);
            }
            
            foreach (var specialItem in m_schemaItems.Specials)
            {
                int id = specialItem.Id;
                var baseData = m_schemaItems.BaseItems.Find(x => x.Id == id);
                var specialData = new ItemSpecialConfig()
                {
                    Id = id,
                    Authority = baseData.Authority,
                    I2Name = baseData.I2Name,
                    Icon = baseData.Icon,
                    Price = baseData.Price,
                    SpecialData = specialItem
                };
                m_dictSpecials.Add(id, specialData);
            }
        }

        public EnumItemType LookupItemType(int itemId, out BaseItems baseItem)
        {
            baseItem = null;
            if (!m_dictItemLookups.ContainsKey(itemId))
            {
                return EnumItemType.Invalid;
            }
            
            bool result = m_dictItemLookups.TryGetValue(itemId, out baseItem);
            EnumItemType type = EnumItemType.Invalid;
            if (result)
            {
                type = (EnumItemType)baseItem.Type;
            }
            return type;
        }

        public string ParseItemI2LocalName(int itemId)
        {
            string strLoc = null;
            BaseItems baseItem;
            var enumType = LookupItemType(itemId, out baseItem);
            switch (enumType)
            {
                case EnumItemType.Medic:
                {
                    strLoc = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_MEDICS + baseItem.I2Name;
                }
                    break;
                case EnumItemType.Prop:
                {
                    strLoc = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_PROPS + baseItem.I2Name;
                }
                    break;
                case EnumItemType.Weapon:
                {
                    strLoc = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_WEAPONS + baseItem.I2Name;
                }
                    break;
                case EnumItemType.Armor:
                {
                    strLoc = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_ARMORS + baseItem.I2Name;
                }
                    break;
                case EnumItemType.Accessory:
                {
                    strLoc = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_ACCESSORIES + baseItem.I2Name;
                }
                    break;
                case EnumItemType.Special:
                {
                    strLoc = ResourceUtils.I2FORMAT_ITEMS_CATEGORY_SPECIALS + baseItem.I2Name;
                }
                    break;
            }

            return strLoc;
        }

        public Sprite LoadItemIconSpriteById(string strSpriteName)
        {
            var assetHandle = YooAssets.LoadAssetSync<Sprite>(strSpriteName);
            var targetSprite = assetHandle.AssetObject as Sprite;
            return targetSprite;
        }
    }
}