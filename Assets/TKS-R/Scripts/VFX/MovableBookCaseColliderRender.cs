using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class MovableBookCaseColliderRender : TransitionColliderRender
    {
        [SerializeField]
        private MovableBookCaseEffectItem movableBookCaseEffect;
        
        public override void SetRenderByCollider(bool on)
        {
            base.SetRenderByCollider(on);

            if (m_isColliderOn)
            {
                movableBookCaseEffect.BookCaseClosed();
            }
            else
            {
                movableBookCaseEffect.BookCaseOpened();
            }
        }
    }
}