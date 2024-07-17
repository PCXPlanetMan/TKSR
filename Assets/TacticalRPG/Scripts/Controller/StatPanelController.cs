using UnityEngine;
using System.Collections;
#if !OLD_TRPG
using TKSR;
#endif

public class StatPanelController : MonoBehaviour 
{
#if OLD_TRPG	
	#region Const
	const string ShowKey = "Show";
	const string HideKey = "Hide";
	#endregion

	#region Fields
	[SerializeField] StatPanel primaryPanel;
	[SerializeField] StatPanel secondaryPanel;
	
	Tweener primaryTransition;
	Tweener secondaryTransition;
	#endregion

	#region MonoBehaviour
	void Start ()
	{
		if (primaryPanel.panel.CurrentPosition == null)
			primaryPanel.panel.SetPosition(HideKey, false);
		if (secondaryPanel.panel.CurrentPosition == null)
			secondaryPanel.panel.SetPosition(HideKey, false);
	}
	#endregion

	#region Public
	public void ShowPrimary (GameObject obj)
	{
		primaryPanel.Display(obj);
		MovePanel(primaryPanel, ShowKey, ref primaryTransition);
	}

	public void HidePrimary ()
	{
		MovePanel(primaryPanel, HideKey, ref primaryTransition);
	}

	public void ShowSecondary (GameObject obj)
	{
		secondaryPanel.Display(obj);
		MovePanel(secondaryPanel, ShowKey, ref secondaryTransition);
	}

	public void HideSecondary ()
	{
		MovePanel(secondaryPanel, HideKey, ref secondaryTransition);
	}
	#endregion

	#region Private
	void MovePanel (StatPanel obj, string pos, ref Tweener t)
	{
		Panel.Position target = obj.panel[pos];
		if (obj.panel.CurrentPosition != target)
		{
			if (t != null)
				t.Stop();
			t = obj.panel.SetPosition(pos, true);
			t.duration = 0.5f;
			t.equation = EasingEquations.EaseOutQuad;
		}
	}
	#endregion
#else
	[SerializeField] StatPanel fullPanel;
	[SerializeField] StatPanel simplePanel;
	[SerializeField] private CommandPanel cmdPanel;

	protected readonly int m_HashDisplayPara = Animator.StringToHash("Display");

	private Animator fullPanelAnim;
	private Animator simplePanelAnim;
	private Animator cmdPanelAnim;
	
	Canvas canvas;
	
	void Start()
	{
		canvas = GetComponentInChildren<Canvas>(true);
		fullPanelAnim = fullPanel.GetComponent<Animator>();
		simplePanelAnim = simplePanel.GetComponent<Animator>();
		cmdPanelAnim = cmdPanel.GetComponent<Animator>();
		canvas.gameObject.SetActive(false);
	}
	
	public void ShowPrimary(GameObject obj)
	{
		canvas.gameObject.SetActive(true);

		HideCmdPanel();
		HideSecondary();
		
		Alliance alliance = obj.GetComponent<Alliance>();
		if (alliance.type == Alliances.Hero)
		{
			fullPanel.Display(obj);
			fullPanelAnim.SetBool(m_HashDisplayPara, true);
		}
		else if (alliance.type == Alliances.Enemy)
		{
			simplePanel.Display(obj);
			simplePanelAnim.SetBool(m_HashDisplayPara, true);
		}
	}

	public void HidePrimary()
	{
		fullPanelAnim.SetBool(m_HashDisplayPara, false);
		simplePanelAnim.SetBool(m_HashDisplayPara, false);
	}

	public void ShowSecondary (GameObject obj)
	{
		canvas.gameObject.SetActive(true);
		
		HideCmdPanel();
		HidePrimary();
		
		Alliance alliance = obj.GetComponent<Alliance>();
		if (alliance.type == Alliances.Hero)
		{
			fullPanel.Display(obj);
			fullPanelAnim.SetBool(m_HashDisplayPara, true);
		}
		else if (alliance.type == Alliances.Enemy)
		{
			simplePanel.Display(obj);
			simplePanelAnim.SetBool(m_HashDisplayPara, true);
		}
	}

	public void HideSecondary ()
	{
		fullPanelAnim.SetBool(m_HashDisplayPara, false);
		simplePanelAnim.SetBool(m_HashDisplayPara, false);
	}

	public void ShowCmdPanel(Unit unit)
	{
		cmdPanel.gameObject.SetActive(true);
		cmdPanelAnim.SetBool(m_HashDisplayPara, true);
		cmdPanel.Display(unit.gameObject);
	}

	public void HideCmdPanel()
	{
		cmdPanelAnim.SetBool(m_HashDisplayPara, false);
	}
#endif
}
