using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TacticalRPG;
using UnityEngine;
using YooAsset;

namespace TKSR
{

    public class CharactersController : MonoBehaviour
    {

        // private SchemaStateMachine schemaStateMachine;
        private GameObject charTemplatePrefab;


        public static CharactersController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindFirstObjectByType<CharactersController>();

                if (instance != null)
                    return instance;

                Create();

                return instance;
            }
        }
        
        protected static CharactersController instance;

        public static CharactersController Create()
        {
            GameObject manager = new GameObject("CharactersController");
            instance = manager.AddComponent<CharactersController>();

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
        private SchemaCharacters m_schemaCharacters;
        
        public IEnumerator LoadCharactersSchemaAsync()
        {
            AssetHandle textAssetHandleCharacters = YooAssets.LoadAssetAsync<TextAsset>(ResourceUtils.AB_SCHEMA_CHARACTERS);
            m_requests.Add(textAssetHandleCharacters);
            yield return textAssetHandleCharacters;

            var jsonPropsSchema = textAssetHandleCharacters.AssetObject as TextAsset;
            if (jsonPropsSchema != null)
                m_schemaCharacters = JsonConvert.DeserializeObject<SchemaCharacters>(jsonPropsSchema.text);
        }

        public void UnloadAllAssets()
        {
            m_dictCharacterPortrait.Clear();
            
            foreach (var request in m_requests) request.Release();
            m_requests.Clear();
        }

        
        private Dictionary<string, Sprite> m_dictCharacterPortrait = new Dictionary<string, Sprite>();
        
        /// <summary>
        /// 直接使用sprite文件名字获取Sprite
        /// </summary>
        /// <param name="portraitName"></param>
        /// <returns></returns>
        private Sprite LoadPortraitSprite(string portraitName)
        {
            var assetHandle = YooAssets.LoadAssetSync<Sprite>(portraitName);
            m_requests.Add(assetHandle);
            
            var charSprite = assetHandle.AssetObject as Sprite;
            return charSprite;
        }
        
        /// <summary>
        /// 从配置中读取头像参数进行加载
        /// </summary>
        /// <param name="strChar"></param>
        /// <returns></returns>
        public Sprite LoadCharacterPortraitSprite(string strChar)
        {
            if (m_dictCharacterPortrait.ContainsKey(strChar))
            {
                return m_dictCharacterPortrait[strChar];
            }

            if (m_schemaCharacters.CharacterParam == null || m_schemaCharacters.CharacterParam.Count == 0)
            {
                return null;
            }

            var foundChar = m_schemaCharacters.CharacterParam.Find(x => x.CharName.CompareTo(strChar) == 0);
            if (foundChar != null)
            {
                var charSprite = LoadPortraitSprite(foundChar.Portrait);
                m_dictCharacterPortrait.Add(strChar, charSprite);
                return charSprite;
            }

            return null;
        }

        // 不同的角色可能拥有相同的job,所以需要记录对应角色的基础信息,方便查询
        private Dictionary<string, CharacterConfigItem> m_dictCharConfigs =
            new Dictionary<string, CharacterConfigItem>();

        public CharacterConfigItem FetchCharConfigItemByName(string strChar)
        {
            CharacterConfigItem item = null;
            if (m_dictCharConfigs.ContainsKey(strChar))
            {
                item = m_dictCharConfigs[strChar];
            }
            else
            {
                AssetHandle handle = YooAssets.LoadAssetSync<UnitRecipe>(strChar);
                UnitRecipe recipe = handle.AssetObject as UnitRecipe;
                if (recipe != null)
                {
                    var foundStartingStat = m_schemaCharacters.JobStartingStats.Find(x => x.Name.CompareTo(recipe.job) == 0);
                    var foundGrowthStat = m_schemaCharacters.JobGrowthStats.Find(x => x.Name.CompareTo(recipe.job) == 0);

                    item = new CharacterConfigItem()
                    {
                        StartingStat = foundStartingStat,
                        GrowthStat = foundGrowthStat
                    };
                    m_dictCharConfigs.Add(strChar, item);
                }
                else
                {
                    Debug.LogError($"[TKSR] Load {strChar} UnitRecipe failed.");
                    return null;
                }
                handle.Release();
            }

            return item;
        }

        public LevelUpData CalcCharDataByLevel(string strChar, int targetLv)
        {
            LevelUpData targetLevel = null;
            var configItem = FetchCharConfigItemByName(strChar);
            if (configItem != null)
            {
                var levelUpdateData = CharacterDataUtils.CalcCharLevelUpConfigData(targetLv, configItem.StartingStat, configItem.GrowthStat);
                if (levelUpdateData != null)
                {
                    targetLevel = levelUpdateData;
                }
            }
            
            return targetLevel;
        }

        public CharacterParam FetchCharPropByName(string strChar)
        {
            var foundParam = m_schemaCharacters.CharacterParam.Find(x => x.CharName.CompareTo(strChar) == 0);
            return foundParam;
        }
    }
}