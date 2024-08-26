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


		private bool isNoContent = false;
		private float m_NoContentCountDown = 0f;

		public bool BindMessageValue(string value)
		{
			// [TKSR] 专门用于显示无内容的黑屏
			if (value.Length > 0) 
			{
				if (value.CompareTo("[[[BLACK]]]") == 0)
				{
					MessageValue = "";
					MessageComponent.text = "";
					isNoContent = true;
					m_NoContentCountDown = 0.5f; // 默认保持倒计时1s的纯黑屏
                    return true;
				}
			}

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
                    if (parentPanel && parentPanel.halfBlack)
                    {
						AnimationReturnFinished();
                    }
                    return;
				}
#endif
			
				// [TKSR] 如果是没有内容的黑屏,则在倒计时结束后直接Fade
				if (isNoContent)
				{
					if (m_NoContentCountDown <= 0f)
					{
                        HideBlack();
                        if (parentPanel && parentPanel.halfBlack)
                        {
                            AnimationReturnFinished();
                        }
                    }

					m_NoContentCountDown -= Time.deltaTime;
					return;
                }
				
				
#if UNITY_EDITOR || UNITY_STANDALONE
				if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
#elif UNITY_ANDROID || UNITY_IOS
				if (Input.touchCount > 1)
#else
				return;
#endif
				{
					HideBlack();
                    if (parentPanel && parentPanel.halfBlack)
                    {
                        AnimationReturnFinished();
                    }
                }
			}
		}
		
		private bool m_isInBlack = false;
		public void AnimationInBlackFinished()
		{
			m_isInBlack = true;

			if (parentPanel)
			{
				Debug.Log("[TKSR] DoTeleportMainPlayer");
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