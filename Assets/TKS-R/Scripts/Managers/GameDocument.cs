using System.Collections.Generic;

namespace TKSR
{
    // In order to make a object in Unity that's convertable to json it appears it needs to be a regular c# object.
    // Unity does not support property getter and setter. Remove the { get; set; } from all the classes .

    [System.Serializable]
    public class GameItemInfo
    {
        public int ItemId;
        public int Count;
    }

    [System.Serializable]
    public class GameSkillInfo
    {
        public int SkillId;
        public int LvUpPoint;
        public bool Learned;
    }

    [System.Serializable]
    public class GameCharDisplayInfo
    {
        public string CharName;
        public int Level;
        public int HP;
        public int MP;
        public int MaxHP;
        public int MaxMP;
        public int Exp;
        public int NeedUpExp;
        public string Status;

        public int AttackValue;
        public int DefenseValue;
        public int HitValue;
        public int EvadeValue;
        public int SpeedValue;
        public int LuckValue;
        public int UnderstandValue;
        public int MovementValue;
        public int SkillPoints;
        
        public int Weapon;
        public int Armor;
        public int Accessory1;
        public int Accessory2;
        
        public List<GameItemInfo> UsedItems;
        public List<GameSkillInfo> SkillsTree;
    }

    [System.Serializable]
    public class GameTaskCharOwnItem
    {
        public int CharId;
        public List<GameItemInfo> OwnedItems;
    }

    [System.Serializable]
    public class GameScenarioInfo
    {
        public int ScenarioId;
        public bool ScenarioDone;
        public Dictionary<string, int> CurrentTasks;

        public GameScenarioInfo()
        {
            ScenarioDone = false;
        }
    }

    [System.Serializable]
    public class GameEventInfo
    {
        public int EventId;
        public List<GameScenarioInfo> ContainScenarios;
        public bool EventDone;

        public GameEventInfo()
        {
            ContainScenarios = new List<GameScenarioInfo>();
            EventDone = false;
        }
    }

    /// <summary>
    /// 按照Dialogue组件的Quest格式进行封装
    /// </summary>
    [System.Serializable]
    public class GameQuestState
    {
        public string QuestName;
        public int StateInt;

        public List<GameQuestEntryState> QuestEntryStates;
    }
    
    /// <summary>
    /// 按照Dialogue组件的QuestEntry格式进行封装
    /// </summary>
    [System.Serializable]
    public class GameQuestEntryState
    {
        public int QuestEntryId;
        public int EntryStateInt;
    }
    

    [System.Serializable]
    public class GameDocument
    {
        public int DocumentId;
        public string SceneName;
        public float PosX;
        public float PosY;
        public string Direction;
        public uint Gold;
        public uint Morality;
        public uint Intelligence;
        public uint Courage;
        public uint MedicalSkill;
        public string FirstName;
        public string LastName;
        public List<GameQuestState> GameQuests;
        public List<string> I2StoryNotes;
        public List<int> ObtainedChars;
        public long Timestamp;
        public GameCharDisplayInfo MainRoleInfo;
        public List<GameCharDisplayInfo> Candidates;
        public List<string> Team;
        public List<GameItemInfo> Medics;
        public List<GameItemInfo> Props;
        public List<GameItemInfo> Weapons;
        public List<GameItemInfo> Armors;
        public List<GameItemInfo> Accessories;
        public List<GameItemInfo> Specials;

        public List<GameTaskCharOwnItem> TaskCharOwnItems;
    }

    [System.Serializable]
    public class TKSArchives
    {
        // 理论上共有9份存档,但增加一个作为临时(自动)存档index=0
        public List<GameDocument> Documents;
    }
}
