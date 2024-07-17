using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UITeamGroupContent : MonoBehaviour
    {
        public TextMeshProUGUI win;
        public TextMeshProUGUI lose;
        public TextMeshProUGUI kills;
        public TextMeshProUGUI quests;
        public TextMeshProUGUI specials;
        public TextMeshProUGUI souls;
        public TextMeshProUGUI beasts;
        public TextMeshProUGUI intelligence;
        public TextMeshProUGUI morality;
        public TextMeshProUGUI courage;
        public TextMeshProUGUI medic;
        
        // Update is called once per frame
        void OnEnable()
        {
            UpdateTeamGroupInfo();
        }

        private void UpdateTeamGroupInfo()
        {
            var curDocument = DocumentDataManager.Instance.GetCurrentDocument();
            intelligence.text = curDocument.Intelligence.ToString();
            morality.text = curDocument.Morality.ToString();
            courage.text = curDocument.Courage.ToString();
            medic.text = curDocument.MedicalSkill.ToString();

            quests.text = "0%";
            specials.text = "0%";
        }
    }
}