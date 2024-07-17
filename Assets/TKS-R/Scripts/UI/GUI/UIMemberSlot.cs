using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class UIMemberSlot : MonoBehaviour
    {
        public Image head;
        
        public TextMeshProUGUI memberName;
        public TextMeshProUGUI HP;
        public TextMeshProUGUI MP;
        
        public void UpdateMemberInfo(string strCharName, int hp, int mp)
        {
            var actorDisplayName = PixelCrushers.DialogueSystem.CharacterInfo.GetLocalizedDisplayNameInDatabase(strCharName);
            memberName.text = actorDisplayName;
            
            head.sprite = CharactersController.Instance.LoadCharacterPortraitSprite(strCharName);
            
            var localManager = HP.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_MEMBER_HP, hp.ToString());
            
            localManager = MP.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_MEMBER_MP, mp.ToString());
        }
    }
}