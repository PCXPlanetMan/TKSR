using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 用于判断Quest是否满足
    /// </summary>
    public class QuestsChecker : MonoBehaviour
    {
        #if UNITY_EDITOR && TKSR_DEV
        public string questDescription;
        #endif
        public QuestStateItem[] listQuestStateItems;
        public QuestEntryStateItem[] listQuestEntryStateItems;
        
        /// <summary>
        /// 一旦Quest和QuestEntry列表中有任一一个条件不满足,则说明整个QuestsChecker失败,只有当所有条件都满足才能说明此QuestsChecker成功
        /// </summary>
        /// <returns></returns>
        public virtual bool AreConditionsSatisfied()
        {
            if ((listQuestStateItems == null || listQuestStateItems.Length == 0) &&
                (listQuestEntryStateItems == null || listQuestEntryStateItems.Length == 0))
            {
                return false;
            }
            
            foreach (var item in listQuestStateItems)
            {
                if (!item.IsQuestSatisfied())
                {
                    return false;
                }
            }
            
            foreach (var item in listQuestEntryStateItems)
            {
                if (!item.IsQuestEntrySatisfied())
                {
                    return false;
                }
            }

            return true;
        }

        public string LogQuests()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[Quests]=");
            foreach (var quest in listQuestStateItems)
            {
                stringBuilder.Append(quest.questName);
                stringBuilder.Append("+(");
                stringBuilder.Append(quest.questState);
                stringBuilder.Append(");");
            }

            stringBuilder.Append("||||[QuestEntries]=");
            foreach (var entry in listQuestEntryStateItems)
            {
                stringBuilder.Append(entry.questEntryName);
                stringBuilder.Append("+(");
                stringBuilder.Append(entry.questEntryState);
                stringBuilder.Append(");");
            }

            return stringBuilder.ToString();
        }
    }
}