using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace TKSR
{
    /// <summary>
    /// This class is used to transition between scenes. This includes triggering all the things that need to happen on transition such as data persistence.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindFirstObjectByType<SceneController>();

                if (instance != null)
                    return instance;

                Create ();

                return instance;
            }
        }

        public static bool Transitioning
        {
            get { return Instance.m_Transitioning; }
        }

        protected static SceneController instance;

        public static SceneController Create ()
        {
            GameObject sceneControllerGameObject = new GameObject("SceneController");
            instance = sceneControllerGameObject.AddComponent<SceneController>();

            return instance;
        }

        public SceneTransitionDestination initialSceneTransitionDestination;

        protected Scene m_CurrentZoneScene;
        protected SceneTransitionDestination.DestinationTag m_ZoneRestartDestinationTag;
        protected PlayerInput m_PlayerInput;

        private SceneTransitionDestination m_CurrentEntrance;

        protected bool m_Transitioning;
        
        void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            
            m_PlayerInput = FindFirstObjectByType<PlayerInput>();

            if (initialSceneTransitionDestination != null)
            {
                SetEnteringGameObjectLocation(initialSceneTransitionDestination);
                ScreenFader.SetAlpha(1f);
                StartCoroutine(ScreenFader.FadeSceneIn());
                initialSceneTransitionDestination.OnReachDestination.Invoke();
                initialSceneTransitionDestination.DoNextActionsByQuests();
            }
            else
            {
                m_CurrentZoneScene = SceneManager.GetActiveScene();
                m_ZoneRestartDestinationTag = SceneTransitionDestination.DestinationTag.A;
            }
            
            AfterTransition();

        }

        void Start()
        {
            if (initialSceneTransitionDestination != null)
            {
                initialSceneTransitionDestination.DoNextActionsByQuests();
            }
        }

        protected IEnumerator Deployment(GameObject deployGameObject)
        {
            // [TKSR] BUG:至少要大于Timeline的FPS才能保证Deploy中某些设置方向的函数正常起作用;否则在相邻的帧内,这些数值会被Timeline重新设置回去???
            yield return new WaitForSeconds(1f / 20f);
            
            if (deployGameObject != null)
            {
                deployGameObject.gameObject.SetActive(true);
            }
        }
        
        
        public static void DoDeployment(GameObject deployGameObject)
        {
            Instance.StartCoroutine(Instance.Deployment(deployGameObject));
        }
        
        public static void RestartZone(bool resetHealth = true)
        {
            if(resetHealth && PlayerCharacter.PlayerInstance != null)
            {
                // PlayerCharacter.PlayerInstance.damageable.SetHealth(PlayerCharacter.PlayerInstance.damageable.startingHealth);
            }

            Instance.StartCoroutine(Instance.Transition(Instance.m_CurrentZoneScene.name, true, Instance.m_ZoneRestartDestinationTag, TransitionPoint.TransitionType.DifferentZone));
        }

        public static void RestartZoneWithDelay(float delay, bool resetHealth = true)
        {
            Instance.StartCoroutine(CallWithDelay(delay, RestartZone, resetHealth));
        }

        public static void TransitionToZone(string newSceneName, bool resetInputValuesOnTransition, SceneTransitionDestination.DestinationTag transitionDestinationTag, TransitionPoint.TransitionType transitionType, string tipsWhenSceneLoading)
        {
            Instance.StartCoroutine(Instance.Transition(newSceneName, resetInputValuesOnTransition, transitionDestinationTag, transitionType, tipsWhenSceneLoading));
        }
        
        public static void TransitionToScene(TransitionPoint transitionPoint)
        {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.resetInputValuesOnTransition, transitionPoint.transitionDestinationTag, transitionPoint.transitionType, transitionPoint.pauseSceneLoading ? transitionPoint.tipsWhenSceneLoading : null));
        }
        
        public static void TransitionToBattle(TransitionPoint transitionPoint)
        {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.battleSceneName, transitionPoint.resetInputValuesOnTransition, transitionPoint.transitionDestinationTag, transitionPoint.transitionType, transitionPoint.pauseSceneLoading ? transitionPoint.tipsWhenSceneLoading : null));
        }

        public static SceneTransitionDestination GetDestinationFromTag(SceneTransitionDestination.DestinationTag destinationTag)
        {
            return Instance.GetDestination(destinationTag);
        }
        
        public static void TransitionToScene(string strSceneName, Vector2 vecPlayerPosition, EnumFaceDirection playerDirection)
        {
            Instance.StartCoroutine(Instance.TransitionByDocument(strSceneName, vecPlayerPosition, playerDirection));
        }

        /// <summary>
        /// 场景切换时注意释放已经创建出来的对话资源
        /// </summary>
        private void ClearExistChatBubbles()
        {
            if (DialogueManager.hasInstance)
            {
                var canvas = DialogueManager.instance.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    for (int i = canvas.transform.childCount - 1; i >= 0; i--)
                    {
                        var child = canvas.transform.GetChild(i);
                        var chatBubble = child.gameObject.GetComponent<TKSRChatBubbleWithPortrait>();
                        if (chatBubble != null)
                        {
                            chatBubble.transform.SetParent(null);
                            GameObject.DestroyImmediate(chatBubble.gameObject);
                        }
                    }
                }
            }
        }

        private SceneHandle sceneHandle = null;
        protected IEnumerator Transition(string newSceneName, bool resetInputValues, SceneTransitionDestination.DestinationTag destinationTag, TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone, string strPauseSceneLoading = null)
        {
            string[] splitPath = newSceneName.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (splitPath.Length > 1)
            {
                newSceneName = splitPath[splitPath.Length - 1];
            }
            
            yield return BeforeTransition();
            
            m_Transitioning = true;
            PersistentDataManager.SaveAllData();
            
            if (m_PlayerInput == null)
                m_PlayerInput = FindFirstObjectByType<PlayerInput>();
            if (m_PlayerInput != null && m_PlayerInput.gameObject.activeInHierarchy)
                m_PlayerInput.ReleaseControl(resetInputValues);

            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading, strPauseSceneLoading));
            PersistentDataManager.ClearPersisters();
            ClearExistChatBubbles();
            var package = YooAssets.GetPackage(ResourceUtils.AB_YOO_PACKAGE);
            var sceneMode = LoadSceneMode.Single;
            var physicsMode = LocalPhysicsMode.None;
            bool suspendLoad = false;
            SceneHandle handle = package.LoadSceneAsync(newSceneName, sceneMode, physicsMode, suspendLoad);
            
            
            yield return sceneHandle;
            Debug.Log($"[TKSR] [YooAssets] LoadScene name is {sceneHandle.SceneName}");

            // [TKSR] 有时候会用Loading界面模拟剧情切换中间过滤状态
            while (!string.IsNullOrEmpty(strPauseSceneLoading))
            {
#if UNITY_EDITOR && TKSR_DEV
                if (TimelineScenarioItem.s_IsDialogAutoInTimeline)
                {
                    break;
                }
#endif
                
                // TODO:通过Touch进行Continue
#if UNITY_EDITOR || UNITY_STANDALONE
                if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
#elif UNITY_ANDROID || UNITY_IOS
                if (Input.touchCount > 1)
#endif
                {
                    break;
                }
                yield return null;
            }
            
            m_PlayerInput = FindFirstObjectByType<PlayerInput>();
            if (m_PlayerInput != null)
                m_PlayerInput.ReleaseControl(resetInputValues);

            PersistentDataManager.LoadAllData();
            SceneTransitionDestination entrance = GetDestination(destinationTag);
            SetEnteringGameObjectLocation(entrance);
            SetupNewScene(transitionType, entrance);
            
            if(entrance != null)
                entrance.OnReachDestination.Invoke();
            // yield return StartCoroutine(ScreenFader.FadeSceneIn());
            
            if (m_PlayerInput != null)
                m_PlayerInput.GainControl();

            m_Transitioning = false;

            AfterTransition();
            
            // [TKSR] 防止AfterTransition和OnReachDestination之间由于协程存在,而导致主角"闪现",所以将过渡场景延后展示
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
        }
        
        protected IEnumerator TransitionByDocument(string newSceneName, Vector2 vecPlayerPosition, EnumFaceDirection playerDirection)
        {
            string[] splitPath = newSceneName.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (splitPath.Length > 1)
            {
                newSceneName = splitPath[splitPath.Length - 1];
            }
            
            bool resetInputValues = true;
            SceneTransitionDestination.DestinationTag destinationTag =
                SceneTransitionDestination.DestinationTag.ByDocument;
            TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone;
            
            
            yield return BeforeTransition();
            
            m_Transitioning = true;
            PersistentDataManager.SaveAllData();
            
            if (m_PlayerInput == null)
                m_PlayerInput = FindFirstObjectByType<PlayerInput>();
            if (m_PlayerInput != null)
                m_PlayerInput.ReleaseControl(resetInputValues);

            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));
            PersistentDataManager.ClearPersisters();
            ClearExistChatBubbles();

            var curSceneName = SceneManager.GetActiveScene().name;
            if (curSceneName.CompareTo(newSceneName) != 0)
            {
                var package = YooAssets.GetPackage(ResourceUtils.AB_YOO_PACKAGE);
                var sceneMode = LoadSceneMode.Single;
                var physicsMode = LocalPhysicsMode.None;
                bool suspendLoad = false;
                sceneHandle = package.LoadSceneAsync(newSceneName, sceneMode, physicsMode, suspendLoad);
                yield return sceneHandle;
                Debug.Log($"[TKSR] [YooAssets] LoadScene name is {sceneHandle.SceneName}");
            }
            m_PlayerInput = FindFirstObjectByType<PlayerInput>();
            if (m_PlayerInput != null)
                m_PlayerInput.ReleaseControl(resetInputValues);

            PersistentDataManager.LoadAllData();
            SceneTransitionDestination entrance = GetDestination(destinationTag);
            var setter = entrance.gameObject.GetComponent<CharacterStateSetter>();
            setter.facingDirection = playerDirection;
            var transform1 = entrance.transform;
            transform1.position = new Vector3(vecPlayerPosition.x, vecPlayerPosition.y, 0f);
            transform1.rotation = Quaternion.identity;
            SetEnteringGameObjectLocation(entrance);
            SetupNewScene(transitionType, entrance);
            
            if(entrance != null)
                entrance.OnReachDestination.Invoke();
            
            if (m_PlayerInput != null)
                m_PlayerInput.GainControl();

            m_Transitioning = false;

            AfterTransition();
            
            // [TKSR] 防止AfterTransition和OnReachDestination之间由于协程存在,而导致主角"闪现",所以将过渡场景延后展示
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
        }

        protected SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
        {
            SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].destinationTag == destinationTag)
                    return entrances[i];
            }
            Debug.LogWarning("No entrance was found with the " + destinationTag + " tag.");
            return null;
        }

        public void SetEnteringGameObjectLocation(SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Entering Transform's location has not been set.");
                return;
            }
            Transform entranceLocation = entrance.transform;
            if (entrance.transitioningGameObject != null)
            {
                Transform enteringTransform = entrance.transitioningGameObject.transform;
                enteringTransform.position = entranceLocation.position;
                enteringTransform.rotation = entranceLocation.rotation;
            }
            m_CurrentEntrance = entrance;
        }

        protected void SetupNewScene(TransitionPoint.TransitionType transitionType, SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Restart information has not been set.");
                return;
            }
        
            if (transitionType == TransitionPoint.TransitionType.DifferentZone)
                SetZoneStart(entrance);
        }

        protected void SetZoneStart(SceneTransitionDestination entrance)
        {
            m_CurrentZoneScene = entrance.gameObject.scene;
            m_ZoneRestartDestinationTag = entrance.destinationTag;
        }

        static IEnumerator CallWithDelay<T>(float delay, Action<T> call, T parameter)
        {
            yield return new WaitForSeconds(delay);
            call(parameter);
        }

        
        #region 扩展场景管理功能

        private IEnumerator BeforeTransition()
        {
            if (YooAssets.Initialized)
            {
                var package = YooAssets.GetPackage(ResourceUtils.AB_YOO_PACKAGE);
                var operation = package.UnloadUnusedAssetsAsync();
                yield return operation;
            }
            
            var mainPlayer = FindFirstObjectByType<PlayerCharacter>(FindObjectsInactive.Include);
            if (mainPlayer != null)
            {
                mainPlayer.EnableGesture(false);
            }

            // [TKSR] Fix Bug:切换场景的时候,注意关闭所有Bark,否则会将BarkUI带入下一个场景
            var allNPCs = GameObject.FindGameObjectsWithTag("NPC");
            foreach (var npc in allNPCs)
            {
                var barkOnIdle = npc.gameObject.GetComponent<BarkOnIdle>();
                if (barkOnIdle != null)
                {
                    var barkUI = barkOnIdle.gameObject.GetComponentInChildren<TKSRChatBubbleBarkUI>();
                    if (barkUI != null)
                    {
                        barkUI.Hide();
                    }

                    barkOnIdle.enabled = false;
                }
            }
        }

        public void EnableMainPlayerInput(bool enable)
        {
            if (m_PlayerInput == null)
                m_PlayerInput = FindFirstObjectByType<PlayerInput>();
            
            if (m_PlayerInput != null)
            {
                if (enable)
                {
                    m_PlayerInput.GainControl();
                }
                else
                {
                    m_PlayerInput.ReleaseControl();
                }
            }
        }

        public PlayerCharacter EnableMainPlayerPhysicAndGesture(bool enable)
        {
            var mainPlayer = PlayerCharacter.PlayerInstance;
            if (mainPlayer == null)
            {
                mainPlayer = FindFirstObjectByType<PlayerCharacter>(FindObjectsInactive.Include);
            }
            if (mainPlayer != null)
            {
                var collider2D = mainPlayer.gameObject.GetComponent<Collider2D>();
                if (collider2D != null)
                {
                    collider2D.enabled = enable;
                }
                mainPlayer.EnableGesture(enable);
            }

            return mainPlayer;
        }
        
        /// <summary>
        /// 场景切换专场传输完成后执行的操作,一般用于播放Timeline动画
        /// </summary>
        private void AfterTransition()
        {
            var mainPlayer = EnableMainPlayerPhysicAndGesture(true);
            
            if (m_CurrentEntrance != null)
            {
                // if (m_CurrentEntrance.isAttachedToScenario && m_CurrentEntrance.timelineScenario != null)
                // {
                //     m_CurrentEntrance.timelineScenario.InitTimeline();
                //     EnableMainPlayerInput(false);
                //     EnableMainPlayerPhysicAndGesture(false);
                // }
                // else
                // {
                //     bool deployResult = DoDeploymentsQuests();
                //     Debug.Log($"[TKSR] In this current entrance, quest is satisfied and result = {deployResult}");
                // }
                
                m_CurrentEntrance.DoNextActionsByQuests();
            }
            else
            {
                // [TKSR] 如果没有指定初始传送点则说明大概率是本地调试场景,此时主角位置由GameObject决定,一般用于调试
                if (mainPlayer != null)
                {
                    mainPlayer.gameObject.SetActive(true);
                }
                
                Debug.Log($"[TKSR] Not found Entrance of this scene, maybe its a debug scene.");
            }
        }
        
        public void ResumeCurrentActiveScenario()
        {
            if (m_CurrentEntrance != null && m_CurrentEntrance.CurrentTimeline != null)
            {
                m_CurrentEntrance.CurrentTimeline.ResumeMainTimeline();
            }
            else
            {
                if (m_CurrentEntrance == null)
                {
                    Debug.LogError($"[TKSR] Resume timeline but Something wrong with m_CurrentEntrance == null");
                }
                else if (m_CurrentEntrance.CurrentTimeline == null)
                {
                    Debug.LogError($"[TKSR] Resume timeline but Something wrong with m_CurrentEntrance.CurrentTimeline == null");
                }
            }
        }

        public void ResetMainPlayerControlledStatus()
        {
            // [TKSR] TODO:一般来说进入Timeline时是不需要操作MainPlayer上的Collider2D。
            // 但是在Editor模式下如果想在Timeline编辑器模式下Seek到某一帧，会导致MainPlayer瞬移到某个TransitionPoint(一般是第一个)上,从而导致
            // 触发碰撞而进入到场景切换TransitionInternal，因此而打断Seek Timeline的操作
            var mainPlayer = EnableMainPlayerPhysicAndGesture(true);
            if (mainPlayer != null)
            {
                mainPlayer.gameObject.SetActive(true);
                mainPlayer.SetInConversation(false);
            }
            EnableMainPlayerInput(true);
        }
        
        public void ForceEntrancePlayTimeline(SceneTransitionDestination entrance)
        {
            m_CurrentEntrance = entrance;
            if(entrance != null)
                entrance.OnReachDestination.Invoke();
            AfterTransition();
        }
        
        public SceneTransitionDestination CurrentEntrance
        {
            get => m_CurrentEntrance;
        }
        #endregion
        
    }
}