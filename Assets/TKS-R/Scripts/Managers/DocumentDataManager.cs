using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using I2.Loc;
using Newtonsoft.Json;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace TKSR
{
    [Serializable]
    public class TransportItemData
    {
        public int itemId;
        public int itemAmount;
        public float toastShowDuration = 0f;
    }
    
    [Serializable]
    public class TransportThings
    {
        public List<TransportItemData> datas;
        public int goldAmount;
        public float goldToastShowDuration = 0f;

        public TransportThings()
        {
            datas = new List<TransportItemData>();
        }
    }
    
    public class DocumentDataManager : MonoBehaviour
    {
        [HideInInspector]
        public bool isABConfigLoaded = false;
        
        private readonly int MAX_DOCUMENTS_CNT = 10;
        private readonly int MIN_DOCUMENT_INDEX = 1;
        private readonly int MAX_TEAM_NUMBER = 5; // 队伍内的成员最大数目
        private readonly int MAX_ITEM_COUNT = 99; // 道具最大上限

        public static DocumentDataManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindFirstObjectByType<DocumentDataManager>();

                if (instance != null)
                    return instance;

                Create();

                return instance;
            }
        }


        protected static DocumentDataManager instance;

        public static DocumentDataManager Create()
        {
            GameObject document = new GameObject("DocumentDataManager");
            instance = document.AddComponent<DocumentDataManager>();

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

        private IEnumerator Start()
        {
            yield return InitializeYooAsset();

            yield return LoadingSchemasAsync();

            isABConfigLoaded = true;

            NewDefaultDocument();
        }

        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        
        private IEnumerator InitializeYooAsset()
        {
            GameManager.Instance.Behaviour = this;

            // 初始化事件系统
            UniEvent.Initalize();

            // 初始化资源系统
            YooAssets.Initialize();

            // 加载更新页面
            // var go = Resources.Load<GameObject>("PatchWindow");
            // GameObject.Instantiate(go);

            // 开始补丁更新流程
            PatchOperation operation = new PatchOperation(ResourceUtils.AB_YOO_PACKAGE, EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), PlayMode);
            YooAssets.StartOperation(operation);
            yield return operation;
            
            if (operation.Status == EOperationStatus.Succeed)
                Debug.Log("[TKSR] YooAssets Load Patch Succeed!");
            else 
                Debug.LogError($"[TKSR] YooAssets Update Patch Failed：{operation.Error}");

            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage(ResourceUtils.AB_YOO_PACKAGE);
            YooAssets.SetDefaultPackage(gamePackage);
        }
        
        private IEnumerator LoadingSchemasAsync()
        {
            yield return CharactersController.Instance.LoadCharactersSchemaAsync();

            yield return ItemsController.Instance.LoadItemsSchemaAsync();
        }
        
        private TKSArchives _archives;

        public TKSArchives LoadTKSArchives()
        {
            _archives = null;

            // %userprofile%\AppData\LocalLow\
            var dataPath = Path.Combine(Application.persistentDataPath, ResourceUtils.DOCUMENT_ARCHIVES_FILE_NAME);
            if (File.Exists(dataPath))
            {
                string dataAsJson = File.ReadAllText(dataPath);
                _archives = JsonConvert.DeserializeObject<TKSArchives>(dataAsJson);
            }

            return _archives;
        }

        private void SaveTKSArchives()
        {
            if (_archives != null)
            {
                var dataPath = Path.Combine(Application.persistentDataPath, ResourceUtils.DOCUMENT_ARCHIVES_FILE_NAME);
                if (File.Exists(dataPath))
                {
                    File.Delete(dataPath);
                }

                string dataAsJson = JsonConvert.SerializeObject(_archives, Formatting.Indented);
                File.WriteAllText(dataPath, dataAsJson);
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="docIndex">1->9</param>
        public bool LoadGameDocument(int docIndex)
        {
            var archives = DocumentDataManager.Instance.LoadTKSArchives();
            if (archives == null || archives.Documents == null || archives.Documents.Count == 0)
            {
                Debug.LogWarning("No document file existed");
                return false;
            }
            else
            {
                GameDocument curDefaultDocument = null;
                if (docIndex >= MIN_DOCUMENT_INDEX && docIndex < archives.Documents.Count && docIndex < MAX_DOCUMENTS_CNT)
                {
                    curDefaultDocument = archives.Documents[docIndex];
                }

                if (curDefaultDocument == null)
                {
                    Debug.LogWarningFormat("There is no saved file in current archives by index = {0}", docIndex);
                    return false;
                }

                _gameDocument = null;

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, curDefaultDocument);
                    stream.Seek(0, SeekOrigin.Begin);
                    _gameDocument = (GameDocument)formatter.Deserialize(stream);
                }

                LoadGameQuestItemsListState();
                return true;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="docIndex">1->9</param>
        public void SaveGameDocument(int docIndex)
        {
            LoadTKSArchives();
            if (_archives == null)
            {
                _archives = new TKSArchives();
            }

            GameDocument curDefaultDocument = GetCurrentDocument();
            if (docIndex >= MIN_DOCUMENT_INDEX && docIndex < MAX_DOCUMENTS_CNT)
            {
                if (_archives.Documents == null || _archives.Documents.Count == 0)
                {
                    _archives.Documents = new List<GameDocument>();
                    for (int i = 0; i < MAX_DOCUMENTS_CNT; i++)
                    {
                        var document = new GameDocument();
                        document.DocumentId = i;
                        _archives.Documents.Add(document);
                    }
                }

                _archives.Documents.RemoveAt(0);
                curDefaultDocument.Timestamp = DateTime.Now.Ticks;
                var playerInstance = PlayerCharacter.PlayerInstance;
                if (playerInstance != null)
                {
                    var position = playerInstance.Controller2D.Rigidbody2D.position;
                    curDefaultDocument.PosX = position.x;
                    curDefaultDocument.PosY = position.y;
                    curDefaultDocument.Direction = playerInstance.GetCurrentFaceDirectionByAnimation().ToString();
                }

                var curScene = SceneManager.GetActiveScene();
                curDefaultDocument.SceneName = curScene.name;
                _archives.Documents.Insert(0, curDefaultDocument);

                _archives.Documents.RemoveAt(docIndex);
                GameDocument newDocument = null;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, curDefaultDocument);
                    stream.Seek(0, SeekOrigin.Begin);
                    newDocument = (GameDocument)formatter.Deserialize(stream);
                }

                newDocument.DocumentId = docIndex;
                _archives.Documents.Insert(docIndex, newDocument);

                SaveTKSArchives();
            }
            else
            {
                Debug.LogErrorFormat("Save Document in error index : {0}", docIndex);
                _archives = null;
            }
        }

        private GameDocument _gameDocument;

        /// <summary>
        /// 新建游戏存档(初始化),一般是开启新游戏时创建,必须要在YooAssets初始化并加载其他配置后才创建
        /// </summary>
        /// <returns></returns>
        public GameDocument NewDefaultDocument()
        {
            if (_gameDocument != null)
                _gameDocument = null;

            _gameDocument = new GameDocument()
            {
                DocumentId = 0,
                SceneName = null,
                PosX = 0f,
                PosY = 0f,
                Gold = 0,
                Morality = 0,
                Intelligence = 0,
                Courage = 0,
                MedicalSkill = 0,
                GameQuests = new List<GameQuestState>(),
                I2StoryNotes = new List<string>(),
                Timestamp = DateTime.Now.Ticks,
                MainRoleInfo = null,
                TaskCharOwnItems = new List<GameTaskCharOwnItem>()
            };
            
            // 解析Lv=1的主角状态和队伍信息
            _gameDocument.MainRoleInfo = GenGameCharInfoFromSchema(ResourceUtils.MAINROLE_NAME, 1);
            var maincharPropItem = CharactersController.Instance.FetchCharPropByName(ResourceUtils.MAINROLE_NAME);
            _gameDocument.Intelligence = (uint)maincharPropItem.INT_V;
            _gameDocument.Morality = (uint)maincharPropItem.MOL_V;
            _gameDocument.Courage = (uint)maincharPropItem.CRG_V;
            _gameDocument.MedicalSkill = (uint)maincharPropItem.MED_V;
            SyncDocumentTalentsToDlg();
            SyncDocumentGoldToDlg();

            _gameDocument.Medics = new List<GameItemInfo>();
            _gameDocument.Props = new List<GameItemInfo>();
            _gameDocument.Weapons = new List<GameItemInfo>();
            _gameDocument.Armors = new List<GameItemInfo>();
            _gameDocument.Accessories = new List<GameItemInfo>();
            _gameDocument.Specials = new List<GameItemInfo>();

            
            return _gameDocument;
        }

        private void SyncDocumentTalentsToDlg()
        {
            DialogueLua.SetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_MAXINTEL, _gameDocument.Intelligence);
            DialogueLua.SetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_MAXMOR, _gameDocument.Morality);
            DialogueLua.SetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_MAXCOR, _gameDocument.Courage);
        }
        
        private void SyncDocumentGoldToDlg()
        {
            DialogueLua.SetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_GOLDEN, _gameDocument.Gold);
        }
        
        private GameCharDisplayInfo GenGameCharInfoFromSchema(string charName, int level)
        {
            GameCharDisplayInfo info = new GameCharDisplayInfo();
            info.CharName = charName;
            info.Level = level;
            info.UsedItems = new List<GameItemInfo>();
            info.SkillsTree = new List<GameSkillInfo>();

            var levelData = CharactersController.Instance.CalcCharDataByLevel(charName, level);
            info.MaxHP = levelData.MHP;
            info.MaxMP = levelData.MMP;
            info.HP = info.MaxHP;
            info.MP = info.MaxMP;

            info.AttackValue = levelData.ATK;
            info.DefenseValue = levelData.DEF;
            info.HitValue = levelData.HIT;
            info.EvadeValue = levelData.EVD;
            info.SpeedValue = levelData.SPD;
            
            var charPropItem = CharactersController.Instance.FetchCharPropByName(charName);
            info.LuckValue = charPropItem.LUCK;
            info.UnderstandValue = charPropItem.USD;
            
            var charConfigItem = CharactersController.Instance.FetchCharConfigItemByName(charName);
            info.MovementValue = charConfigItem.StartingStat.MOV;
            
            // TODO:根据当前的装备状况更新数值
            
            return info;
        }

        public void LevelupMainPlayer(int newLevel)
        {
            var charInfo = GenGameCharInfoFromSchema(ResourceUtils.MAINROLE_NAME, newLevel);
            _gameDocument.MainRoleInfo = charInfo;

            // TODO:专门用于激活武陵城黄盖/吕蒙任务
            SpecialCheckWuLingQuest();
        }

        private void SpecialCheckWuLingQuest()
        {
            if (_gameDocument.MainRoleInfo.Level >= 10)
            {
                string strWuLingQuest = "Wu Return : HuangGai";
                var questState = QuestLog.GetQuestState(strWuLingQuest);
                if ((questState & QuestState.Unassigned) == QuestState.Unassigned)
                {
                    QuestLog.SetQuestState(strWuLingQuest, QuestState.Active);
                    Debug.Log("[TKSR] MainPlayer level up greater than 10, Make WuLing City HuangGai Quest active.");
                }
                strWuLingQuest = "Wu Return : LvMeng";
                questState = QuestLog.GetQuestState(strWuLingQuest);
                if ((questState & QuestState.Unassigned) == QuestState.Unassigned)
                {
                    QuestLog.SetQuestState(strWuLingQuest, QuestState.Active);
                    Debug.Log("[TKSR] MainPlayer level up greater than 10, Make WuLing City LvMeng Quest active.");
                }
            }
            
        }

        public GameDocument GetCurrentDocument()
        {
            if (_gameDocument == null)
            {
                return NewDefaultDocument();
            }

            return _gameDocument;
        }

        /// <summary>
        /// 在存档中更新记录Quest状态
        /// </summary>
        /// <param name="questName"></param>
        /// <param name="stateInt"></param>
        public void SaveGameQuestItemState(string questName, QuestState state)
        {
            if (_gameDocument == null)
            {
                Debug.LogError("[TKSR] Empty game document.");
                return;
            }

            int stateInt = (int)state;
            var foundQuestItem = _gameDocument.GameQuests.Find(x => x.QuestName.CompareTo(questName) == 0);
            if (foundQuestItem != null)
            {
                foundQuestItem.StateInt = stateInt;
            }
            else
            {
                _gameDocument.GameQuests.Add(new GameQuestState()
                {
                    QuestName = questName,
                    StateInt = stateInt
                });
            }
        }
        
        public void SaveGameQuestEntryItemState(string questName, int questEntryId, QuestState entryState)
        {
            if (_gameDocument == null)
            {
                Debug.LogError("[TKSR] Empty game document.");
                return;
            }

            int entryStateInt = (int)entryState;
            var foundQuestItem = _gameDocument.GameQuests.Find(x => x.QuestName.CompareTo(questName) == 0);
            if (foundQuestItem == null)
            {
                Debug.LogError($"[TKSR] Can't find quest in document : {questName}");
            }
            else
            {
                var questState = (QuestState)foundQuestItem.StateInt;
                if (questState == QuestState.Unassigned)
                {
                    Debug.LogWarning($"[TKSR] Found quest : {questName} is Unassigned");
                }

                if (foundQuestItem.QuestEntryStates == null)
                {
                    foundQuestItem.QuestEntryStates = new List<GameQuestEntryState>();
                }

                var foundQuestEntryItem = foundQuestItem.QuestEntryStates.Find(x => x.QuestEntryId == questEntryId);
                if (foundQuestEntryItem != null)
                {
                    foundQuestEntryItem.EntryStateInt = entryStateInt;
                }
                else
                {
                    foundQuestItem.QuestEntryStates.Add(new GameQuestEntryState()
                    {
                        QuestEntryId = questEntryId,
                        EntryStateInt = entryStateInt
                    });
                }
            }
        }
        
        /// <summary>
        /// 从Dialogue组件数据库中读取Quest数据并保存在游戏存档中
        /// </summary>
        private void LoadGameQuestItemsListState()
        {
            if (_gameDocument != null)
            {
                var questsList = _gameDocument.GameQuests;
                if (questsList != null && questsList.Count > 0)
                {
                    foreach (var quest in questsList)
                    {
                        QuestState questState = (QuestState)quest.StateInt;
                        QuestLog.SetQuestState(quest.QuestName, questState);

                        if (quest.QuestEntryStates != null && quest.QuestEntryStates.Count > 0)
                        {
                            foreach (var entry in quest.QuestEntryStates)
                            {
                                QuestLog.SetQuestEntryState(quest.QuestName, entry.QuestEntryId, (QuestState)entry.EntryStateInt);
                            }
                        }
                    }
                }

                TransportMainRoleNameToDialogueDatabase();
            }
        }
        

        public void UpdateMainRoleName(string strFirstName, string strLastName)
        {
            if (_gameDocument != null)
            {
                _gameDocument.FirstName = strFirstName;
                _gameDocument.LastName = strLastName;

                TransportMainRoleNameToDialogueDatabase();
            }
        }

        /// <summary>
        /// 同步主角名字从存档到Dialogue数据库中
        /// </summary>
        private void TransportMainRoleNameToDialogueDatabase()
        {
            if (!string.IsNullOrEmpty(_gameDocument.FirstName))
            {
                string s = DialogueLua.GetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_FIRST_NAME).asString;
                DialogueLua.SetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_FIRST_NAME, _gameDocument.FirstName);
                Debug.Log($"[TKSR] Document read Dialogue first name = {s} and save {_gameDocument.FirstName} to Dialogue database");
            }
            if (!string.IsNullOrEmpty(_gameDocument.LastName))
            {
                string s = DialogueLua.GetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_LAST_NAME).asString;
                DialogueLua.SetVariable(ResourceUtils.DIALOGUE_VAR_FORMAT_LAST_NAME, _gameDocument.LastName);
                Debug.Log($"[TKSR] Document read Dialogue last name = {s} and save {_gameDocument.LastName} to Dialogue database");
            }
            
            DialogueManager.ChangeActorName(ResourceUtils.MAINROLE_NAME, GetMainPlayerFullName());
        }

        public string GetMainPlayerFullName()
        {
            if (_gameDocument == null)
                return null;

            if (string.IsNullOrEmpty(_gameDocument.FirstName) || string.IsNullOrEmpty(_gameDocument.LastName))
                return null;

            return _gameDocument.LastName + _gameDocument.FirstName;
        }

        /// <summary>
        /// 剧情播放过程中获取物品和其他数据
        /// </summary>
        /// <param name="itemsList">id:count:duration,id:count:duration,...,[GOLD]:count:duration 若某个元素如果是GOLD,则说明是金钱</param>
        public void TransportDataByTimeline(string itemsList)
        {
            if (_gameDocument == null)
            {
                Debug.LogError("[TKSR] There is no Game Document Inited yet.");
                return;
            }
            
            TransportThings things = new TransportThings();
            var items = itemsList.Split(',');
            for (int i = 0; i < items.Length; i++)
            {
                if (string.IsNullOrEmpty(items[i]))
                {
                    break;
                }
                var values = items[i].Split(':');
                var itemKey = values[0];
                string itemValue = null;
                if (values.Length > 1)
                {
                    itemValue = values[1];
                }

                float fDuration = 0f;
                if (values.Length > 2)
                {
                    fDuration = float.Parse(values[2]);
                }
                
                if (itemKey.CompareTo(ResourceUtils.CONFIG_GOLD) == 0)
                {
                    if (string.IsNullOrEmpty(itemValue))
                    {
                        break;
                    }
                    int goldCount = int.Parse(itemValue);
                    things.goldAmount = goldCount;
                    things.goldToastShowDuration = fDuration;
                    
                    UpdateGameMoneyCount(things.goldAmount, things.goldToastShowDuration);
                }
                else
                {
                    int itemID = int.Parse(itemKey);
                    int itemCount = 0;
                    if (string.IsNullOrEmpty(itemValue))
                    {
                        itemCount = 1;
                    }
                    else
                    {
                        itemCount = int.Parse(itemValue);
                    }

                    var itemData = new TransportItemData()
                    {
                        itemId = itemID,
                        itemAmount = itemCount,
                        toastShowDuration = fDuration
                    };
                    
                    UpdateGameItemCount(itemData.itemId, itemData.itemAmount, itemData.toastShowDuration);
                    
                    things.datas.Add(itemData);
                }
            }
        }

        public void UpdateGameMoneyCount(int goldAmount, float showDuration = 0f)
        {
            Debug.Log($"[TKSR] Before Gold Value = {_gameDocument.Gold}, Try to update with {goldAmount}");
            if (goldAmount > 0)
            {
                _gameDocument.Gold += (uint)goldAmount;
            }
            else
            {
                _gameDocument.Gold = (uint)(_gameDocument.Gold - goldAmount);
            }
            Debug.Log($"[TKSR] After Gold Value = {_gameDocument.Gold}");
            _gameDocument.Gold = Math.Clamp(_gameDocument.Gold, 0, ResourceUtils.MAX_GOLD_COUNT);
            Debug.Log($"[TKSR] Finally Fixed Gold Value = {_gameDocument.Gold}");

            bool showToast = showDuration > 0f ? true : false;
            if (showToast)
            {
                GameUI.Instance.toastPanel.ShowToastForMoney(goldAmount, showDuration);
            }
        }

        public void UpdateGameItemCount(int itemId, int itemAmount, float showDuration = 0f)
        {
            BaseItems baseItem = null;
            EnumItemType itemType = ItemsController.Instance.LookupItemType(itemId, out baseItem);
            Debug.Log($"[TKSR] Update Game count with item id = {itemId} type = {itemType}");
            List<GameItemInfo> targetItemsList = null; 
            switch (itemType)
            {
                case EnumItemType.Medic:
                {
                    targetItemsList = _gameDocument.Medics;
                }
                    break;
                case EnumItemType.Prop:
                {
                    targetItemsList = _gameDocument.Props;
                }
                    break;
                case EnumItemType.Weapon:
                {
                    targetItemsList = _gameDocument.Weapons;
                }
                    break;
                case EnumItemType.Armor:
                {
                    targetItemsList = _gameDocument.Armors;
                }
                    break;
                case EnumItemType.Accessory:
                {
                    targetItemsList = _gameDocument.Accessories;
                }
                    break;
                case EnumItemType.Special:
                {
                    targetItemsList = _gameDocument.Specials;
                }
                    break;
            }

            if (targetItemsList != null)
            {
                var foundItemInfo = targetItemsList.Find(x => x.ItemId == itemId);
                if (foundItemInfo == null)
                {
                    foundItemInfo = new GameItemInfo();
                    foundItemInfo.ItemId = itemId;
                    foundItemInfo.Count = 0;
                    targetItemsList.Add(foundItemInfo);
                }
                Debug.Log($"[TKSR] Before Item Count = {foundItemInfo.Count}, Try to update with {itemAmount}");
                foundItemInfo.Count += itemAmount;
                Debug.Log($"[TKSR] After Item Count = {foundItemInfo.Count}");
                foundItemInfo.Count = Math.Clamp(foundItemInfo.Count, 0, (int)ResourceUtils.MAX_ITEM_COUNT);
                Debug.Log($"[TKSR] Finally Fixed Item Count = {foundItemInfo.Count}");

                if (!string.IsNullOrEmpty(baseItem.I2DVariable))
                {
                    DialogueLua.SetVariable(baseItem.I2DVariable, foundItemInfo.Count);
                }
            }

            bool showToast = showDuration > 0f ? true : false;
            if (showToast)
            {
                GameUI.Instance.toastPanel.ShowToastForItem(itemId, itemAmount, showDuration);
            }
        }

        public void RecordNoteI2(string strNoteI2)
        {
            if (_gameDocument != null)
            {
                if (_gameDocument.I2StoryNotes == null)
                {
                    _gameDocument.I2StoryNotes = new List<string>();
                }

                var existedNotes = _gameDocument.I2StoryNotes;
                var foundExist = existedNotes.Find(x => x.CompareTo(strNoteI2) == 0);
                if (!string.IsNullOrEmpty(foundExist))
                {
                    Debug.LogWarning($"[TKSR] Note item is already existed in Document: {strNoteI2}");
                }
                else
                {
                    existedNotes.Add(strNoteI2);
                }
                Debug.Log($"[TKSR] Save a note by I2 type = {strNoteI2}");
            }
        }
        
        
        #if TKSR_DEV && UNITY_EDITOR
        public void DebugUpdateMainPlayerLevel(int newLevel)
        {
            LevelupMainPlayer(newLevel);
        }

        public void DebugAddAllSchemaItems()
        {
            int defaultCount = 99;
            var allItems = ItemsController.Instance.dictAllItems;
            foreach (var kv in allItems)
            {
                int itemId = kv.Key;
                UpdateGameItemCount(itemId, defaultCount);
                
            }
        }
        
        public void DebugAddAllDialogueNotes()
        {
            //var allCategories =  LocalizationManager.GetCategories();
            
            var allNotes = LocalizationManager.GetTermsList("UI/SettingInfo/Team/Notes");
            foreach (var note in allNotes)
            {
                var newNote = note.Replace(ResourceUtils.I2FORMAT_TEAM_CATEGORY_NOTES, "");
                RecordNoteI2(newNote + ",1");
            }
        }
        
        public void DebugUpdateDocumentGolden(int newValue)
        {
            _gameDocument.Gold = (uint)newValue;
            SyncDocumentGoldToDlg();
        }
        
        public void DebugUpdateDocumentIntelligence(int newValue)
        {
            _gameDocument.Intelligence = (uint)newValue;
            SyncDocumentTalentsToDlg();
        }
        
        public void DebugUpdateDocumentMorality(int newValue)
        {
            _gameDocument.Morality = (uint)newValue;
            SyncDocumentTalentsToDlg();
        }
        
        public void DebugUpdateDocumentCourage(int newValue)
        {
            _gameDocument.Courage = (uint)newValue;
            SyncDocumentTalentsToDlg();
        }
        
        public void DebugAddItemToPackage(int itemID, int itemCount)
        {
            UpdateGameItemCount(itemID, itemCount);
        }

        public void DebugUpdateTaskRatio(float ratio)
        {
            m_debugCompletedTaskRatio = ratio;
        }
#endif

        private GameCharDisplayInfo FindCharInfoFromCandidates(string charName)
        {
            var gameDocument = GetCurrentDocument();
            if (charName == ResourceUtils.MAINROLE_NAME)
            {
                return gameDocument.MainRoleInfo;
            }
            else
            {
                if (gameDocument.Candidates != null)
                    return gameDocument.Candidates.Find(x => x.CharName == charName);
            }

            return null;
        }

        public GameCharDisplayInfo[] FindCharsInfoInTeam()
        {
            var gameDocument = GetCurrentDocument();
            List<GameCharDisplayInfo> chars = new List<GameCharDisplayInfo>();
            chars.Add(gameDocument.MainRoleInfo);
            if (gameDocument.Team != null)
            {
                for (int i = 0; i < gameDocument.Team.Count; i++)
                {
                    string id = gameDocument.Team[i];
                    GameCharDisplayInfo charInfo = FindCharInfoFromCandidates(id);
                    chars.Add(charInfo);
                }
            }

            return chars.ToArray();
        }

        public GameCharDisplayInfo FindCharInfoInTeam(string charName)
        {
            var gameDocument = GetCurrentDocument();
            if (charName == ResourceUtils.MAINROLE_NAME)
            {
                return gameDocument.MainRoleInfo;
            }
            else
            {
                for (int i = 0; i < gameDocument.Team.Count; i++)
                {
                    if (gameDocument.Team[i] == charName)
                    {
                        GameCharDisplayInfo charInfo = FindCharInfoFromCandidates(charName);
                        return charInfo;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 直接在队伍中创建一个等级为charLevel的角色(一般用于调试)
        /// </summary>
        /// <param name="charID"></param>
        /// <param name="charLevel"></param>
        public void CreateCharToTeam(string charName, int charLevel)
        {
            var gameDocument = GetCurrentDocument();
            GameCharDisplayInfo foundCharInfo = null;
            if (charName == ResourceUtils.MAINROLE_NAME)
            {
                Debug.LogWarning("MainRole can't be created by function");
                gameDocument.MainRoleInfo = GenGameCharInfoFromSchema(charName, charLevel);
                foundCharInfo = gameDocument.MainRoleInfo;
            }
            else
            {
                var roleInfo = GenGameCharInfoFromSchema(charName, charLevel);
                if (roleInfo == null)
                    return;

                if (gameDocument.Team == null)
                    gameDocument.Team = new List<string>(MAX_TEAM_NUMBER);
                if (gameDocument.Candidates == null)
                    gameDocument.Candidates = new List<GameCharDisplayInfo>();

                if (gameDocument.Candidates != null)
                {
                    foundCharInfo = gameDocument.Candidates.Find(x => x.CharName == charName);
                    if (foundCharInfo != null)
                    {
                        foundCharInfo.HP = roleInfo.HP;
                        foundCharInfo.MP = roleInfo.MP;
                        foundCharInfo.Level = roleInfo.Level;
                        foundCharInfo.Weapon = roleInfo.Weapon;
                        foundCharInfo.Armor = roleInfo.Armor;
                        foundCharInfo.Accessory1 = roleInfo.Accessory1;
                        foundCharInfo.Accessory2 = roleInfo.Accessory2;
                        foundCharInfo.UsedItems = new List<GameItemInfo>();
                        foundCharInfo.UsedItems.AddRange(roleInfo.UsedItems);
                    }
                    else
                    {
                        foundCharInfo = roleInfo;
                        gameDocument.Candidates.Add(foundCharInfo);
                    }

                    if (gameDocument.Team.Count < MAX_TEAM_NUMBER)
                    {
                        gameDocument.Team.Add(roleInfo.CharName);
                    }
                }
            }
        }

        public void DismissCharFromTeam(string charName)
        {
            var gameDocument = GetCurrentDocument();
            if (charName == ResourceUtils.MAINROLE_NAME)
            {
                Debug.LogWarning("MainRole can't be dismissed");
                return;
            }

            if (gameDocument.Team != null)
            {
                int removed = gameDocument.Team.RemoveAll(x => x == charName);
                // TODO:某队员离队后,需要重新更新游戏中某些场景的布局或者剧情数据
            }
        }

        /// <summary>
        /// 判断某角色是否在队伍中
        /// </summary>
        /// <param name="charID"></param>
        /// <returns></returns>
        public bool IsCharInTeam(string charName)
        {
            bool inTeam = false;
            if (_gameDocument == null)
            {
                return false;
            }

            if (_gameDocument.Team != null)
            {
                inTeam = _gameDocument.Team.Exists(x => x == charName);
            }

            return inTeam;
        }

        // 用于统计完成任务比率,实际上就是完成的任务数目/总任务数目
        // 原版游戏中是大于25%就触发南阳任务,这里假设大于15个完成的任务就可以开启
        public float GetTaskCompletedRatio()
        {
            var allQuests = QuestLog.GetAllQuests(QuestState.Unassigned | QuestState.Active | QuestState.Success | QuestState.Failure | QuestState.Grantable);
            var allCompletedQuests = QuestLog.GetAllQuests(QuestState.Success);
            int allQuestsCnt = allQuests.Length;
            int allCompletedQuestsCnt = allCompletedQuests.Length;
            Debug.Log($"[TKSR] Quest Completed Ratio = {allCompletedQuestsCnt} / {allQuestsCnt}");
            float ratio = allCompletedQuestsCnt * 1f / allQuestsCnt;

#if TKSR_DEV && UNITY_EDITOR
            if (m_debugCompletedTaskRatio > 0f && m_debugCompletedTaskRatio <= 1f)
            {
                ratio = m_debugCompletedTaskRatio;
            }
#endif
            return ratio;
        }

        public int GetTaskCompletedCnt()
        {
            var allCompletedQuests = QuestLog.GetAllQuests(QuestState.Success);
            int allCompletedQuestsCnt = allCompletedQuests.Length;
            return allCompletedQuestsCnt;
        }

#if TKSR_DEV && UNITY_EDITOR
        private float m_debugCompletedTaskRatio = 0f;
#endif
    }
}