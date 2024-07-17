using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 可以在Deploy节点或者TransitionDestination上添加,用于更新Quest状态
    /// </summary>
    public class QuestsSetter : MonoBehaviour
    {
        #if UNITY_EDITOR
        public string debugDescription;
        #endif
        
        [Header("Condition Quests")]
        public QuestStateItem[] listQuestStateItemsCondition;
        public QuestEntryStateItem[] listQuestEntryStateItemsCondition;
        
        [Header("Result Quests")]
        public QuestStateItem[] listQuestStateItemsResult;
        public QuestEntryStateItem[] listQuestEntryStateItemsResult;

        protected virtual void Awake()
        {
            TryToChangeQuestsStatus();
        }

        public bool TryToChangeQuestsStatus()
        {
            if (AreConditionsSatisfied())
            {
                SaveQuestsDataToDialogueDatabase();
                return true;
            }

            return false;
        }
        
        private bool AreConditionsSatisfied()
        {
            if ((listQuestStateItemsCondition == null || listQuestStateItemsCondition.Length == 0) &&
                (listQuestEntryStateItemsCondition == null || listQuestEntryStateItemsCondition.Length == 0))
            {
                return false;
            }
            
            foreach (var item in listQuestStateItemsCondition)
            {
                if (!item.IsQuestSatisfied())
                {
                    return false;
                }
            }
            
            foreach (var item in listQuestEntryStateItemsCondition)
            {
                if (!item.IsQuestEntrySatisfied())
                {
                    return false;
                }
            }

            return true;
        }

        private void SaveQuestsDataToDialogueDatabase()
        {
            if ((listQuestStateItemsResult == null || listQuestStateItemsResult.Length == 0) &&
                (listQuestEntryStateItemsResult == null || listQuestEntryStateItemsResult.Length == 0))
            {
                return;
            }
            
            foreach (var item in listQuestStateItemsResult)
            {
                QuestLog.SetQuestState(item.questName, item.questState);
            }

            foreach (var item in listQuestEntryStateItemsResult)
            {
                int entryId = 0;
                string questName = QuestEntryStateItem.GetQuestNameEntryId(item.questEntryName, ref entryId);
                if (string.IsNullOrEmpty(questName) || entryId == 0)
                {
                    Debug.LogWarning($"[TKSR] Can not found Quest Entry with name = {item.questEntryName}");
                    continue;
                }
                
                QuestLog.SetQuestEntryState(questName, entryId, item.questEntryState);
            }
        }
    }
}