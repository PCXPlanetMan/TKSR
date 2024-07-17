using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class TeleportEffectItem : EffectItem
    {
        protected readonly int m_HashTeleportInEffectParam = Animator.StringToHash("InEffect");
        void Awake()
        {
            effectType = TimelineEffectType.Teleport;
        }

        public override void PlayEffect(string strEffectExtraParam = null)
        {
            base.PlayEffect(strEffectExtraParam);
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                bool inEffect = bool.Parse(strEffectExtraParam);
                animator.SetBool(m_HashTeleportInEffectParam, inEffect);
            }
        }
    }
}