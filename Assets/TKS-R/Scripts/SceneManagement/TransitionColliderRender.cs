using System;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    public class TransitionColliderRender : MonoBehaviour
    {
        protected bool m_isColliderOn = false;
        public virtual void SetRenderByCollider(bool colliderEnabled)
        {
            m_isColliderOn = colliderEnabled;

            var spriteRender = GetComponent<SpriteRenderer>();
            if (spriteRender != null)
            {
                spriteRender.enabled = colliderEnabled;
            }
        }
    }
}