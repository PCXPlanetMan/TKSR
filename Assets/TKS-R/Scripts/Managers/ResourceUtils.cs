using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{


    public static class ResourceUtils
    {
        #region 游戏资源常量

        public static string AB_YOO_PACKAGE = "TKSRDefaultPackage";
        public static string AB_SCHEMA_CHARACTERS = "SchemaCharacters";
        public static string AB_SCHEMA_ITEMS = "SchemaItems";
        #endregion
        


        #region 游戏内容常量

        public static string MAINROLE_NAME = "MainPlayer"; // 主角实例ID
        public static string DIALOGUE_ACTOR_VIRTUAL_SELECTOR = "VirtualSelector";
        
        public static string CONFIG_GOLD = "GOLD";
        public static uint MAX_GOLD_COUNT = 30000;
        public static uint MAX_ITEM_COUNT = 99;

        public static string DIALOGUE_VAR_FORMAT_FIRST_NAME = "Main Player First Name";
        public static string DIALOGUE_VAR_FORMAT_LAST_NAME = "Main Player Last Name";
        public static string DIALOGUE_VAR_FORMAT_MAXINTEL = "MaxIntelligence";
        public static string DIALOGUE_VAR_FORMAT_MAXMOR = "MaxMorality";
        public static string DIALOGUE_VAR_FORMAT_MAXCOR = "MaxCourage";
        public static string DIALOGUE_VAR_FORMAT_GOLDEN = "GoldenValue";
        
        /// <summary>
        /// 华佗可能出现的5个城市
        /// </summary>
        public static List<string> s_HuaTuoLocations = new List<string>()
        {
            "ChangAn/Map_CityChangAn_Street", "WuWei/Map_CityWuWei_Street", "YeCheng/Map_CityYeCheng", "ChengDu/Map_CityChengDu", "JianYe/Map_CityJianYe"
        };
        #endregion


        #region UI格式化字符串

        public static readonly string FORMAT_DOC_CHAR_LV = "LV {0}";
        public static readonly string I2PARAM_DATE_YEAR = "Year";
        public static readonly string I2PARAM_DATE_MONTH = "Month";
        public static readonly string I2PARAM_DATE_DAY = "Day";
        public static readonly string I2PARAM_PAGE_INDEX = "PageIndex";
        public static readonly string I2PARAM_PAGES_CNT = "Pages";
        public static readonly string I2PARAM_MEMBER_HP = "MemberHp";
        public static readonly string I2PARAM_MEMBER_MP = "MemberMp";
        public static readonly string I2PARAM_MAINPLAYER_FIRSTNAME = "MainFirstName";
        public static readonly string I2PARAM_MAINPLAYER_LASTNAME = "MainLastName";
        
        public static readonly string I2PARAM_STATUS_LV = "StatusLv";
        public static readonly string I2PARAM_STATUS_HP = "StatusHp";
        public static readonly string I2PARAM_STATUS_MP = "StatusMp";
        public static readonly string I2PARAM_STATUS_EXP = "StatusExp";
        public static readonly string I2PARAM_STATUS_NEEDUPEXP = "StatusNeedUpExp";
        public static readonly string I2PARAM_STATUS_ATK = "StatusAttack";
        public static readonly string I2PARAM_STATUS_DEF = "StatusDefense";
        public static readonly string I2PARAM_STATUS_HIT = "StatusHit";
        public static readonly string I2PARAM_STATUS_EVD = "StatusEvade";
        public static readonly string I2PARAM_STATUS_SPD = "StatusSpeed";
        public static readonly string I2PARAM_STATUS_LCK = "StatusLuck";
        public static readonly string I2PARAM_STATUS_USD = "StatusUnderstand";
        public static readonly string I2PARAM_STATUS_MOV = "StatusMove";
        public static readonly string I2PARAM_STATUS_SPS = "StatusSkillPoints";
        public static readonly string I2PARAM_FORMAT_GOLDCOUNT = "GoldCount";
        public static readonly string I2PARAM_FORMAT_ITEMNAME = "ItemName";
        
        
        public static readonly string FORMAT_DATE_HMS = "{0:D2}:{1:D2}:{2:D2}";
        public static readonly string I2FORMAT_DOC_LOAD = "UI/Doc_Load";
        public static readonly string I2FORMAT_DOC_SAVE = "UI/Doc_Save";
        public static readonly string I2FORMAT_BATTLE_OPENING = "Battle/Battle_Opening";
        public static readonly string I2FORMAT_BATTLE_WIN = "Battle/Battle_Win";
        public static readonly string I2FORMAT_BATTLE_LOSE = "Battle/Battle_Lose";
        public static readonly string I2FORMAT_GOT_MONEY = "Format/GotGolds";
        public static readonly string I2FORMAT_GOT_ITEM = "Format/GotItem";
        public static readonly string I2FORMAT_GOLD_NOT_ENOUGH = "Format/GoldsNotEnough";
        public static readonly string I2FORMAT_QUEST_LIUCONG_TOILET_TIME = "Format/QuestLiuCong_ToiletTime";
        
        public static readonly string I2FORMAT_BATTLE_CHAR_CATEGORY = "Battle/Characters/";
        public static readonly string I2FORMAT_BATTLE_ABILITY_CATEGORY = "Battle/Ability/";
        public static readonly string I2FORMAT_SCENES_NAMES = "ScenesNames/";
        public static readonly string I2FORMAT_ITEMS_CATEGORY_MEDICS = "Configs/Items/Medics/";
        public static readonly string I2FORMAT_ITEMS_CATEGORY_PROPS = "Configs/Items/Props/";
        public static readonly string I2FORMAT_ITEMS_CATEGORY_WEAPONS = "Configs/Items/Weapons/";
        public static readonly string I2FORMAT_ITEMS_CATEGORY_ARMORS = "Configs/Items/Armors/";
        public static readonly string I2FORMAT_ITEMS_CATEGORY_ACCESSORIES = "Configs/Items/Accessories/";
        public static readonly string I2FORMAT_ITEMS_CATEGORY_SPECIALS = "Configs/Items/Specials/";
        
        public static readonly string I2FORMAT_TEAM_CATEGORY_NOTES = "UI/SettingInfo/Team/Notes/";
        
        #endregion

        #region 部分配置数据
        public static string PREFS_SYSTEM_BGM_ONOFF = "bgmSwitch";
        public static string PREFS_SYSTEM_BGM_VOLUME = "bgmVolume";
        public static string PREFS_SYSTEM_AUDIO_ONOFF = "audioSwitch";
        public static string PREFS_SYSTEM_AUDIO_VOLUME = "audioVolume";
        #endregion


        public static string DOCUMENT_ARCHIVES_FILE_NAME = "save.json";
        public static string CONFIG_CHAR_DLG_ANCHORS = "char_anchors.json";
    }
}