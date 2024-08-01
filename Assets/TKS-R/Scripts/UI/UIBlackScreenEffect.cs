using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UIBlackScreenEffect : MonoBehaviour
    {
        public TextMeshProUGUI infoText;
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR && TKSR_DEV
            if (TimelineScenarioItem.s_IsDialogAutoInTimeline)
            {
                if (!string.IsNullOrEmpty(infoText.text))
                {
                    SceneController.Instance.ResumeCurrentActiveScenario();
                    infoText.text = string.Empty;
                }

                return;
            }
#endif
            
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 1)
#else
            return;
#endif
            {
                if (!string.IsNullOrEmpty(infoText.text))
                {
                    SceneController.Instance.ResumeCurrentActiveScenario();
                    infoText.text = string.Empty;
                }
            }
        }
    }
}