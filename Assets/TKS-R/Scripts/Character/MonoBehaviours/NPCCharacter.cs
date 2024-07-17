using System;
using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Usable = PixelCrushers.DialogueSystem.Wrappers.Usable;

namespace TKSR
{
    public class NPCCharacter : ICharacterSpriteRender
    {
        public EnumFaceDirection faceDirection;

        // 某些情况下(例如standing/saying）需要在对话时仍旧保持动画,不需要对着主角
        public bool stillKeepAnimation = false;

        // 当对话结束后用于直接播放剧情(例如华佗自动走开)(不能用Dialogue的Event系统,因为它是一个和场景相关的事件)
        public TimelineScenarioItem sceneIndependentTimeline;
        
        protected override void Awake()
        {
            base.Awake();
            
            UpdateFacingDirection(faceDirection);
        }
        
        protected virtual void FixedUpdate()
        {
            if (!m_CharacterController2D.Collider2D.enabled)
            {
                return;
            }
            
            UpdateSpriteRendererFlip();
            
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
        
        public void OnDie()
        {
            m_Animator.SetTrigger(m_HashDeadPara);
        }

        private PatrolAI m_PatrolAI;

        public PatrolAI PatrolAI
        {
            set => m_PatrolAI = value;
        }
        
        public virtual void DoConversationWithMainPlayer()
        {
            if (m_PatrolAI != null)
            {
                m_PatrolAI.ChangePatrolStatus(PatrolStatus.Conversation);
                SetMoveVector(Vector2.zero);
            }
        }

        public virtual void MakeNPCFaceToMainPlayer()
        {
            var current = Controller2D.Rigidbody2D.position;
            var target = PlayerCharacter.PlayerInstance.Controller2D.Rigidbody2D.position;
            var direction = target - current;
            direction = direction.normalized;
            
            var faceToMainPlayer = CharacterController2D.VectorToQuadDirection(direction);
            UpdateFacingDirection(faceToMainPlayer);
        }

        public void ResumePatrolAI()
        {
            if (m_PatrolAI != null)
                m_PatrolAI.ChangePatrolStatus(PatrolStatus.InPatrolling);
        }

        /// <summary>
        /// 如果NPC在场景中Deploy后是静止且是非IDLE状态,则在对话的时候,需要强行转换为IDLE状态.
        /// </summary>
        public void MakeCharacterStaticIdle()
        {
            if (m_Animator != null)
            {
                m_Animator.SetBool(m_HashSayingParam, false);
                m_Animator.SetBool(m_HashStandingParam, false);
                m_Animator.SetBool(m_HashDrinkingParam, false);
                m_Animator.SetBool(m_HashRunPara, false);
            }
        }

        /// <summary>
        /// 如果NPC在场景中Deploy后是静止的,有可能由PatrolAI设置了非IDLE的动画.
        /// </summary>
        public void MakeCharacterStaticAnimation(string animStateName, CharacterStateSetter.ParameterSetter[] animStateParams)
        {
            if (m_Animator == null)
            {
                Debug.LogError("[TKSR] No Animator on NPCCharacter when make static animation.");
                return;
            }
                
            
            if (!string.IsNullOrEmpty(animStateName))
            {
                int hashStateName = Animator.StringToHash (animStateName);
                m_Animator.Play(hashStateName);
            }

            if (animStateParams != null)
            {
                for (int i = 0; i < animStateParams.Length; i++)
                    animStateParams[i].SetParameter(m_Animator);
            }
        }

        public Usable Usable { get; set; }

        /// <summary>
        /// 在某些对话过程中切换NPC的朝向
        /// </summary>
        /// <param name="face"></param>
        public void UpdateFacingDirectionForDialogue(int face)
        {
            if (Enum.IsDefined(typeof(EnumFaceDirection), face))
            {
                EnumFaceDirection faceDirection = (EnumFaceDirection)face;
                Debug.Log($"[TKSR] Make NPC Character face to direction = {faceDirection} when in Dialogue");
                UpdateFacingDirection(faceDirection);
                
                PlayerCharacter.PlayerInstance.AddExtraConversationNPC(this);
            }
            else
            {
                Debug.LogError($"[TKSR] Error face direction with int = {face}");
            }
        }
        
        public void UpdateFacingToMainPlayerForDialogue(bool idle = false)
        {
            EnumFaceDirection faceDirection = CharacterController2D.VectorToQuadDirection(
                PlayerCharacter.PlayerInstance.Controller2D.Rigidbody2D.position - Controller2D.Rigidbody2D.position);
            
            UpdateFacingDirection(faceDirection);
                
            PlayerCharacter.PlayerInstance.AddExtraConversationNPC(this);

            if (idle)
            {
                if (m_PatrolAI != null)
                {
                    m_PatrolAI.animStateName = null;
                
                    CharacterStateSetter.ParameterSetter[] animStateParams = new CharacterStateSetter.ParameterSetter[3];
                    animStateParams[0] = new CharacterStateSetter.ParameterSetter();
                    animStateParams[0].parameterName = s_AnimStandingName;
                    animStateParams[0].parameterType = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                    animStateParams[0].boolValue = false;
                    animStateParams[0].Awake();

                    animStateParams[1] = new CharacterStateSetter.ParameterSetter();
                    animStateParams[1].parameterName = s_AnimSayingName;
                    animStateParams[1].parameterType = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                    animStateParams[1].boolValue = false;
                    animStateParams[1].Awake();

                    animStateParams[2] = new CharacterStateSetter.ParameterSetter();
                    animStateParams[2].parameterName = s_AnimDrinkingName;
                    animStateParams[2].parameterType = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                    animStateParams[2].boolValue = false;
                    animStateParams[2].Awake();

                    m_PatrolAI.animStateParams = animStateParams;
                }
                
                MakeCharacterStaticAnimation(null, m_PatrolAI.animStateParams);
            }
        }
        
        /// <summary>
        /// 在某些对话过程中切换NPC成为Standing或者Saying动画
        /// </summary>
        /// <param name="face"></param>
        public void UpdateStandingAnimationForDialogue()
        {
            PrepareChangeToStandingAnimationForDialogue();
            MakeCharacterStaticAnimation(null, m_PatrolAI.animStateParams);
        }

        public void PrepareChangeToStandingAnimationForDialogue()
        {
            if (m_PatrolAI != null)
            {
                m_PatrolAI.animStateName = s_AnimStandingName;
                
                CharacterStateSetter.ParameterSetter[] animStateParams = new CharacterStateSetter.ParameterSetter[3];
                animStateParams[0] = new CharacterStateSetter.ParameterSetter();
                animStateParams[0].parameterName = s_AnimStandingName;
                animStateParams[0].parameterType = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                animStateParams[0].boolValue = true;
                animStateParams[0].Awake();

                animStateParams[1] = new CharacterStateSetter.ParameterSetter();
                animStateParams[1].parameterName = s_AnimSayingName;
                animStateParams[1].parameterType = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                animStateParams[1].boolValue = false;
                animStateParams[1].Awake();
                
                animStateParams[1] = new CharacterStateSetter.ParameterSetter();
                animStateParams[1].parameterName = s_AnimDrinkingName;
                animStateParams[1].parameterType = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                animStateParams[1].boolValue = false;
                animStateParams[1].Awake();

                m_PatrolAI.animStateParams = animStateParams;
            }
        }
        
        
        protected override void StandFaceToByDirection(Vector2 vecDirection)
        {
            base.StandFaceToByDirection(vecDirection);
            m_Animator.SetBool(m_HashSayingParam, false);
            m_Animator.SetBool(m_HashStandingParam, false);
            m_Animator.SetBool(m_HashDrinkingParam, false);
        }
        
        protected override void MoveFaceToByDirection(Vector2 vecDirection, float speed = 1f)
        {
            base.MoveFaceToByDirection(vecDirection, speed);
            m_Animator.SetBool(m_HashSayingParam, false);
            m_Animator.SetBool(m_HashStandingParam, false);
            m_Animator.SetBool(m_HashDrinkingParam, false);
        }
    }
}
