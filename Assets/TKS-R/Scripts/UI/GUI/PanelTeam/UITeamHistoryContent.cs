using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UITeamHistoryContent : MonoBehaviour
    {
        public TextMeshProUGUI history;

        void Awake()
        {
            var curDocument = DocumentDataManager.Instance.GetCurrentDocument();
            if (curDocument != null)
            {
                var localManager = history.GetComponent<LocalizationParamsManager>();
                localManager.SetParameterValue(ResourceUtils.I2PARAM_MAINPLAYER_FIRSTNAME, curDocument.FirstName); 
                localManager.SetParameterValue(ResourceUtils.I2PARAM_MAINPLAYER_LASTNAME, curDocument.LastName); 
            }
        }
    }
}