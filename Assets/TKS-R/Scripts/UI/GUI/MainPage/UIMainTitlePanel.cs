using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TKSR
{
    public class UIMainTitlePanel : MonoBehaviour
    {
#if UNITY_EDITOR && TKSR_DEV
        public UnityEvent eventDebug;
        
        private void OnGUI()
        {
            GUIStyle  myButtonStyle = new GUIStyle(GUI.skin.button);
            myButtonStyle.fontSize = 36;
            if (GUI.Button(new Rect(0, 0, 250, 80), "调试场景", myButtonStyle))
            {
                eventDebug?.Invoke();
            }
        }
#endif
        
        public void OnClickMainTitleBtnNew()
        {
            GameUI.Instance.ShowUIPanel(EnumUIPanelType.None);

            // var defaultNewDocument = DocumentDataManager.Instance.NewDefaultDocument();
            // Debug.Log($"[TKSR] Create a new Default Document when new game with lv = {defaultNewDocument.MainRoleInfo.Level}");

            var initTransitionPoint = GameObject.FindFirstObjectByType<TransitionPoint>(FindObjectsInactive.Exclude);
            if (initTransitionPoint != null)
            {
                SceneController.TransitionToScene(initTransitionPoint);
            }
            else
            {
                Debug.LogError("[TKSR] Not found Map Void Transition Point.");
            }
        }

        public void OnClickMainTitleBtnLoad()
        {
            GameUI.Instance.ShowUIPanel(EnumUIPanelType.Load, EnumUIPanelShowMode.Pop);
        }

        public void OnClickMainTitleBtnExit()
        {
            GameUI.Instance.ShowUIPanel(EnumUIPanelType.None);
        }
    }
}
