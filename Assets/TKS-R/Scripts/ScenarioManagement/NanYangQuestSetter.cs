using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 在大地图南阳城入口检测贾诩任务开启条件
    /// </summary>
    public class NanYangQuestSetter : MonoBehaviour
    {
        #if UNITY_EDITOR
        public string debugDescription;
#endif

        private readonly float THRESHOLD_OF_MAX_COMPLETED_TASKS_RATIO = 0.25f;


        public void TryToChangeQuestsStatus()
        {
            string strNanYangQuest = "Wei Return : JiaXu";
            var questState = QuestLog.GetQuestState(strNanYangQuest);
            if ((questState & QuestState.Unassigned) == QuestState.Unassigned)
            {
                float taskCompletedRatio = DocumentDataManager.Instance.GetTaskCompletedRatio();
                if (taskCompletedRatio >= THRESHOLD_OF_MAX_COMPLETED_TASKS_RATIO)
                {
                    QuestLog.SetQuestState(strNanYangQuest, QuestState.Active);

                    Debug.Log("[TKSR] Task Completed Ratio is greater than 25%, Make NanYang City JiaXu Quest active.");
                }               
            }

            return;
        }
    }
}