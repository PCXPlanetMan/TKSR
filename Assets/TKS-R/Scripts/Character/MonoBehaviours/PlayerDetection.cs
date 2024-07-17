using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class PlayerDetection : MonoBehaviour
    {
        private PlayerCharacter m_parentPlayer;

        private void Awake()
        {
            m_parentPlayer = this.GetComponentInParent<PlayerCharacter>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.Log($"OnTriggerEnter, other = {other.name}");
            
            // var targetNPC = other.GetComponentInParent<NPCCharacter>();
            // if (targetNPC != null)
            // {
            //     Debug.Log($"OnTriggerStay2D NPC = {targetNPC.name}");
            //     bool check = m_parentPlayer.CheckTargetPatrolNPC(targetNPC);
            //     if (check)
            //     {
            //         m_parentPlayer.DoConversationWithNPC(targetNPC);
            //     }
            // }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // Debug.Log($"OnTriggerStay, other = {other.name}");
            var targetNPC = other.GetComponentInParent<NPCCharacter>();
            if (targetNPC != null)
            {
                // Debug.Log($"OnTriggerStay2D NPC = {targetNPC.name}");
            
                bool check = m_parentPlayer.CheckTargetPatrolNPC(targetNPC);
                if (check)
                {
                    m_parentPlayer.DoConversationWithNPC(targetNPC);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Debug.Log($"OnTriggerExit, other = {other.name}");
        }
    }
}
