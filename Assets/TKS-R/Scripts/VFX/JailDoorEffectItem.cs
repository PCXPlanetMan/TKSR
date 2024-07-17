using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class JailDoorEffectItem : EffectItem
    {
        private readonly int m_HashJailDoorCaseEffectParam = Animator.StringToHash("Close");
        private readonly int m_HashAnimOpenedParam = Animator.StringToHash("JailDoorOpened");
        private readonly int m_HashAnimClosedParam = Animator.StringToHash("JailDoorClosed");

        
        void Awake()
        {
            effectType = TimelineEffectType.JailDoor;
        }

        public override void PlayEffect(string strEffectExtraParam = null)
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                bool toClose = bool.Parse(strEffectExtraParam);
                animator.SetBool(m_HashJailDoorCaseEffectParam, toClose);
            }
        }
        
        public void JailDoorClosed()
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                animator.Play(m_HashAnimClosedParam);
                animator.SetBool(m_HashJailDoorCaseEffectParam, true);
            }
        }
        
        public void JailDoorOpened()
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                animator.Play(m_HashAnimOpenedParam);
                animator.SetBool(m_HashJailDoorCaseEffectParam, false);
            }
        }
    }
}