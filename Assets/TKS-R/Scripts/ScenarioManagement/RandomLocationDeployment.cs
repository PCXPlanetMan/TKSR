using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    public class RandomLocationDeployment : MonoBehaviour
    {
        private List<ScenarioQuests> m_ScenarioQuests;

        [SerializeField] private GameObject deployment;

        [SceneName]
        public string locationName;
        
        void Awake()
        {
            var allQuests = this.GetComponents<ScenarioQuests>();
            m_ScenarioQuests = allQuests.ToList();

            RunQuestsToDeploy();
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
            Lua.RegisterFunction(nameof(RandomGenNextLocation), this, SymbolExtensions.GetMethodInfo(() => RandomGenNextLocation()));
            Lua.RegisterFunction(nameof(AddOne), this, SymbolExtensions.GetMethodInfo(() => AddOne((double)0)));
        }

        void OnDisable()
        {
            // Remove the functions from Lua: (Replace these lines with your own.)
            Lua.UnregisterFunction(nameof(RandomGenNextLocation));
            Lua.UnregisterFunction(nameof(AddOne));
        }
        

        public double AddOne(double value)
        { // Note: Lua always passes numbers as doubles.
            return value + 1;
        }

        public void RandomGenNextLocation()
        {
            if (string.IsNullOrEmpty(locationName))
            {
                Debug.Log("[TKSR] Not Set temp last city of HuaTuo or Already Random set the current city.");
                return;
            }
            
            var questName = "Other Return : HuaTuo";
            
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
                        QuestLog.SetQuestEntryState(questName, i, QuestState.Active);
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
    }
}
