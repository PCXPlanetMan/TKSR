using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    [RequireComponent(typeof(Collider2D))]
    public class TransitionPoint : MonoBehaviour
    {
        public enum TransitionType
        {
            DifferentZone, SameScene, OnlyDeploy, BattleScene, Nothing
        }


        public enum TransitionWhen
        {
            ExternalCall, InteractPressed, OnTriggerEnter,
        }

    
        [Tooltip("This is the gameobject that will transition.  For example, the player.")]
        public GameObject transitioningGameObject;
        [Tooltip("Whether the transition will be within this scene, to a different zone or a non-gameplay scene.")]
        public TransitionType transitionType;
        [SceneName]
        public string newSceneName;
        [Tooltip("The tag of the SceneTransitionDestination script in the scene being transitioned to.")]
        public SceneTransitionDestination.DestinationTag transitionDestinationTag;
        [Tooltip("The destination in this scene that the transitioning gameobject will be teleported.")]
        public TransitionPoint destinationTransform;
        [Tooltip("Do deployment for scene.")]
        public GameObject deployGameObject;
        [Tooltip("What should trigger the transition to start.")]
        public TransitionWhen transitionWhen;
        
        // [TKSR] 用于TKSR的跳转参数,场景切换过程中可以显示某些字符串,点击继续
        // 这些参数不需要在Inspector里面设置
        [Tooltip("Pause scene loading?")]
        public bool pauseSceneLoading;
        [Tooltip("Tips display when scene loading.")]
        public string tipsWhenSceneLoading;
        [Tooltip("Battle Scene when timeline scenario finished.")] 
        [BattleName]
        public string battleSceneName;
        
        [Tooltip("The player will lose control when the transition happens but should the axis and button values reset to the default when control is lost.")]
        public bool resetInputValuesOnTransition = true;
        [Tooltip("Is this transition only possible with specific items in the inventory?")]
        public bool requiresInventoryCheck;
        [Tooltip("The inventory to be checked.")]
        public InventoryController inventoryController;
        [Tooltip("The required items.")]
        public InventoryController.InventoryChecker inventoryCheck;
        
        bool m_TransitioningGameObjectPresent;

        void Start ()
        {
            if (transitionWhen == TransitionWhen.ExternalCall)
                m_TransitioningGameObjectPresent = true;
        }

        void OnTriggerEnter2D (Collider2D other)
        {
            if (other.gameObject == transitioningGameObject)
            {
                m_TransitioningGameObjectPresent = true;

                if (ScreenFader.IsFading || SceneController.Transitioning)
                    return;

                if (transitionWhen == TransitionWhen.OnTriggerEnter)
                    TransitionInternal ();
            }
        }

        void OnTriggerExit2D (Collider2D other)
        {
            if (other.gameObject == transitioningGameObject)
            {
                m_TransitioningGameObjectPresent = false;
            }
        }

        void Update ()
        {
            if (ScreenFader.IsFading || SceneController.Transitioning)
                return;

            if(!m_TransitioningGameObjectPresent)
                return;

            if (transitionWhen == TransitionWhen.InteractPressed)
            {
                if (PlayerInput.Instance.Interact.Down)
                {
                    TransitionInternal ();
                }
            }
        }

        protected void TransitionInternal ()
        {
            if (requiresInventoryCheck)
            {
                if(!inventoryCheck.CheckInventory (inventoryController))
                    return;
            }
        
            if (transitionType == TransitionType.SameScene)
            {
                GameObjectTeleporter.Teleport (transitioningGameObject, destinationTransform.transform);
            }
            else if (transitionType == TransitionType.OnlyDeploy)
            {
                Debug.Log("[TKSR] Do nothing but only do deployment after timeline finished.");
                if (deployGameObject != null)
                {
                    deployGameObject.gameObject.SetActive(true);
                }

                // SceneController.DoDeployment(deployGameObject);
            }
            else if (transitionType == TransitionType.BattleScene)
            {
                if (string.IsNullOrEmpty(battleSceneName))
                {
                    Debug.LogError("[TKSR] Transition to battle scene, but no battle scene name.");
                    return;
                }
                SceneController.TransitionToBattle(this);
            }
            else if (transitionType == TransitionType.DifferentZone)
            {
                // [TKSR] 南阳城入口检测
                if (newSceneName.StartsWith("NanYang"))
                {
                    var questSetter = this.gameObject.GetComponent<NanYangQuestSetter>();
                    if (questSetter != null)
                    {
                        questSetter.TryToChangeQuestsStatus();
                    }
                }


                SceneController.TransitionToScene (this);
            }
        }

        public void Transition ()
        {
            if(!m_TransitioningGameObjectPresent)
                return;
            
            if(transitionWhen == TransitionWhen.ExternalCall)
                TransitionInternal ();
        }

        /// <summary>
        /// 从对话系统的Scene Event中解析特定的（例如用于专场的）本地字符串
        /// </summary>
        /// <param name="strData"></param>
        public void TransitionForDialogEntry(string strData)
        {
            var stringArray = strData.Split(',');
            string strEntryTitle = stringArray[0];
            string strEntryId = stringArray[1];
            int withEntryId = int.Parse(strEntryId);
            
            pauseSceneLoading = true;
            var db = DialogueManager.masterDatabase;
            var conversion = db.GetConversation(strEntryTitle);
            var entry = conversion.GetDialogueEntry(withEntryId);
            if (entry != null)
            {
                string subtitleText = entry.subtitleText;
                
                var localizedText = DialogueManager.GetLocalizedText(subtitleText);
                var formattedText = FormattedText.Parse(localizedText, db.emphasisSettings);
                var strDialogContent = formattedText.text;
                tipsWhenSceneLoading = strDialogContent;
            }
            Transition();
        }
    }
}