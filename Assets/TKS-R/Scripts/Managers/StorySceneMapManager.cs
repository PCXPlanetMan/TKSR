using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using YooAsset;

namespace TKSR
{

    public class StorySceneMapManager : MonoBehaviour
    {
        public static StorySceneMapManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindFirstObjectByType<StorySceneMapManager>();

                if (instance != null)
                    return instance;

                Create();

                return instance;
            }
        }

        protected static StorySceneMapManager instance;

        public static StorySceneMapManager Create()
        {
            GameObject manager = new GameObject("StorySceneMapManager");
            instance = manager.AddComponent<StorySceneMapManager>();

            return instance;
        }
        
        void Awake()
        {
            instance = this;
#if UNITY_EDITOR && TKSR_DEV
            var questName = "Other Return : HuaTuo";
            for (int i = 1; i <= QuestLog.GetQuestEntryCount(questName) && i <= 5; i++)
            {
                var entryState = QuestLog.GetQuestEntryState(questName, i);
                if ((entryState & QuestState.Active) == QuestState.Active)
                {
                    m_foundHuaTuoCityIndex = i;
                    break;
                }
            }

            m_foundHuaTuoCityIndex -= 1;
#endif
        }
        
#if UNITY_EDITOR && TKSR_DEV
        private int m_foundHuaTuoCityIndex = -1;
        void OnGUI()
        {
            if (m_foundHuaTuoCityIndex >= 0 && m_foundHuaTuoCityIndex < ResourceUtils.s_HuaTuoLocations.Count)
            {
                string strDebugCity = ResourceUtils.s_HuaTuoLocations[m_foundHuaTuoCityIndex];
                GUIStyle  myButtonStyle = new GUIStyle(GUI.skin.button);
                myButtonStyle.fontSize = 32;
                GUI.Label(new Rect(0, 120, 600, 120), "华佗所在的城市:\r\n" + strDebugCity, myButtonStyle);
            }
        }
#endif

        public void ActionShowItemByDialogue(string itemParam)
        {
            Debug.Log($"[TKSR] Send item from Dialogue : {itemParam}");
            DocumentDataManager.Instance.TransportDataByTimeline(itemParam);
        }

        public void ActionSaveStoryNote(string strNode)
        {
            Debug.Log($"[TKSR] Save story node from Dialogue : {strNode}");
            DocumentDataManager.Instance.RecordNoteI2(strNode);
        }

        public void ActionDoCheckQuests()
        {
            SceneController.Instance.CurrentEntrance.DoNextActionsByQuests();
        }

        /// <summary>
        /// 显示钱不够的Warning Toast
        /// </summary>
        /// <param name="strWarning">Gold,Duration</param>
        public void ActionShowNotEnoughMoney(string strWarning)
        {
            if (!string.IsNullOrEmpty(strWarning))
            {
                var warningData = strWarning.Split(':');
                float showDuration = 0f;
                int goldAmount = 0;
                if (warningData.Length > 0)
                {
                    if (int.TryParse(warningData[0], out int gold))
                    {
                        goldAmount = gold;
                    }
                }

                if (warningData.Length > 1)
                {
                    if (float.TryParse(warningData[1], out float duration))
                    {
                        showDuration = duration;
                    }
                }

                if (goldAmount > 0)
                {
                    GameUI.Instance.toastPanel.ShowToastNotEnoughMoney(goldAmount, showDuration);
                }
            }
        }
    }
}