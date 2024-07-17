using System;
using LivelyChatBubbles;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class TKSRSelectorDlg : MonoBehaviour
	{
		[HideInInspector]
		public TKSRSelectorSubtitlePanel parentPanel;
		
		[Tooltip("Design-time positioned response buttons. (Optional if Button Template is assigned.)")]
		public StandardUIResponseButton[] buttons;
		
		public void BindResponsesValue(Response[] responses, Transform target)
		{
			ClearResponseButtons();
			for (int i = 0; i < responses.Length; i++)
			{
				var button = buttons[i];

				if (button != null)
				{
					button.response = responses[i];
					button.gameObject.SetActive(true);
					button.isVisible = true;
					button.isClickable = responses[i].enabled;
					button.target = target;
					button.SetFormattedText(responses[i].formattedText);
				}
			}
		}
		
		public void ClearResponseButtons()
		{
			if (buttons != null)
			{
				for (int i = 0; i < buttons.Length; i++)
				{
					if (buttons[i] == null) continue;
					buttons[i].Reset();
					buttons[i].isVisible = false;
					buttons[i].gameObject.SetActive(false);
				}
			}
		}
	}
}