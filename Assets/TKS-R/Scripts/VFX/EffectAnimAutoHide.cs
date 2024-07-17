using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSr
{
    public class EffectAnimAutoHide : MonoBehaviour
    {
        public void HideSelfByAnimFinished()
        {
            this.gameObject.SetActive(false);
        }
    }
}
