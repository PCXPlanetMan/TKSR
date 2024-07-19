using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 提示显示NPC巡逻路径
    /// </summary>
    public class PatrolPathDrawer : MonoBehaviour
    {
        [Tooltip("Target character under patrolled.")]
        public GameObject targetCharacter;
        [Tooltip("Init Deploy Position")] 
        public Transform initDeployPosition;
        [Tooltip("Init face direction of the target character.")]
        public EnumFaceDirection targetFace;
        [Tooltip("How to run all path nodes in Patrol.")]
        public PatrolWrapMode targetPatrolMode;
        [Tooltip("Delay duration before run patrol path nodes.")]
        public float patrolDelay;
        [Tooltip("All patrol path nodes.")]
        public PatrolNode[] patrolPathNodes;
        
        public Animator animator;
        [Tooltip("If no path nodes exist, means always in static status, Can set other static animation.")]
        public bool setState;
        public string animatorStateName;
        public bool setParameters;
        public CharacterStateSetter.ParameterSetter[] parameterSetters;
        

        private void OnEnable()
        {
            for (int i = 0; i < parameterSetters.Length; i++)
                parameterSetters[i].Awake ();
            
            if (targetCharacter != null)
            {
                targetCharacter.gameObject.SetActive(true);
                NPCCharacter npc = targetCharacter.gameObject.GetComponent<NPCCharacter>();
                if (npc != null)
                {
                    npc.faceDirection = targetFace;
                    if (initDeployPosition != null)
                    {
                        npc.transform.position = initDeployPosition.position;
                    }

                    bool isHideCharacter = false;
                    if (npc.GetType() == typeof(HideCharacter))
                    {
                        Debug.Log("[TKSR] This is a Hide Character, no need to invoke some functions.");
                        isHideCharacter = true;
                        // 可交互的物体在进入部署阶段的时候,则一定是要求其SetActive(true)且可交互;
                        // 在场景中默认拜访这些可交互物体的时候可以SetActive(false)和collider2D.enabled=false
                        var hideCharacter = (HideCharacter)npc;
                        hideCharacter.EnableCollider(true);
                    }
                    
                    if (!isHideCharacter)
                        npc.UpdateFacingDirection(targetFace);
                    
                    var patrolAI = npc.GetComponent<PatrolAI>();
                    if (patrolAI == null)
                    {
                        patrolAI = npc.gameObject.AddComponent<PatrolAI>();
                    }

                    npc.PatrolAI = patrolAI;
                    npc.Usable = this.gameObject.GetComponent<Usable>();

                    patrolAI.mode = targetPatrolMode;
                    patrolAI.patrolDelay = patrolDelay;
                    patrolAI.initDirection = targetFace;
                    if (patrolPathNodes != null && patrolPathNodes.Length > 0)
                    {
                        patrolAI.patrolPathNodes = patrolPathNodes.ToList();
                        patrolAI.animStateName = null;
                        patrolAI.animStateParams = null;
                        patrolAI.ChangePatrolStatus(PatrolStatus.Static);
                    }
                    else
                    {
                        patrolAI.animStateName = animatorStateName;
                        patrolAI.animStateParams = parameterSetters;
                        if (!isHideCharacter)
                            npc.MakeCharacterStaticAnimation(animatorStateName, parameterSetters);
                    }

                    // 预先卸载已存在的ScenarioLauncher,如果有新的,则可以在ScenarioSetter中重新赋值
                    var scenarioLauncher = npc.GetComponent<ScenarioLauncher>();
                    if (scenarioLauncher != null)
                    {
                        scenarioLauncher.DestroyLauncher();
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GUIStyle tagStyle = new GUIStyle();
            Transform to = null;
            if (this.transform.childCount >= 2)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    var from = this.transform.GetChild(i);
                    if (i < this.transform.childCount - 1)
                    {
                        to = this.transform.GetChild(i + 1);
                    }
                    else
                    {
                        to = this.transform.GetChild(0);
                    }

                    Gizmos.DrawLine(from.position, to.position);

                    Gizmos.DrawSphere(from.position, 0.1f);
                    
                    tagStyle.normal.background = null;
                    tagStyle.normal.textColor = Color.magenta;
                    tagStyle.fontSize = 16;
                    UnityEditor.Handles.Label(from.position + new Vector3(-0.5f, 0.5f, 0), from.name, tagStyle);
                }
            }
        }
#endif
    }
}