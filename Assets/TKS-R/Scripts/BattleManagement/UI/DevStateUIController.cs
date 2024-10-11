using System.Collections;
using System.Collections.Generic;
using TacticalRPG;
using UnityEngine;

namespace TKSR
{
    public class DevStateUIController : MonoBehaviour
    {
        public static DevStateUIController Instance;
        
        public Canvas canvas;

        void Awake()
        {
            Instance = this;
        }
        
        public void OnClickBtnStateOK()
        {
            InputController.Instance.SimOnFireEvent();
        }

        public void OnClickBtnStateCancel()
        {
            InputController.Instance.SimOnFireEvent(1);
        }
    }
}