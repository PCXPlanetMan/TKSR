using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TacticalRPG;
using TKSR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class CommandPanel : MonoBehaviour
    {
        public Image portrait;
        public TextMeshProUGUI cmdName;
        
        public void Display(GameObject obj)
        {
            var state = BattleController.Instance.CurrentState;
            Debug.Log($"[TKSR] In CommandPanel current state = {state.GetType()}");
            string strCmdType = null;
            if (state.GetType() == typeof(MoveTargetState))
            {
                strCmdType = ResourceUtils.I2FORMAT_BATTLE_ABILITY_CATEGORY + "Move";
            }
            else if (state.GetType() == typeof(AbilityTargetState))
            {
                strCmdType = ResourceUtils.I2FORMAT_BATTLE_ABILITY_CATEGORY + "Attack";
            }
            if (strCmdType != null)
            {
                var localize = cmdName.GetComponent<Localize>();
                localize.SetTerm(strCmdType);
            }
            
            //Alliance alliance = obj.GetComponent<Alliance>();
            //Stats stats = obj.GetComponent<Stats>();
            // if (stats)
            // {
            //     hpValue.text = string.Format("{0} / {1}", stats[StatTypes.HP], stats[StatTypes.MHP]);
            //     mpValue.text = string.Format("{0} / {1}", stats[StatTypes.MP], stats[StatTypes.MMP]);
            //     level.text = string.Format("{0}", stats[StatTypes.LVL]);
            // }

            portrait.sprite = CharactersController.Instance.LoadCharacterPortraitSprite(obj.name);
        }
    }
}