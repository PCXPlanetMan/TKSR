#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TKSR
{
    public class SceneTransitionDestination : MonoBehaviour
    {
        public enum DestinationTag
        {
            Invaild = -2,
            ByDocument = -1,
            A, B, C, D, E, F, G,
            // [PPAN] Updated by TKSR
            H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
            A1, B1, C1, D1, E1, F1, G1, H1, I1, J1, K1, L1, M1, N1, O1, P1, Q1, R1, S1, T1, U1, V1, W1, X1, Y1, Z1
        }


        public DestinationTag destinationTag;    // This matches the tag chosen on the TransitionPoint that this is the destination for.
        [Tooltip("This is the gameobject that has transitioned.  For example, the player.")]
        public GameObject transitioningGameObject;
        public UnityEvent OnReachDestination;
        [Tooltip("Default deployment used if there is no quest in this entrance.")]
        public GameObject defaultDeployment;
        
        // [TKSR] 显示传送点Gizmo
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GUIStyle labelFont = new GUIStyle();
            labelFont.normal.textColor = Color.red;
            Handles.Label(transform.position, destinationTag.ToString(), labelFont);
        }
#endif

        #region [TKSR] 在场景出口处判断Quests
        // 在每个场景的出口处进行Quests状态判断,以决定进入场景后执行:1.播放剧情Timeline;2.布置场景NPC的Deployment状态;3.什么都不做

        private List<ScenarioQuests> m_ScenarioQuests = null;
        protected virtual void Awake()
        {
            var allQuests = GetComponents<ScenarioQuests>();
            if (allQuests != null && allQuests.Length > 0)
            {
                m_ScenarioQuests = allQuests.ToList();
            }
            else
            {
                m_ScenarioQuests = null;
            }
        }

        // 专用于获取播放Timeline的Quest
        public ScenarioQuests SatisfiedTimelineQuest
        {
            get
            {
                if (m_foundQuestsList.Count != 1)
                {
                    return null;
                }
                else
                {
                    return m_foundQuestsList[0];
                }
            }
        }
        
        public TimelineScenarioItem CurrentTimeline;

        protected List<ScenarioQuests> m_foundQuestsList = new List<ScenarioQuests>(); 

        protected bool FoundSatisfiedQuest()
        {
            if (m_ScenarioQuests == null)
            {
                return false;
            }
            
            m_foundQuestsList.Clear();
            bool found = false;

            foreach (var quest in m_ScenarioQuests)
            {
                if (quest.AreConditionsSatisfied())
                {
                    m_foundQuestsList.Add(quest);
                }
            }
            
            if (m_foundQuestsList.Count > 1) // 满足条件的Quests有可能大于1,这种情况下必须是所有Quests必须同时是执行Deployment
            {
                bool checkResult = true;
                for (int i = 0; i < m_foundQuestsList.Count; i++)
                {
                    Debug.Log($"[TKSR] {i} FoundQuest = {m_foundQuestsList[i].LogQuests()}");
                    if (m_foundQuestsList[i].deployment == null)
                    {
                        checkResult = false;
                        break;
                    }
                }

                if (!checkResult)
                {
                    Debug.LogError($"[TKSR] Found {m_foundQuestsList.Count} quests and not all Do Deployment, this is impossible.");
                    found = false;
                }
                else
                {
                    found = true;
                }
            }
            else if (m_foundQuestsList.Count == 1)
            {
                var satisfiedQuest = m_foundQuestsList[0];
                Debug.Log($"[TKSR] SceneTransitionDestination = {this.gameObject.name}, Quest Found : {satisfiedQuest.LogQuests()}.");
                found = true;
            }
            else
            {
                found = false;
            }

            if (found)
            {
                CurrentTimeline = SatisfiedTimelineQuest != null ? SatisfiedTimelineQuest.timeline : null;
            }
            else
            {
                CurrentTimeline = null;
            }

            return found;
        }
        
        public void DoNextActionsByQuests()
        {
            m_foundQuestsList.Clear();
            
            if (m_ScenarioQuests == null || m_ScenarioQuests.Count == 0)
            {
                Debug.Log("[TKSR] There is no quests need to Check.");
                if (defaultDeployment != null)
                {
                    Debug.Log("[TKSR] No Quests. Do Default Deployment.");
                    defaultDeployment.SetActive(true);
                }

                return;
            }

            bool hasFoundQuest = FoundSatisfiedQuest();
            
            if (hasFoundQuest && m_foundQuestsList.Count > 0)
            {
                foreach (var satisfiedQuest in m_foundQuestsList)
                {
                    // 可以同时执行Deployment和Timeline???????
                    
                    if (satisfiedQuest.deployment != null)
                    {
                        Debug.Log($"[TKSR] SceneTransitionDestination = {this.gameObject.name}, Do deployment = {satisfiedQuest.deployment.name} when Quest Found.");
                        satisfiedQuest.deployment.SetActive(true);
                    }
                    
                    if (satisfiedQuest.timeline != null)
                    {
                        Debug.Log($"[TKSR] SceneTransitionDestination = {this.gameObject.name}, Play timeline = {satisfiedQuest.timeline.name} when Quest Found.");
                        satisfiedQuest.timeline.InitTimeline();
                        SceneController.Instance.EnableMainPlayerInput(false);
                        SceneController.Instance.EnableMainPlayerPhysicAndGesture(false);
                    }
                    // else
                    // {
                    //     Debug.Log("[TKSR] No Deployment and Timeline. Nobody in this scene now.");
                    // }
                
                    if (satisfiedQuest.OnReachQuest != null)
                        satisfiedQuest.OnReachQuest.Invoke();
                }
            }
            else
            {
                if (defaultDeployment != null)
                {
                    Debug.Log("[TKSR] No Quest Found. Use Default Deployment.");
                    defaultDeployment.SetActive(true);
                }
            }
        }
        #endregion
    }
}