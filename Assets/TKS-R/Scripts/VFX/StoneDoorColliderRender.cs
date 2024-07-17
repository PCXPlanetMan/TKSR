using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class StoneDoorColliderRender : TransitionColliderRender
    {
        [SerializeField]
        private StoneDoorEffectItem stoneDoorEffect;
        
        public override void SetRenderByCollider(bool on)
        {
            base.SetRenderByCollider(on);

            if (m_isColliderOn)
            {
                stoneDoorEffect.StoneDoorClosed();
            }
            else
            {
                stoneDoorEffect.StoneDoorOpened();
            }
        }
    }
}