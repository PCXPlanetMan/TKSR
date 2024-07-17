using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class StoneDoorEffectItem : EffectItem
    {
        private readonly int m_HashOpenStoneDoorEffectParam = Animator.StringToHash("Open");
        private readonly int m_HashAnimDoorClosedParam = Animator.StringToHash("DoorClosed");
        private readonly int m_HashAnimDoorOpenedParam = Animator.StringToHash("DoorOpened");

        
        void Awake()
        {
            effectType = TimelineEffectType.StoneDoor;
        }

        public override void PlayEffect(string strEffectExtraParam = null)
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                bool toOpen = bool.Parse(strEffectExtraParam);
                animator.SetBool(m_HashOpenStoneDoorEffectParam, toOpen);
            }
        }

        public void StoneDoorClosed()
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                animator.Play(m_HashAnimDoorClosedParam);
                animator.SetBool(m_HashOpenStoneDoorEffectParam, false);
            }
        }
        
        public void StoneDoorOpened()
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                animator.Play(m_HashAnimDoorOpenedParam);
                animator.SetBool(m_HashOpenStoneDoorEffectParam, true);
            }
        }
    }
}