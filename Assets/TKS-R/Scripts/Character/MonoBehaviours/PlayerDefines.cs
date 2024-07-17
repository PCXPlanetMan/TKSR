using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public enum EnumFaceDirection
    {
        Invalid = -1,
        N = 0,
        NE = 1,
        E = 2,
        SE = 3,
        S = 4,
        SW = 5,
        W = 6,
        NW = 7,
    }

    public class ConstDefines
    {
        public static float MIN_DISTANCE = .05f;
    }
    
    public enum EnumWeaponType
    {
        None,
        Sword,
        Blade,
        Arrow,
        Spear, // 枪
        Pike, // 矛
        Halberd, // 戟
        Broadsword, // 大刀
        Fan,
        Hammer,
        Dice,
        All
    }
    
    public enum EnumToastType
    {
        Normal = 0,
        TKRItem,
        Money,
        Skill,
        Char,
        IntCourageMor,
        Text
    }

    public enum EnumTinyGameUI
    {
        Invalid = 0,
        InputName,
        PuzzleBaGua,
        PuzzleHuaRongDao
    }

    public enum EnumBattleAnimStatus
    {
        None = 0,
        Open,
        EndWin,
        EndLose
    }

    

    public enum EnumCampType
    {
        None,
        Wei,
        Shu,
        Wu,
        Other
    }
}