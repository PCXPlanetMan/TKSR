using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if !OLD_TRPG
using I2.Loc;
using TKSR;
using TMPro;
#endif

namespace TacticalRPG {	
	public class StatPanel : MonoBehaviour
	{
#if OLD_TRPG
		public Panel panel;
		public Sprite allyBackground;
		public Sprite enemyBackground;
		public Image background;
		public Image avatar;
		public Text nameLabel;
		public Text hpLabel;
		public Text mpLabel;
		public Text lvLabel;
	
		public void Display (GameObject obj)
		{
			Alliance alliance = obj.GetComponent<Alliance>();
			background.sprite = alliance.type == Alliances.Enemy ? enemyBackground : allyBackground;
			// avatar.sprite = null; Need a component which provides this data
			nameLabel.text = obj.name;
			Stats stats = obj.GetComponent<Stats>();
			if (stats)
			{
				hpLabel.text = string.Format( "HP {0} / {1}", stats[StatTypes.HP], stats[StatTypes.MHP] );
				mpLabel.text = string.Format( "MP {0} / {1}", stats[StatTypes.MP], stats[StatTypes.MMP] );
				lvLabel.text = string.Format( "LV. {0}", stats[StatTypes.LVL]);
			}
		}
#else
		public Image portrait;
		public TextMeshProUGUI charName;
		public TextMeshProUGUI level;
		public TextMeshProUGUI hpValue;
		public TextMeshProUGUI mpValue;
		public RectMask2D hpProgress;
		public RectMask2D mpProgress;
	
		public void Display(GameObject obj)
		{
			Alliance alliance = obj.GetComponent<Alliance>();
			
			Stats stats = obj.GetComponent<Stats>();
			if (stats)
			{
				hpValue.text = string.Format("{0} / {1}", stats[StatTypes.HP], stats[StatTypes.MHP]);
				mpValue.text = string.Format("{0} / {1}", stats[StatTypes.MP], stats[StatTypes.MMP]);
				level.text = string.Format("{0}", stats[StatTypes.LVL]);
			}
	
			var fullWidth = hpProgress.rectTransform.rect.width;
			float hpRatio = stats[StatTypes.HP] * 1f / stats[StatTypes.MHP];
			hpRatio = Mathf.Clamp(hpRatio, 0f, 1f);
			var oldPadding = hpProgress.padding;
			oldPadding.z = (1f - hpRatio) * fullWidth; // Set RectMask2D Padding Right Value;
			hpProgress.padding = oldPadding;
			
			fullWidth = mpProgress.rectTransform.rect.width;
			float mpRatio = stats[StatTypes.MP] * 1f / stats[StatTypes.MMP];
			mpRatio = Mathf.Clamp(mpRatio, 0f, 1f);
			oldPadding = mpProgress.padding;
			oldPadding.z = (1f - mpRatio) * fullWidth; // Set RectMask2D Padding Right Value;
			mpProgress.padding = oldPadding;
			
			portrait.sprite = CharactersController.Instance.LoadCharacterPortraitSprite(obj.name);
			
			var localize = charName.GetComponent<Localize>();
			var termCharName = ResourceUtils.I2FORMAT_BATTLE_CHAR_CATEGORY + obj.name;
			localize.SetTerm(termCharName);
		}
#endif
	}
}
