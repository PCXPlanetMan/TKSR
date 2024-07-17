using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class GUIInputNameController : MonoBehaviour
    {
        public TMP_InputField inputFirstName;
        public TMP_InputField inputLastName;
        
        public void OnClickBtnInputNameCancel()
        {
            inputFirstName.text = string.Empty;
            inputLastName.text = string.Empty;
        }

        public void OnClickBtnInputNameOK()
        {
            if (string.IsNullOrEmpty(inputFirstName.text))
            {
                Debug.Log("[TKSR] There is no first name input.");
                return;
            }

            if (string.IsNullOrEmpty(inputLastName.text))
            {
                Debug.Log("[TKSR] There is no last name input.");
                return;
            }

            DocumentDataManager.Instance.UpdateMainRoleName(inputFirstName.text, inputLastName.text);

            SceneController.Instance.ResumeCurrentActiveScenario();
            
            GameUI.Instance.DetachGUIInputNamePanel();
        }
    }
}