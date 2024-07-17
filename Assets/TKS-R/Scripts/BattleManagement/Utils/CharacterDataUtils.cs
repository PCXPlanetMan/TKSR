using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class LevelUpData
    {
        public string Name;
        public int Level;
        public int MHP;
        public int MMP;
        public int ATK;
        public int DEF;
        public int MAT;
        public int MDF;
        public int HIT;
        public int EVD;
        public int SPD;
    }
    
    public class CharacterDataUtils
    {
        public static readonly int MAX_DESIGN_LEVEL = 60;
        
        private static LevelUpData LevelUp(LevelUpData old_lv, JobStartingStats jobStartStats, JobGrowthStats jobGrowthStats)
        {
            LevelUpData lv = new LevelUpData();
            lv.Name = jobStartStats.Name;
            if (old_lv == null)
            {
                lv.Level = 1;
                lv.MHP = jobStartStats.MHP;
                lv.MMP = jobStartStats.MMP;
                lv.ATK = jobStartStats.ATK;
                lv.DEF = jobStartStats.DEF;
                lv.MAT = jobStartStats.MAT;
                lv.MDF = jobStartStats.MDF;
                lv.HIT = jobStartStats.HIT;
                lv.EVD = jobStartStats.EVD;
                lv.SPD = jobStartStats.SPD;
            }
            else
            {
                lv.Level = old_lv.Level + 1;
                lv.MHP = CalcValueByLevel(old_lv.MHP, lv.Level, jobGrowthStats.MHP);
                lv.MMP = CalcValueByLevel(old_lv.MMP, lv.Level, jobGrowthStats.MMP);
                lv.ATK = CalcValueByLevel(old_lv.ATK, lv.Level, jobGrowthStats.ATK);
                lv.DEF = CalcValueByLevel(old_lv.DEF, lv.Level, jobGrowthStats.DEF);
                lv.MAT = CalcValueByLevel(old_lv.MAT, lv.Level, jobGrowthStats.MAT);
                lv.MDF = CalcValueByLevel(old_lv.MDF, lv.Level, jobGrowthStats.MDF);
                lv.HIT = CalcValueByLevel(old_lv.HIT, lv.Level, jobGrowthStats.HIT);
                lv.EVD = CalcValueByLevel(old_lv.EVD, lv.Level, jobGrowthStats.EVD);
                lv.SPD = CalcValueByLevel(old_lv.SPD, lv.Level, jobGrowthStats.SPD);
            }

            return lv;
        }
        
        static int CalcValueByLevel(int oldValue, int newLevel, string jobStatsValue)
        {
            int whole = 0;
            int fraction = 0;
            ParseGrowthParam(jobStatsValue, ref whole, ref fraction);
            int stepValue = 0;
            int curIndex = newLevel;
            while (curIndex - fraction > 0 && fraction > 0)
            {
                stepValue++;
                curIndex -= fraction;
                fraction--;
                if (fraction == 0)
                {
                    break;
                }
            }

            var newValue = oldValue + whole + stepValue;
            return newValue;
        }
        
        public static void ParseGrowthParam(string strValue, ref int baseValue, ref int subValue)
        {
            var data = strValue.Split('.');
            baseValue = int.Parse(data[0]);
            if (data.Length > 1)
            {
                subValue = int.Parse(data[1]);
            }
            else
            {
                subValue = 0;
            }
        }

        public static List<LevelUpData> CalcDesignLevelUpData(JobStartingStats jobStartStats, JobGrowthStats jobGrowthStats)
        {
            List<LevelUpData> listAllLevelUpData = new List<LevelUpData>();

            LevelUpData old_lv = null;
            int curMaxLevel = 0;
            for (int j = 1; j <= MAX_DESIGN_LEVEL; j++)
            {
                LevelUpData lv = LevelUp(old_lv, jobStartStats, jobGrowthStats);
                listAllLevelUpData.Add(lv);
                old_lv = lv;
            }

            return listAllLevelUpData;
        }

        public static LevelUpData CalcCharLevelUpConfigData(int targetLevelUp, JobStartingStats jobStartStats, JobGrowthStats jobGrowthStats)
        {
            LevelUpData target_lv = null;
            int curMaxLevel = 0;
            for (int j = 1; j <= targetLevelUp; j++)
            {
                LevelUpData lv = LevelUp(target_lv, jobStartStats, jobGrowthStats);
                target_lv = lv;
            }

            return target_lv;
        }
    }
}