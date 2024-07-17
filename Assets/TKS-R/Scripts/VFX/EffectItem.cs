using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public enum TimelineEffectType
    {
        Normal,
        Teleport,
        PoisonSmoke,
        MovableBookStore,
        StoneDoor,
        JailDoor,
    }
    
    public class EffectItem : MonoBehaviour
    {
        public TimelineEffectType effectType = TimelineEffectType.Normal;
        public Transform[] EffectList;

        public virtual void PlayEffect(string strEffectExtraParam = null)
        {
            foreach (var effect in EffectList)
            {
                effect.gameObject.SetActive(true);
                var particle = effect.gameObject.GetComponent<ParticleSystem>();
                if (particle)
                {
                    particle.Play(true);
                }
            }
            
            var audio = gameObject.GetComponent<AudioSource>();
            if (audio && audio.playOnAwake == false)
            {
                audio.Play();
            }
        }
    }
}