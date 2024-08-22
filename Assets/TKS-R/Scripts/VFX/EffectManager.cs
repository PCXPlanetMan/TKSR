using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance
        {
            get { return instance; }
        }

        protected static EffectManager instance;

        void Awake()
        {
            instance = this;
        }

        private IEnumerator ConDoDelay(string strEffect, float fDelay)
        {
            DoPlayEffectByName(strEffect);
            Debug.Log($"[TSKR] Demo a delay empty effect for Test, fDelay = {fDelay}");
            yield return new WaitForSeconds(fDelay);

            SceneController.Instance.ResumeCurrentActiveScenario();
        }

        public void PlayDelayEffect(string strEffect, float delay = 0f, string strExtra = null)
        {
            Debug.Log($"[TKSR] Effect Play : \"{strEffect}\" with delay = {delay} and Extra = {strExtra}");
            if (delay > 0f)
                StartCoroutine(ConDoDelay(strEffect, delay));
            else
            {
                DoPlayEffectByName(strEffect, strExtra);
            }
        }

        private void DoPlayEffectByName(string strEffect, string strExtra = null)
        {
            var goEffect = transform.Find(strEffect);
            if (goEffect != null)
            {
                var effect = goEffect.gameObject.GetComponent<EffectItem>();
                // if (effect.effectType == TimelineEffectType.Teleport)
                // {
                //     
                // }
                // else
                // {
                //     effect.PlayEffect();
                // }
                effect.PlayEffect(strExtra);
            }
            else
            {
                Debug.LogError($"[TKSR] Not found any effect item with Name = {strEffect}");
                return;
            }
        }
        
        public void StopEffect(string strEffect)
        {
            var goEffect = transform.Find(strEffect);
            if (goEffect != null)
            {
                goEffect.gameObject.SetActive(false);
            }
        }
    }
}