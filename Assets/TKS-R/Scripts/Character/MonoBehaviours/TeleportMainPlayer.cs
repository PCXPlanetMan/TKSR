using System.Collections;
using System.Collections.Generic;
using TKSR;
using UnityEngine;

public class TeleportMainPlayer : MonoBehaviour
{
    public Transform teleportNode;
    public EnumFaceDirection finishDirection;
    public SceneTransitionDestination timelineEntrance;
    
    private PlayerCharacter m_MainPlayer;

    void Awake()
    {
        m_MainPlayer = PlayerCharacter.PlayerInstance;
    }
    
    private void FixedUpdate()
    {
        var patrolFinishNode = teleportNode;
        var targetTransform = patrolFinishNode;
        if (targetTransform == null)
        {
            Debug.LogError($"[TKSR] No target finish patrol node found.");
            return;
        }
                    
        var pathTargetPos = targetTransform.position;
        Vector2 target = new Vector2(pathTargetPos.x, pathTargetPos.y);
        var current = m_MainPlayer.Controller2D.Rigidbody2D.position;
        m_MainPlayer.Controller2D.Rigidbody2D.position = target;
        m_MainPlayer.SetMoveVector(Vector2.zero);
        m_MainPlayer.UpdateFacingDirection(finishDirection);
        
        if (timelineEntrance != null)
        {
            if (!m_MainPlayer.IsInConversation)
            {
                SceneController.Instance.ForceEntrancePlayTimeline(timelineEntrance);
            }
        }
        gameObject.SetActive(false);
    }
}
