using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    [Serializable]
    public class QuestStateItem
    {
        [QuestName]
        public string questName;
        public QuestState questState;
        
        public bool IsQuestSatisfied()
        {
            var foundQuestState = QuestLog.GetQuestState(questName);
            if ((questState & foundQuestState) == foundQuestState)
            {
                return true;
            }
            return false;
        }
    }
}
