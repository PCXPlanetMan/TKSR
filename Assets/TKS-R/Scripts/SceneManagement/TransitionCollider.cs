using System;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    [RequireComponent(typeof(Collider2D))]
    public class TransitionCollider : SceneTransitionDestination
    {
        bool m_TransitioningGameObjectPresent;

        private Collider2D m_Collider2D;

        [SerializeField]
        private TransitionColliderRender attachColliderRender;

        protected override void Awake()
        {
            base.Awake();
            
            m_Collider2D = this.gameObject.GetComponent<Collider2D>();
            m_Collider2D.enabled = false;
            
            CheckQuestsToOpenColliderByOrder();
        }

        /// <summary>
        /// 在场景中的碰撞体上使用ScenarioQuests时,并不使用其Deployment
        /// </summary>
        public void CheckQuestsToOpenColliderByOrder()
        {
            Debug.Log($"[TKSR] CheckQuestsToOpenColliderByOrder");
            bool hasFoundQuest = FoundSatisfiedQuest();
            if (hasFoundQuest && m_foundQuestsList.Count > 0)
            {
                foreach (var foundQuest in m_foundQuestsList)
                {
                    Debug.Log($"[TKSR] TransitionCollider = {this.gameObject.name}, Found a quest satisfied, open collider. Quest = {foundQuest.LogQuests()}");
                    if (foundQuest.OnReachQuest != null)
                    {
                        foundQuest.OnReachQuest.Invoke();
                    }
                }
                m_Collider2D.enabled = true;
            }
            else
            {
                m_Collider2D.enabled = false;
            }

            if (attachColliderRender != null)
            {
                attachColliderRender.SetRenderByCollider(m_Collider2D.enabled);
            }
        }
        
        void OnTriggerEnter2D (Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Transition();
                m_TransitioningGameObjectPresent = true;
                //Debug.Log($"[TKSR] OnTriggerEnter2D, m_TransitioningGameObjectPresent = {m_TransitioningGameObjectPresent}");
            }
        }

        void OnTriggerExit2D (Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                m_TransitioningGameObjectPresent = false;
                //Debug.Log($"[TKSR] OnTriggerExit2D, m_TransitioningGameObjectPresent = {m_TransitioningGameObjectPresent}");
            }
        }
        
        public void Transition()
        {
            //Debug.Log($"[TKSR] Transition, m_TransitioningGameObjectPresent = {m_TransitioningGameObjectPresent}");
            if (m_TransitioningGameObjectPresent)
                return;
            
            
            if (m_foundQuestsList.Count == 0)
            {
                Debug.Log("[TKSR] Not found a valid Quest. This is just a Static Collider.");
                return;
            }

            PlayerCharacter player = PlayerCharacter.PlayerInstance;
            if (CurrentTimeline != null)
            {
                Debug.Log("[TKSR] Collider triggered to play Timeline.");
                if (player != null)
                {
                    player.PrepareForPlayTimeline();
                }

                // [TKSR] 必须设置transitioningGameObject = null,因为此Collider继承自SceneTransitionDestination,
                // 而SetEnteringGameObjectLocation会自动同步transitioningGameObject(即MainPlayer)的位置和Entrance一致,这对于Collider是不合理的.
                transitioningGameObject = null;
                SceneController.Instance.SetEnteringGameObjectLocation(this);
                // [TKSR] 用于修正如下的m_TransitioningGameObjectPresent相关BUG
                CurrentTimeline.CallBackTimelineEnd = ActionFixedTraggerExit;
                CurrentTimeline.InitTimeline();
                SceneController.Instance.EnableMainPlayerInput(false);
                // [TKSR] BUG:Disable Player的Collider会直接导致OnTriggerExit2D优先OnTriggerEnter2D结束,导致再次进入
                // TransitionCollider时m_TransitioningGameObjectPresent的状态不对而无法执行Transition
                SceneController.Instance.EnableMainPlayerPhysicAndGesture(false);
            }
            else if (SatisfiedTimelineQuest.dlgActor != null && player != null)
            {
                Debug.Log("[TKSR] Collider triggered to do conversation with Actor.");
                var npc = SatisfiedTimelineQuest.dlgActor.GetComponent<NPCCharacter>();
                npc.DoConversationWithMainPlayer();
                player.DoConversationWithNPC(npc);
            }
        }

        private void ActionFixedTraggerExit(int code)
        {
            Debug.Log("[TKSR] Try to Fixed m_TransitioningGameObjectPresent BUG");
            m_TransitioningGameObjectPresent = false;
            if (CurrentTimeline != null)
            {
                CurrentTimeline.CallBackTimelineEnd = null;
            }
        }
    }
}