using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class MovableBookCaseEffectItem : EffectItem
    {
        private readonly int m_HashCloseBookCaseEffectParam = Animator.StringToHash("Close");
        private readonly int m_HashAnimOpenedParam = Animator.StringToHash("BookCaseOpened");
        private readonly int m_HashAnimClosedParam = Animator.StringToHash("BookCaseClosed");

        
        void Awake()
        {
            effectType = TimelineEffectType.MovableBookStore;
        }

        public override void PlayEffect(string strEffectExtraParam = null)
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                bool toClose = bool.Parse(strEffectExtraParam);
                animator.SetBool(m_HashCloseBookCaseEffectParam, toClose);
            }
        }
        
        public void BookCaseClosed()
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                animator.Play(m_HashAnimClosedParam);
                animator.SetBool(m_HashCloseBookCaseEffectParam, true);
            }
        }
        
        public void BookCaseOpened()
        {
            base.PlayEffect();
            
            foreach (var effect in EffectList)
            {
                var animator = effect.gameObject.GetComponent<Animator>();
                animator.Play(m_HashAnimOpenedParam);
                animator.SetBool(m_HashCloseBookCaseEffectParam, false);
            }
        }
    }
}