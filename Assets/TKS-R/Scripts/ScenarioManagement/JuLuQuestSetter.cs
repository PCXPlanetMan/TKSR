using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 巨鹿村判断是否降雨之后
    /// </summary>
    public class JuLuQuestSetter : MonoBehaviour
    {
        #if UNITY_EDITOR
        public string debugDescription;
#endif

        private readonly float THRESHOLD_OF_MAX_COMPLETED_TASKS_RATIO = 0.25f;

        private void Awake()
        {
            TryToUpdateJuLuOutline();
        }

        public void TryToUpdateJuLuOutline()
        {
            string strNanYangQuest = "Other Return : ZhangJiao";
            var questState = QuestLog.GetQuestState(strNanYangQuest);
            if ((questState & QuestState.Active) == QuestState.Active)
            {
                int entryId = 0;
                string questEntryName = "ZhangJiao Return : Do Rain And Stolen Book";
                string questName = QuestEntryStateItem.GetQuestNameEntryId(questEntryName, ref entryId);

                if (!string.IsNullOrEmpty(questName) && entryId != 0)
                {
                    var foundQuestEntryState = QuestLog.GetQuestEntryState(questName, entryId);
                    QuestState questEntryState = QuestState.Active | QuestState.Success;

                    if ((questEntryState & foundQuestEntryState) == foundQuestEntryState)
                    {
                        DoUpdateJuLuCity();
                    }
                }
            }

            return;
        }

        private void DoUpdateJuLuCity()
        {
            var oldCity = this.gameObject.GetComponent<SpriteRenderer>();
            if (oldCity != null)
            {
                oldCity.enabled = false;
            }

            var updateCity = this.transform.Find("Updated_Map");
            if (updateCity != null)
            {
                updateCity.gameObject.SetActive(true);
                var updatePartCont = updateCity.childCount;
                if (updatePartCont > 0)
                {
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        var oldPart = this.transform.GetChild(i);
                        if (oldPart.name.CompareTo("Updated_Map") == 0)
                        {
                            continue;
                        }

                        oldPart.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}