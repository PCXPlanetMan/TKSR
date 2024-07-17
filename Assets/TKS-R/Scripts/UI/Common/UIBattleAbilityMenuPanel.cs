using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class UIBattleAbilityMenuPanel : MonoBehaviour
    {
        [HideInInspector]
        public AbilityMenuPanelController parentController;

        public Button btnMove;
        public Button btnAttack;
        public Button btnSkillSlot1;
        public Button btnSkillSlot2;
        public Button btnDefense;
        public Button btnRest;
        
        
        public void OnClickBtnAbilityMove()
        {
            parentController.SimSetAbilitySelection(0);
        }

        public void OnClickBtnAbilityAttack()
        {
            parentController.SimSetAbilitySelection(1);
        }

        public void OnClickBtnAbilitySkillSlot1()
        {
            
        }

        public void OnClickBtnAbilitySkillSlot2()
        {
            
        }

        public void OnClickBtnAbilityDefense()
        {
            parentController.SimSetAbilitySelection(4);
        }

        public void OnClickBtnAbilityRest()
        {
            parentController.SimSetAbilitySelection(5);
        }

        public void SetButtonEnableStatus(int index, bool enable)
        {
            if (index == 0)
            {
                btnMove.interactable = enable;
            }
        }
    }
}