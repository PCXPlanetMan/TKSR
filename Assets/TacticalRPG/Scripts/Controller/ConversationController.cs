using UnityEngine;
using System;
using System.Collections;
using I2.Loc;
using TKSR;
using TMPro;
using UnityEngine.UI;

namespace TacticalRPG {	
	public class ConversationController : MonoBehaviour 
	{
	#region Events
		public static event EventHandler completeEvent;
	#endregion
	
#if OLD_TRPG
	#region Const
		const string ShowTop = "Show Top";
		const string ShowBottom = "Show Bottom";
		const string HideTop = "Hide Top";
		const string HideBottom = "Hide Bottom";
	#endregion
	
	#region Fields
		[SerializeField] ConversationPanel leftPanel;
		[SerializeField] ConversationPanel rightPanel;
	
		Canvas canvas;
		IEnumerator conversation;
		Tweener transition;
	#endregion
	
	#region MonoBehaviour
		void Start ()
		{
			canvas = GetComponentInChildren<Canvas>();
			if (leftPanel.panel.CurrentPosition == null)
				leftPanel.panel.SetPosition(HideBottom, false);
			if (rightPanel.panel.CurrentPosition == null)
				rightPanel.panel.SetPosition(HideBottom, false);
			canvas.gameObject.SetActive(false);
		}
	#endregion
	
	#region Public
		public void Show (ConversationData data)
		{
			canvas.gameObject.SetActive(true);
			conversation = Sequence (data);
			conversation.MoveNext();
		}
	
		public void Next ()
		{
			if (conversation == null || transition != null)
				return;
			
			conversation.MoveNext();
		}
	#endregion
	
	#region Private
		IEnumerator Sequence (ConversationData data)
		{
			for (int i = 0; i < data.list.Count; ++i)
			{
				SpeakerData sd = data.list[i];
	
				ConversationPanel currentPanel = (sd.anchor == TextAnchor.UpperLeft || sd.anchor == TextAnchor.MiddleLeft || sd.anchor == TextAnchor.LowerLeft) ? leftPanel : rightPanel;
				IEnumerator presenter = currentPanel.Display(sd);
				presenter.MoveNext();
	
				string show, hide;
				if (sd.anchor == TextAnchor.UpperLeft || sd.anchor == TextAnchor.UpperCenter || sd.anchor == TextAnchor.UpperRight)
				{
					show = ShowTop;
					hide = HideTop;
				}
				else
				{
					show = ShowBottom;
					hide = HideBottom;
				}
	
				currentPanel.panel.SetPosition(hide, false);
				MovePanel(currentPanel, show);
	
				yield return null;
				while (presenter.MoveNext())
					yield return null;
	
				MovePanel(currentPanel, hide);
				transition.completedEvent += delegate(object sender, EventArgs e) {
					conversation.MoveNext();
				};
	
				yield return null;
			}
	
			canvas.gameObject.SetActive(false);
			if (completeEvent != null)
				completeEvent(this, EventArgs.Empty);
		}
	
		void MovePanel (ConversationPanel obj, string pos)
		{
			transition = obj.panel.SetPosition(pos, true);
			transition.duration = 0.5f;
			transition.equation = EasingEquations.EaseOutQuad;
		}
	#endregion
#else
		protected readonly int m_HashInOutPara = Animator.StringToHash("InOut");
		protected readonly int m_HashOutPara = Animator.StringToHash("Out");
		protected readonly int m_HashInPara = Animator.StringToHash("In");
	
		
		Canvas canvas;
	
		public Image topImage;
		public Image bottomImage;
		public TextMeshProUGUI title;
		public Button screenBtn;
		
		private Animator m_Animator;
		
		
		private AudioSource m_AudioSource;
		[Header("Win/Lose Audio")]
		public AudioClip winAudio;
		public AudioClip loseAudio;
		void Awake()
		{
			m_AudioSource = gameObject.GetComponent<AudioSource> ();
			if (m_AudioSource == null)
			{
				m_AudioSource = gameObject.AddComponent<AudioSource>();
			}
			m_AudioSource.loop = false;
			m_AudioSource.volume = 1f;
			m_AudioSource.time = 0f;
		}
		
		void Start()
		{
			canvas = GetComponentInChildren<Canvas>(true);
			m_Animator = GetComponentInChildren<Animator>();
			canvas.gameObject.SetActive(false);
			m_Animator.enabled = false;
		}
	
		public void Show(bool inOpening = true)
		{
			canvas.gameObject.SetActive(true);
			m_Animator.enabled = true;
			if (inOpening)
				m_Animator.SetTrigger(m_HashInOutPara);
			else
			{
				m_Animator.SetTrigger(m_HashInPara);
			}
		}
	
		public void SetOpeningDisplay()
		{
			topImage.color = Color.black;
			bottomImage.color = Color.black;
			title.GetComponent<Localize>().SetTerm(ResourceUtils.I2FORMAT_BATTLE_OPENING);
			if (m_AudioSource)
			{
				m_AudioSource.clip = null;
			}
		}
	
		public void SetWinDisplay()
		{
			topImage.color = Color.blue;
			bottomImage.color = Color.blue;
			title.GetComponent<Localize>().SetTerm(ResourceUtils.I2FORMAT_BATTLE_WIN);
			if (m_AudioSource)
			{
				m_AudioSource.clip = winAudio;
				m_AudioSource.Play();
			}
		}
	
		public void SetLoseDisplay()
		{
			topImage.color = Color.red;
			bottomImage.color = Color.red;
			title.GetComponent<Localize>().SetTerm(ResourceUtils.I2FORMAT_BATTLE_LOSE);
			if (m_AudioSource)
			{
				m_AudioSource.clip = loseAudio;
				m_AudioSource.Play();
			}
		}
	
		public void CallbackFadeIn()
		{
			// m_Animator.SetTrigger(m_HashOutPara);
			screenBtn.gameObject.SetActive(true);
		}
	
		public void CallbackFadeOut()
		{
			// m_Animator.enabled = false;
			// canvas.gameObject.SetActive(false);
			// if (completeEvent != null)
			// 	completeEvent(this, EventArgs.Empty);
		}
	
		public void CallbackAnimFinished()
		{
			m_Animator.enabled = false;
			canvas.gameObject.SetActive(false);
			if (completeEvent != null)
				completeEvent(this, EventArgs.Empty);
		}
	
		public void OnClickWinOrLosePanel()
		{
			Debug.Log("[TKSR] Click Win/Lose Panel.");
			InputController.Instance.SimOnFireEvent();
			// [TKSR] TODO:显示战斗结算界面,在结算界面触发SimOnFireEvent
		}
#endif
	}
}
