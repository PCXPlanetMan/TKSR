using System.Collections;
using System.Collections.Generic;
using TKSR;
using UnityEngine;

namespace TKSR
{
    public class HideCharacter : NPCCharacter
    {
        private Collider2D m_hideCollider;
        protected override void Awake()
        {
            m_hideCollider = GetComponent<Collider2D>();
        }

        protected override void FixedUpdate()
        {
            
        }
        
        public override void DoConversationWithMainPlayer()
        {
            
        }

        public override void MakeNPCFaceToMainPlayer()
        {
            
        }

        public void EnableCollider(bool enable)
        {
            if (m_hideCollider != null)
            {
                m_hideCollider.enabled = enable;
            }
        }
    }
}