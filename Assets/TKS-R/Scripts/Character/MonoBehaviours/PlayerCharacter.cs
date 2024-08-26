using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using PixelCrushers.DialogueSystem;
using TKSRPlayables;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static TKSRPlayables.FaceParam;

namespace TKSR
{
    public class PlayerCharacter : ICharacterSpriteRender
    {
        static protected PlayerCharacter s_PlayerInstance;
        static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }

        public InventoryController inventoryController
        {
            get { return m_InventoryController; }
        }
        
        protected InventoryController m_InventoryController;

        protected Checkpoint m_LastCheckpoint = null;
        protected Vector2 m_StartingPosition = Vector2.zero;
        protected bool m_StartingFacingLeft = false;
        
        private TapGestureRecognizer oneFingerTap;
        private TapGestureRecognizer twoFingersTap;
        
        // MonoBehaviour Messages - called by Unity internally.
        protected override void Awake()
        {
            s_PlayerInstance = this;
            
            base.Awake();
            
            m_InventoryController = GetComponent<InventoryController>();
            
            oneFingerTap = new TapGestureRecognizer();
            oneFingerTap.MinimumNumberOfTouchesToTrack = oneFingerTap.MaximumNumberOfTouchesToTrack = 1;
            oneFingerTap.StateUpdated += TapGestureCallback;
            twoFingersTap = new TapGestureRecognizer();
            twoFingersTap.MinimumNumberOfTouchesToTrack = twoFingersTap.MaximumNumberOfTouchesToTrack = 2;
            twoFingersTap.StateUpdated += TapGestureCallback;
            
            oneFingerTap.RequireGestureRecognizerToFail = twoFingersTap;
        }

        private void OnDestroy()
        {
            s_PlayerInstance = null;
        }

        private void TapGestureCallback(DigitalRubyShared.GestureRecognizer tapGesture)
        {
            if (tapGesture.State == GestureRecognizerState.Ended)
            {
                int touchCount = (tapGesture as TapGestureRecognizer).TapTouches.Count;
                Debug.LogFormat("TapGestureCallback touchCount: {0}", touchCount);
                if (touchCount == 1)
                {
                    Vector3 vecScreen = new Vector3(tapGesture.FocusX, tapGesture.FocusY, 0);
                    //string msg = string.Format("Single tap at {0},{1}", tapGesture.FocusX, tapGesture.FocusY);
                    //Debug.Log(msg);
                    //var hitPoint = Camera.main.ScreenToWorldPoint(vecScreen);
                    
                    var ray = Camera.main.ScreenPointToRay(vecScreen);
                    RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("NPC"));
                    if (rayHit.collider != null)
                    {
                        Debug.Log($"[TKSR] Click the game object = {rayHit.transform.name}");
                        m_targetPatrolNPC = rayHit.collider.gameObject.GetComponent<NPCCharacter>();
                        // TODO:需要在主角和NPC发生Collider碰撞后,才应该将NPC面向MainPlayer,否则很容易出现MainPlayer朝点击点移动过程中不和NPC发生碰撞而NPC已经改变了朝向的情况
                        m_targetPatrolNPC.DoConversationWithMainPlayer();
                    }
                    else
                    {
                        if (m_targetPatrolNPC != null)
                        {
                            m_targetPatrolNPC.ResumePatrolAI();
                        }
                        m_targetPatrolNPC = null;
                    }
                }
                else if (touchCount == 2)
                {
                    GameUI.Instance.ShowUIPanel(EnumUIPanelType.Info);
                }
            }
        }

        private void OnEnable()
        {
            EnableGesture(true);
        }

        private void OnDisable()
        {
            EnableGesture(false);
        }

        void Start()
        {
            m_StartingPosition = transform.position;
            m_StartingFacingLeft = GetSpriteRenderFlipLeft() < 0.0f;
        }

        private NPCCharacter m_targetPatrolNPC;
        
        void Update()
        {
            // if (PlayerInput.Instance.Pause.Down)
            // {
            //     if (!m_InPause)
            //     {
            //         if (ScreenFader.IsFading)
            //             return;
            //
            //         PlayerInput.Instance.ReleaseControl(false);
            //         PlayerInput.Instance.Pause.GainControl();
            //         m_InPause = true;
            //         Time.timeScale = 0;
            //         UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIMenus", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            //     }
            //     else
            //     {
            //         Unpause();
            //     }
            // }
        }

        private bool m_isPatrolling = false;
        private Vector2 m_vecPatrolEndNode = Vector2.zero;

        // private bool m_isInLinkingScenario = false;
        //
        // public bool isInLinkingScenario
        // {
        //     get => m_isInLinkingScenario;
        //     set => m_isInLinkingScenario = value;
        // }
        
        void FixedUpdate()
        {
            if (m_isInConversation || m_isInUIStatus || !m_CharacterController2D.Collider2D.enabled)
            {
                if (!m_CharacterController2D.Collider2D.enabled)
                {
                    return;
                }
            }
            else
            {
                if (PlayerInput.Instance.inputType == InputComponent.InputType.Touch)
                {
                    if (PlayerInput.Instance.Touch.ReceivingInput)
                    {
                        var mousePosition = PlayerInput.Instance.Touch.Value;
                        var worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
                        // 点击NPC进行对话的时候,不能依据鼠标点击位置进行寻路(例如从NPC头顶向NPC移动的时候很容易造成无法进入NPC脚底的Collider触发对话),
                        // 而是直接设定目标NPC的位置,这样才能保证主角正确的移动到NPC脚底的Collider触发对话
                        if (m_targetPatrolNPC != null)
                        {
                            worldPos = m_targetPatrolNPC.transform.position;
                        }
                        
                        Vector2 targetPosition = new Vector2(worldPos.x, worldPos.y);
                        m_isPatrolling = true;
                        m_vecPatrolEndNode = targetPosition;
                    }

                    if (m_isPatrolling)
                    {
                        var current = m_CharacterController2D.Rigidbody2D.position;
                        var distance = Vector2.Distance(current, m_vecPatrolEndNode);
                        if (distance < .1f)
                        {
                            m_isPatrolling = false;
                        }
                        else
                        {
                            var direction = m_vecPatrolEndNode - current;
                            direction = direction.normalized;

                            UpdateSpriteRendererFlip(direction);
                            SetMoveVector(direction);
                        }
                    }
                    else
                    {
                        SetMoveVector(Vector2.zero);
                    }
                }
                else if (PlayerInput.Instance.inputType == InputComponent.InputType.MouseAndKeyboard)
                {
                    UpdateSpriteRendererFlip();
                    Movement2D(true);
                }
            }
            
            Vector2 vecMoveDist = m_MoveVector * maxSpeed * Time.deltaTime;
            m_CharacterController2D.Move(vecMoveDist);
            if (!Mathf.Approximately(vecMoveDist.x, 0.0f) || !Mathf.Approximately(vecMoveDist.y, 0.0f))
            {
                lookDirection.Set(vecMoveDist.x, vecMoveDist.y);
                lookDirection.Normalize();
                m_Animator.SetBool(m_HashRunPara, true);
            }
            else
            {
                m_Animator.SetBool(m_HashRunPara, false);

            }
            
            m_Animator.SetFloat(m_HashHorizontalSpeedPara, lookDirection.x);
            m_Animator.SetFloat(m_HashVerticalSpeedPara, lookDirection.y);
        }
        
        public void Unpause()
        {
            //if the timescale is already > 0, we 
            if (Time.timeScale > 0)
                return;

            StartCoroutine(UnpauseCoroutine());
        }

        protected IEnumerator UnpauseCoroutine()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("UIMenus");
            PlayerInput.Instance.GainControl();
            //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
            //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
        }

        private void Movement2D(bool useInput)
        {
            Vector2 inputVector = new Vector2(PlayerInput.Instance.Horizontal.Value,
                PlayerInput.Instance.Vertical.Value);
            
            if (inputVector.x * inputVector.y != 0)
            {
                inputVector *= 0.7f;
            }
            Vector2 desiredVector = useInput ? inputVector : Vector2.zero;
            m_MoveVector = desiredVector;
        }

        public void OnDie()
        {
            m_Animator.SetTrigger(m_HashDeadPara);

            StartCoroutine(DieRespawnCoroutine(true, false));
        }

        IEnumerator DieRespawnCoroutine(bool resetHealth, bool useCheckPoint)
        {
            PlayerInput.Instance.ReleaseControl(true);
            yield return new WaitForSeconds(1.0f); //wait one second before respawing
            yield return StartCoroutine(ScreenFader.FadeSceneOut(useCheckPoint ? ScreenFader.FadeType.Black : ScreenFader.FadeType.GameOver));
            if(!useCheckPoint)
                yield return new WaitForSeconds (2f);
            Respawn(resetHealth, useCheckPoint);
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            PlayerInput.Instance.GainControl();
        }
        
        public void TeleportToColliderBottom()
        {
            Vector2 colliderBottom = m_CharacterController2D.Rigidbody2D.position;
            m_CharacterController2D.Teleport(colliderBottom);
        }

        public void PlayFootstep()
        {
            var footstepPosition = transform.position;
            footstepPosition.z -= 1;
        }

        public void Respawn(bool resetHealth, bool useCheckpoint)
        {
            //we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
            m_Animator.ResetTrigger(m_HashHurtPara);
            m_Animator.SetTrigger(m_HashRespawnPara);

            if (useCheckpoint && m_LastCheckpoint != null)
            {
                UpdateSpriteRendererFlip(m_LastCheckpoint.respawnFacingLeft);
                GameObjectTeleporter.Teleport(gameObject, m_LastCheckpoint.transform.position);
            }
            else
            {
                UpdateSpriteRendererFlip(m_StartingFacingLeft);
                GameObjectTeleporter.Teleport(gameObject, m_StartingPosition);
            }
        }

        public void SetCheckpoint(Checkpoint checkpoint)
        {
            m_LastCheckpoint = checkpoint;
        }
        
        public bool CheckTargetPatrolNPC(NPCCharacter npc)
        {
            if (m_targetPatrolNPC != null && npc != null)
            {
                return m_targetPatrolNPC == npc;
            }

            return false;
        }

        private bool m_isInConversation = false;
        private NPCCharacter m_targetConversationNPC = null;
        private List<NPCCharacter> m_extraConversationNPCs = new List<NPCCharacter>();

        public void PrepareForPlayTimeline()
        {
            m_isPatrolling = false;
            m_targetPatrolNPC = null;
            SetMoveVector(Vector2.zero);
            SetInConversation(false);
        }
        
        public void DoConversationWithNPC(NPCCharacter npc)
        {
            var scenarioLauncher = npc.GetComponent<ScenarioLauncher>();
            if (scenarioLauncher != null)
            {
                Debug.Log("[TKSR] The NPC has some scenario timeline to launch.");
                SetInConversation(true);
                m_isPatrolling = false;
                m_targetPatrolNPC = null;
                m_targetConversationNPC = npc;
                SetMoveVector(Vector2.zero);
                
                SetInConversation(false);
                
                if (!IsInConversation)
                {
                    if (scenarioLauncher.TryToChangeQuestsStatus())
                    {
                        SceneController.Instance.CurrentEntrance.DoNextActionsByQuests();
                    }
                    scenarioLauncher.DestroyLauncher();
                }
                return;
            }
            
            SetInConversation(true);
            m_isPatrolling = false;
            m_targetPatrolNPC = null;
            m_targetConversationNPC = npc;
            SetMoveVector(Vector2.zero);
            // 调整MainPlayer在最终对话的时候,朝向NPC
            if (m_targetConversationNPC != null)
            {
                if (!m_targetConversationNPC.stillKeepAnimation)
                {
                    Vector3 worldPos = m_targetConversationNPC.transform.position;
                    Vector2 targetPosition = new Vector2(worldPos.x, worldPos.y);
                
                    var current = m_CharacterController2D.Rigidbody2D.position;
                    var direction = targetPosition - current;
                    direction = direction.normalized;

                    var enumFaceDirection = CharacterController2D.VectorToOctDirection(direction);
                    UpdateFacingDirection(enumFaceDirection);

                    m_targetConversationNPC.MakeNPCFaceToMainPlayer();
                }
            }

            if (m_targetConversationNPC != null && m_targetConversationNPC.Usable != null)
                m_targetConversationNPC.Usable.gameObject.SendMessage("OnUse", this.transform, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 如果是多人参与的对话,在对话过程中有其他人改变了默认朝向,则可以在对话结束后恢复默认朝向.
        /// </summary>
        /// <param name="npc"></param>
        public void AddExtraConversationNPC(NPCCharacter npc)
        {
            if (m_targetConversationNPC == npc)
            {
                return;
            }
            
            m_extraConversationNPCs.Add(npc);
        }
        
        public void SetInConversation(bool enable)
        {
            m_isInConversation = enable;
            EnableGesture(!enable);
            if (!enable)
            {
                if (m_targetConversationNPC != null)
                {
                    if (m_targetConversationNPC.sceneIndependentTimeline != null)
                    {
                        AddLogOutTimelineScenario(m_targetConversationNPC.sceneIndependentTimeline);
                    }
                    
                    m_targetConversationNPC.ResumePatrolAI();
                    m_targetConversationNPC = null;
                }

                if (m_extraConversationNPCs != null && m_extraConversationNPCs.Count > 0)
                {
                    foreach (var extraNPC in m_extraConversationNPCs)
                    {
                        extraNPC.ResumePatrolAI();
                    }
                    m_extraConversationNPCs.Clear();
                }
            }
        }

        public void EnableGesture(bool enable)
        {
            if (enable)
            {
                FingersScript.Instance.AddGesture(oneFingerTap);
                FingersScript.Instance.AddGesture(twoFingersTap);
            }
            else
            {
                if (FingersScript.HasInstance)
                {
                    FingersScript.Instance.RemoveGesture(oneFingerTap);
                    FingersScript.Instance.RemoveGesture(twoFingersTap);
                }
            }
        }
        
        public bool IsInConversation => m_isInConversation;

        /// <summary>
        /// 通过当前的动画状态获取角色的方向
        /// </summary>
        /// <returns></returns>
        public EnumFaceDirection GetCurrentFaceDirectionByAnimation()
        {
            var clipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
            float maxWeight = 0f;
            AnimatorClipInfo maxWeightClip = clipInfo[0];
            foreach (var clip in clipInfo)
            {
                if (clip.weight > maxWeight)
                {
                    maxWeight = clip.weight;
                    maxWeightClip = clip;
                }
            }
            
            var animName = maxWeightClip.clip.name;
            EnumFaceDirection resultFaceDirection = EnumFaceDirection.N;
            if (animName.EndsWith("Up"))
            {
                resultFaceDirection = EnumFaceDirection.N;
            }
            else if (animName.EndsWith("Down"))
            {
                resultFaceDirection = EnumFaceDirection.S;
            }
            else if (animName.EndsWith("Right"))
            {
                bool isSpriteLeft = spriteRenderer.flipX;
                if (animName.EndsWith("UpRight"))
                {
                    if (isSpriteLeft)
                    {
                        resultFaceDirection = EnumFaceDirection.NW;
                    }
                    else
                    {
                        resultFaceDirection = EnumFaceDirection.NE;
                    }
                }
                else if (animName.EndsWith("DownRight"))
                {
                    if (isSpriteLeft)
                    {
                        resultFaceDirection = EnumFaceDirection.SW;
                    }
                    else
                    {
                        resultFaceDirection = EnumFaceDirection.SE;
                    }
                }
                else
                {
                    if (isSpriteLeft)
                    {
                        resultFaceDirection = EnumFaceDirection.W;
                    }
                    else
                    {
                        resultFaceDirection = EnumFaceDirection.E;
                    }
                }
            }
            else
            {
                Debug.LogError("[TKSR] Error direction from Animation");
            }
            
            return resultFaceDirection;
        }

        private bool m_isInUIStatus = false;
        public void PausePlayer(bool pause)
        {
            m_isInUIStatus = pause;
            EnableGesture(!pause);
        }
        
        public void OnConversationStart(Transform actor)
        {
            Debug.Log(string.Format("Starting conversation with {0}", actor.name));
        }

        public void OnConversationLine(Subtitle subtitle)
        {
            if (subtitle == null || subtitle.speakerInfo == null || subtitle.formattedText == null ||
                subtitle.speakerInfo.transform == null)
            {
                Debug.LogWarning("Some Subtitle data is empty?");
            }
            Debug.Log(string.Format("{0}: {1}", subtitle.speakerInfo.transform.name, subtitle.formattedText.text));
        }

        public void OnConversationEnd(Transform actor)
        {
            Debug.Log(string.Format("Ending conversation with {0}", actor.name));
            SetInConversation(false);

            TryToLaunchLogOutTimelineScenario();
        }
        
        private TimelineScenarioItem m_lastLogOutTimeline;
        /// <summary>
        /// 当某些对话结束后,需要将对象NPC移动走出当前场景(例如华佗出城/长安城区天丹灵骗子出城等);
        /// 此时需要在对话最后一项设置需要强制执行的Timeline
        /// </summary>
        /// <param name="item"></param>
        public void AddLogOutTimelineScenario(TimelineScenarioItem item)
        {
            m_lastLogOutTimeline = item;
        }

        /// <summary>
        /// 在对话结束的回调中尝试播放可能的LogOut剧情
        /// </summary>
        private void TryToLaunchLogOutTimelineScenario()
        {
            if (m_lastLogOutTimeline != null)
            {
                PrepareForPlayTimeline();
                SceneController.Instance.CurrentEntrance.CurrentTimeline = m_lastLogOutTimeline;
                m_lastLogOutTimeline.InitTimeline();
                SceneController.Instance.EnableMainPlayerInput(false);
                SceneController.Instance.EnableMainPlayerPhysicAndGesture(false);
            }

            m_lastLogOutTimeline = null;
        }


        public void StandFaceToTargetForDlgEvent(Transform target)
        {
            base.StandFaceToTarget(new FaceParam(FaceType.ToTransform, target));
        }
    }
}
