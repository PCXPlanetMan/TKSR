using System.Collections;
using System.Collections.Generic;
using TKSR;
using UnityEngine;

public class PatrolMainPlayer : MonoBehaviour
{
    public Transform[] patrolNodes;
    public EnumFaceDirection finishDirection;
    public SceneTransitionDestination timelineEntrance;
    
    private PlayerCharacter m_MainPlayer;

    void Awake()
    {
        m_MainPlayer = PlayerCharacter.PlayerInstance;
    }

    [SerializeField]
    private int m_curNodeIndex = -1;
    public int NodeIndex
    {
        get => m_curNodeIndex;
        set => m_curNodeIndex = value;
    }

    private void FixedUpdate()
    {
        if (NodeIndex < 0)
        {
            if (timelineEntrance != null)
            {
                if (!m_MainPlayer.IsInConversation)
                {
                    // if (timelineEntrance.timelineScenario.IsPlayableDirectorPlaying())
                    // {
                    //     timelineEntrance.timelineScenario.ResumeMainTimeline();
                    //     timelineEntrance.timelineScenario.MuteMainPlayerTracks(false);
                    //     m_MainPlayer.isInLinkingScenario = false;
                    // }
                    // else
                    {
                        SceneController.Instance.ForceEntrancePlayTimeline(timelineEntrance);
                    }
                    gameObject.SetActive(false);
                }
            }
            return;
        }

        if (patrolNodes == null || patrolNodes.Length == 0)
            return;

        if (NodeIndex >= patrolNodes.Length)
            return;
            

        var patrolFinishNode = patrolNodes[NodeIndex];
        var targetTransform = patrolFinishNode;
        if (targetTransform == null)
        {
            Debug.LogError($"[TKSR] No target finish patrol node found.");
            return;
        }
                    
        var pathTargetPos = targetTransform.position;
        Vector2 target = new Vector2(pathTargetPos.x, pathTargetPos.y);
        var current = m_MainPlayer.Controller2D.Rigidbody2D.position;
        var distance = Vector2.Distance(current, target);
        if (distance < ConstDefines.MIN_DISTANCE)
        {
            m_MainPlayer.SetMoveVector(Vector2.zero);
            m_MainPlayer.UpdateFacingDirection(finishDirection);
            Debug.Log($"{gameObject.name} patrolled to finish node.");
            NodeIndex = -1;
            if (timelineEntrance != null)
            {
                if (!m_MainPlayer.IsInConversation)
                {
                    // if (timelineEntrance.timelineScenario.IsPlayableDirectorPlaying())
                    // {
                    //     timelineEntrance.timelineScenario.ResumeMainTimeline();
                    //     timelineEntrance.timelineScenario.MuteMainPlayerTracks(false);
                    //     m_MainPlayer.isInLinkingScenario = false;
                    // }
                    // else
                    {
                        SceneController.Instance.ForceEntrancePlayTimeline(timelineEntrance);
                    }
                    gameObject.SetActive(false);
                }
            }

            m_MainPlayer.Controller2D.Teleport(target);
            
            return;
        }

        var direction = target - current;
        direction = direction.normalized;
        m_MainPlayer.SetMoveVector(direction);
        m_MainPlayer.UpdateSpriteRendererFlip(direction);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle tagStyle = new GUIStyle();
        
        if (patrolNodes != null && patrolNodes.Length > 0)
        {
            var firstNode = patrolNodes[0];
            if (firstNode != null)
            {
                Gizmos.DrawCube(firstNode.position, new Vector3(0.2f, 0.2f, 0.2f));
                    
                tagStyle.normal.background = null;
                tagStyle.normal.textColor = Color.green;
                tagStyle.fontSize = 16;
                UnityEditor.Handles.Label(firstNode.position + new Vector3(-0.4f, 0.5f, 0), firstNode.name, tagStyle);
            }
            

            Transform lastNode = firstNode;
            for (int i = 1; i < patrolNodes.Length; i++)
            {
                var node = patrolNodes[i];
                if (node != null)
                {
                    Gizmos.DrawCube(node.position, new Vector3(0.2f, 0.2f, 0.2f));
                
                    UnityEditor.Handles.Label(node.position + new Vector3(-0.4f, 0.5f, 0), node.name, tagStyle);

                    Gizmos.DrawLine(lastNode.position, node.position);
                }
            }
            
        }
    }
#endif
}
