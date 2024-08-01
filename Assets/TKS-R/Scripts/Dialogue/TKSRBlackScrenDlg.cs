using System;
using LivelyChatBubbles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class TKSRBlackScrenDlg : MonoBehaviour
	{
		[Tooltip("Text component in the tree used to display the bubble's message.")]
		public TextMeshProUGUI MessageComponent;
		
		[Multiline]
		public string MessageValue;
		public bool BindMessageValue(string value)
		{
			MessageValue = value;
			if (!MessageComponent || MessageComponent.text == value)
				return false;
			MessageComponent.text = value;
			return true;
		}

		[HideInInspector]
		public TKSRBlackScreenSubtitlePanel parentPanel;

		private Animator m_Animator;
		protected readonly int m_HashBlackPara = Animator.StringToHash("Black");
		
		void Awake()
		{
			m_Animator = GetComponent<Animator>();
		}

		void Update()
		{
			if (m_isInBlack)
			{
#if UNITY_EDITOR && TKSR_DEV
				if (TimelineScenarioItem.s_IsDialogAutoInTimeline)
				{
					HideBlack();
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
					HideBlack();
				}
			}
		}
		
		private bool m_isInBlack = false;
		public void AnimationInBlackFinished()
		{
			m_isInBlack = true;

			if (parentPanel)
			{
				parentPanel.DoTeleportMainPlayer();
			}
		}

		public void AnimationReturnFinished()
		{
			if (parentPanel)
			{
				parentPanel.ContinueWhenChatFinished();
			}
		}

		public void ShowBlack()
		{
			m_isInBlack = false;
			gameObject.SetActive(true);
			m_Animator.SetBool(m_HashBlackPara, true);
		}

		public void HideBlack()
		{
			m_isInBlack = false;
			m_Animator.SetBool(m_HashBlackPara, false);
		}
	}
}