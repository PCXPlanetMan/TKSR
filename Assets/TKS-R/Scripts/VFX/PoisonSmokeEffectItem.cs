using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class PoisonSmokeEffectItem : EffectItem
    {
        protected readonly int m_HashTeleportInEffectParam = Animator.StringToHash("InEffect");
        void Awake()
        {
            effectType = TimelineEffectType.PoisonSmoke;
        }

        public override void PlayEffect(string strEffectExtraParam = null)
        {
            bool stopEmit = false;
            bool.TryParse(strEffectExtraParam, out stopEmit);
            
            foreach (var effect in EffectList)
            {
                effect.gameObject.SetActive(true);
                var particle = effect.gameObject.GetComponentInChildren<ParticleSystem>();
                if (particle)
                {
                    if (!stopEmit)
                    {
                        particle.Play(true);
                    }
                    else
                    {
                        var emitting = particle.emission;
                        emitting.enabled = false;
                    }
                }
            }

            if (!stopEmit)
            {
                var audio = gameObject.GetComponent<AudioSource>();
                if (audio && audio.playOnAwake == false)
                {
                    audio.Play();
                }
            }
        }
    }
}