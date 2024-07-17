using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    
    [Serializable]
    public class PatrolNode
    {
        public Transform pathTarget;
        public float pausedDuration;
    }

    [Serializable]
    public enum PatrolWrapMode
    {
        Occlusion = 0, // 从头到尾,并重新到开头,闭环
        PingPong = 1,
        Loop = 2,
        Once = 4,
    }
    
    public enum PatrolStatus
    {
        Static,
        InPatrolling,
        Conversation
    }

    public class PatrolAI : MonoBehaviour
    {
        [HideInInspector]
        public PatrolWrapMode mode;
        [HideInInspector]
        public float patrolDelay;
        [HideInInspector]
        public List<PatrolNode> patrolPathNodes = new List<PatrolNode>();
        [HideInInspector]
        public EnumFaceDirection initDirection;
        [HideInInspector] 
        public string animStateName;
        [HideInInspector]
        public CharacterStateSetter.ParameterSetter[] animStateParams;
        
        private NPCCharacter m_characterComp;
        private PatrolStatus m_patrolStatus = PatrolStatus.Static;
        private PatrolStatus m_oldPatrolStatus = PatrolStatus.Static;
        private int m_pathTargetIndex = -1;
        private float m_currentPauseTime = 0f;
        
        void Awake()
        {
            m_characterComp = GetComponent<NPCCharacter>();
            m_currentPauseTime = patrolDelay;
        }
        
        // Update is called once per frame
        void Update()
        {
            if (patrolPathNodes.Count == 0)
            {
                return;
            }
            
            if (m_patrolStatus == PatrolStatus.Static)
            {
                if (m_currentPauseTime > 0f)
                {
                    m_currentPauseTime -= Time.deltaTime;
                }
                else
                {
                    m_patrolStatus = PatrolStatus.InPatrolling;
                    if (mode == PatrolWrapMode.Occlusion)
                    {
                        m_pathTargetIndex++;
                        if (m_pathTargetIndex >= patrolPathNodes.Count)
                        {
                            m_pathTargetIndex = 0;
                        }
                    }
                    else 
                    {
                        // [TKSR] TODO: Other mode codes...
                    }
                }
            }
        }
        
        private void FixedUpdate()
        {
            if (patrolPathNodes.Count == 0)
            {
                return;
            }
            
            if (m_patrolStatus == PatrolStatus.InPatrolling)
            {
                if (m_pathTargetIndex < patrolPathNodes.Count && m_pathTargetIndex >= 0)
                {
                    var patrolNode = patrolPathNodes[m_pathTargetIndex];
                    var targetTransform = patrolNode.pathTarget;
                    if (targetTransform == null)
                    {
                        Debug.LogError($"[TKSR] Empty path patrol transform node with Index = {m_pathTargetIndex}");
                        return;
                    }
                    
                    var pathTargetPos = targetTransform.position;
                    Vector2 target = new Vector2(pathTargetPos.x, pathTargetPos.y);
                    var current = m_characterComp.Controller2D.Rigidbody2D.position;
                    var distance = Vector2.Distance(current, target);
                    if (distance <= ConstDefines.MIN_DISTANCE)
                    {
                        m_patrolStatus = PatrolStatus.Static;
                        m_characterComp.SetMoveVector(Vector2.zero);
                        m_currentPauseTime = patrolNode.pausedDuration;
                        m_characterComp.Controller2D.Teleport(target);
                        m_characterComp.Controller2D.ZeroMovement();
                        
                        Debug.Log($"{gameObject.name} patrolled to target index = {m_pathTargetIndex}, pause {m_currentPauseTime} seconds. target = {target}");
                        return;
                    }

                    var direction = target - current;
                    direction = direction.normalized;
                    m_characterComp.SetMoveVector(direction);
                }
                else
                {
                    Debug.LogError($"[TKSR] Patrol AI FixedUpdate with error path node index = {m_pathTargetIndex}");
                }
            }
        }


        public void ChangePatrolStatus(PatrolStatus status)
        {
            m_patrolStatus = status;
            
            if (m_characterComp.GetType() == typeof(HideCharacter))
            {
                return;
            }
            
            if (patrolPathNodes.Count == 0)
            {
                if (status == PatrolStatus.InPatrolling)
                {
                    m_characterComp.UpdateFacingDirection(initDirection);
                    m_characterComp.MakeCharacterStaticAnimation(animStateName, animStateParams);
                }
                else if (status == PatrolStatus.Conversation)
                {
                    if (!m_characterComp.stillKeepAnimation)
                        m_characterComp.MakeCharacterStaticIdle();
                }
            }
        }
    }
}