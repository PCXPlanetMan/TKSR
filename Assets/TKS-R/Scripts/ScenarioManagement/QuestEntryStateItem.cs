using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    [Serializable]
    public class QuestEntryStateItem
    {
        [QuestEntryName]
        public string questEntryName;
        public QuestState questEntryState;
        
        public bool IsQuestEntrySatisfied()
        {
            int entryId = 0;
            string questName = GetQuestNameEntryId(questEntryName, ref entryId);

            if (string.IsNullOrEmpty(questName) || entryId == 0)
            {
                return false;
            }
            
            var foundQuestEntryState = QuestLog.GetQuestEntryState(questName, entryId);
            if ((questEntryState & foundQuestEntryState) == foundQuestEntryState)
            {
                return true;
            }
            return false;
        }

        public static string GetQuestNameEntryId(string entryName, ref int entryId)
        {
            string questName = null;
            entryId = 0;
            var allQuests = QuestLog.GetAllQuests(QuestState.Unassigned | QuestState.Active | QuestState.Success | QuestState.Failure);
            foreach (var quest in allQuests)
            {
                for (int i = 1; i <= QuestLog.GetQuestEntryCount(quest); i++)
                {
                    var questEntry = QuestLog.GetQuestEntry(quest, i);
                    if (questEntry.CompareTo(entryName) == 0)
                    {
                        entryId = i;
                        break;
                    }
                }

                if (entryId > 0)
                {
                    questName = quest;
                    break;
                }
            }

            return questName;
        }
    }
}
