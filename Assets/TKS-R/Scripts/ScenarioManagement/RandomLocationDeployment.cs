using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 专用于华佗在不同的城市中出现
    /// </summary>
    public class RandomLocationDeployment : MonoBehaviour
    {
        private List<ScenarioQuests> m_ScenarioQuests;

        // 当华佗在其他城市执行任务的时候,就不需要检查Quests.
        public bool checkQuests = true;
        [SerializeField] private GameObject deployment;
        [SceneName]
        public string locationName;
        
        void Awake()
        {
            if (checkQuests)
            {
                var allQuests = this.GetComponents<ScenarioQuests>();
                m_ScenarioQuests = allQuests.ToList();

                RunQuestsToDeploy();
            }
        }

        private void RunQuestsToDeploy()
        {
            if (m_ScenarioQuests == null)
            {
                return;
            }
            
            bool found = false;

            foreach (var quest in m_ScenarioQuests)
            {
                if (quest.AreConditionsSatisfied())
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                deployment.gameObject.SetActive(true);
            }
        }
        
        void OnEnable()
        {
            // Make the functions available to Lua: (Replace these lines with your own.)
            Lua.RegisterFunction(nameof(RandomGenNextLocation), this, SymbolExtensions.GetMethodInfo(() => RandomGenNextLocation(true)));
            Lua.RegisterFunction(nameof(HuaTuoReturnToRealNextLocation), this, SymbolExtensions.GetMethodInfo(() => HuaTuoReturnToRealNextLocation()));
        }

        void OnDisable()
        {
            // Remove the functions from Lua: (Replace these lines with your own.)
            Lua.UnregisterFunction(nameof(RandomGenNextLocation));
            Lua.UnregisterFunction(nameof(HuaTuoReturnToRealNextLocation));
        }
        
        // [QuestName]
        // private string questName;
        
        private readonly string questName = "Other Return : HuaTuo";
        
        /// <summary>
        /// realGen = false : 当某些任务需要华佗去看病,此时华佗离开后将暂时不会出现在设定的5个城市中,直到任务完成。
        /// 此时不要将城市所在的Entry设置为Active即可
        /// </summary>
        public void RandomGenNextLocation(bool realGen)
        {
            if (string.IsNullOrEmpty(locationName))
            {
                Debug.Log("[TKSR] Not Set temp last city of HuaTuo or Already Random set the current city.");
                return;
            }
            
            Debug.Log($"[TKSR] Last location city of HuaTuo is {locationName}");
            List<string> tempLocations = new List<string>();
            tempLocations.AddRange(ResourceUtils.s_HuaTuoLocations);
            tempLocations.Remove(locationName);
            if (tempLocations.Count == ResourceUtils.s_HuaTuoLocations.Count - 1)
            {
                int randomIndex = Random.Range(0, tempLocations.Count);
                string nextCity = tempLocations[randomIndex];
                int findIndex = ResourceUtils.s_HuaTuoLocations.FindIndex(x => x.CompareTo(nextCity) == 0);
                int entryId = findIndex + 1;
                Debug.Log($"[TKSR] HuaTuo will locate in City : {nextCity} with EntryId = {entryId}");

                for (int i = 1; i <= ResourceUtils.s_HuaTuoLocations.Count; i++)
                {
                    if (i == entryId)
                    {
                        if (realGen)
                            QuestLog.SetQuestEntryState(questName, i, QuestState.Active);
                        else
                        {
                            QuestLog.SetQuestEntryState(questName, i, QuestState.Grantable);
                        }
                    }
                    else
                    {
                        QuestLog.SetQuestEntryState(questName, i, QuestState.Unassigned);
                    }
                }
            }
            else
            {
                Debug.LogError($"[TKSR] Current Locations City of HuaTuo is Not Correct: {string.Join(',', tempLocations)}");
            }
        }
        
        /// <summary>
        /// 当华佗完成某些任务后需要重新设置其下一个可用的随机城市
        /// </summary>
        public void HuaTuoReturnToRealNextLocation()
        {
            string lastRandomCity = string.Empty;
            for (int i = 1; i <= ResourceUtils.s_HuaTuoLocations.Count; i++)
            {
                var questEntryName = QuestLog.GetQuestEntry(questName, i);
                var questEntryState = QuestLog.GetQuestEntryState(questName, i);
                Debug.Log($"[TKSR] HuaTuo Re-Gen to Next City : i={i}, QuestEntry={questEntryName}, State={questEntryState}");
                if (questEntryState == QuestState.Grantable)
                {
                    QuestLog.SetQuestEntryState(questName, i, QuestState.Active);

                    if (string.IsNullOrEmpty(lastRandomCity))
                    {
                        lastRandomCity = questEntryName;
                    }
                    else
                    {
                        Debug.LogError("[TKSR] Found too many last random city for HuaTuo.");
                    }
                }
                else
                {
                    QuestLog.SetQuestEntryState(questName, i, QuestState.Unassigned);
                }
            }

            if (string.IsNullOrEmpty(lastRandomCity))
            {
                Debug.LogError("[TKSR] Not found a valid next random city for HuaTuo.");
            }
        }
    }
}
