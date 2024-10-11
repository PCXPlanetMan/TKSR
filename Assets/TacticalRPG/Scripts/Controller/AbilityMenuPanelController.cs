using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if !OLD_TRPG
using TKSR;
#endif

namespace TacticalRPG {	
	public class AbilityMenuPanelController : MonoBehaviour 
	{
	#if OLD_TRPG
	#region Constants
		const string ShowKey = "Show";
		const string HideKey = "Hide";
		const string EntryPoolKey = "AbilityMenuPanel.Entry";
		const int MenuCount = 4;
	#endregion
	
	#region Fields / Properties
		[SerializeField] GameObject entryPrefab;
		[SerializeField] Text titleLabel;
		[SerializeField] Panel panel;
		[SerializeField] GameObject canvas;
		List<AbilityMenuEntry> menuEntries = new List<AbilityMenuEntry>(MenuCount);
		public int selection { get; private set; }
	#endregion
	
	#region MonoBehaviour
		void Awake ()
		{
			GameObjectPoolController.AddEntry(EntryPoolKey, entryPrefab, MenuCount, int.MaxValue);
		}
	
		void Start ()
		{
			panel.SetPosition(HideKey, false);
			canvas.SetActive(false);
		}
	#endregion
	
	#region Public
		public void Show (string title, List<string> options)
		{
			canvas.SetActive(true);
			Clear ();
			titleLabel.text = title;
			for (int i = 0; i < options.Count; ++i)
			{
				AbilityMenuEntry entry = Dequeue();
				entry.Title = options[i];
				menuEntries.Add(entry);
			}
			SetSelection(0);
			TogglePos(ShowKey);
		}
	
		public void Hide ()
		{
			Tweener t = TogglePos(HideKey);
			t.completedEvent += delegate(object sender, System.EventArgs e)
			{
				if (panel.CurrentPosition == panel[HideKey])
				{
					Clear();
					canvas.SetActive(false);
				}
			};
		}
	
		public void SetLocked (int index, bool value)
		{
			if (index < 0 || index >= menuEntries.Count)
				return;
	
			menuEntries[index].IsLocked = value;
			if (value && selection == index)
				Next();
		}
	
		public void Next ()
		{
			for (int i = selection + 1; i < selection + menuEntries.Count; ++i)
			{
				int index = i % menuEntries.Count;
				if (SetSelection(index))
					break;
			}
		}
	
		public void Previous ()
		{
			for (int i = selection - 1 + menuEntries.Count; i > selection; --i)
			{
				int index = i % menuEntries.Count;
				if (SetSelection(index))
					break;
			}
		}
	#endregion
	
	#region Private
		AbilityMenuEntry Dequeue ()
		{
			Poolable p = GameObjectPoolController.Dequeue(EntryPoolKey);
			AbilityMenuEntry entry = p.GetComponent<AbilityMenuEntry>();
			entry.transform.SetParent(panel.transform, false);
			entry.transform.localScale = Vector3.one;
			entry.gameObject.SetActive(true);
			entry.Reset();
			return entry;
		}
	
		void Enqueue (AbilityMenuEntry entry)
		{
			Poolable p = entry.GetComponent<Poolable>();
			GameObjectPoolController.Enqueue(p);
		}
	
		void Clear ()
		{
			for (int i = menuEntries.Count - 1; i >= 0; --i)
				Enqueue(menuEntries[i]);
			menuEntries.Clear();
		}
	
		bool SetSelection (int value)
		{
			if (menuEntries[value].IsLocked)
				return false;
			
			// Deselect the previously selected entry
			if (selection >= 0 && selection < menuEntries.Count)
				menuEntries[selection].IsSelected = false;
			
			selection = value;
			
			// Select the new entry
			if (selection >= 0 && selection < menuEntries.Count)
				menuEntries[selection].IsSelected = true;
			
			return true;
		}
	
		Tweener TogglePos (string pos)
		{
			Tweener t = panel.SetPosition(pos, true);
			t.duration = 0.5f;
			t.equation = EasingEquations.EaseOutQuad;
			return t;
		}
	#endregion
#else
		[SerializeField] UIBattleAbilityMenuPanel menuPanel;
		Canvas canvas;
	
		public int selection { get; private set; } = -1;
		
		protected readonly int m_HashDisplayPara = Animator.StringToHash("Display");
	
		private Animator menuPanelAnim;
		
		void Start()
		{
			canvas = GetComponentInChildren<Canvas>(true);
			menuPanelAnim = menuPanel.GetComponent<Animator>();
			canvas.gameObject.SetActive(false);
			menuPanel.parentController = this;
		}
		
		public void Show (string title, List<string> options)
		{
			canvas.gameObject.SetActive(true);
			menuPanelAnim.SetBool(m_HashDisplayPara, true);
		}
	
		public void Hide()
		{
			menuPanelAnim.SetBool(m_HashDisplayPara, false);
		}
		
		public void SetLocked (int index, bool value)
		{
			Debug.Log($"[TKSR] AbilityMenuPanelController:SetLocked = [{index}], {value} ");
			menuPanel.SetButtonEnableStatus(index, !value);
		}
	
		public void Next ()
		{
			Debug.Log("[TKSR] AbilityMenuPanelController:Next Not implement ");
		}
	
		public void Previous ()
		{
			Debug.Log("[TKSR] AbilityMenuPanelController:Previous Not implement ");
		}
		
		/// <summary>
		/// 点击技能菜单,模拟发送左击消息来切换状态
		/// </summary>
		/// <param name="value"></param>
		public void SimSetAbilitySelection(int value)
		{
			selection = value;
			InputController.Instance.SimOnFireEvent();
			selection = -1;
		}
	#endif
	}
}
