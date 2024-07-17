using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class JailDoorColliderRender : TransitionColliderRender
    {
        [SerializeField]
        private JailDoorEffectItem jailDoorEffect;
        
        public override void SetRenderByCollider(bool on)
        {
            base.SetRenderByCollider(on);

            if (m_isColliderOn)
            {
                jailDoorEffect.JailDoorClosed();
            }
            else
            {
                jailDoorEffect.JailDoorOpened();
            }
        }
    }
}