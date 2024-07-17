using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour 
{
	public Tile tile { get; protected set; }
	public Directions dir;

	public void Place (Tile target)
	{
		// Make sure old tile location is not still pointing to this unit
		if (tile != null && tile.content == gameObject)
			tile.content = null;
		
		// Link unit and tile references
		tile = target;
		
		if (target != null)
			target.content = gameObject;
	}

	public void Match ()
	{
		transform.localPosition = tile.center;
#if OLD_TRPG
		transform.localEulerAngles = dir.ToEuler();
#else
		DisplayAnimation(EnumBattleAnim.Idle);
#endif
	}
	
	#if !OLD_TRPG
	private Animator m_Animator;
	
	protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("Look X");
	protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("Look Y");
	protected readonly int m_HashDeadPara = Animator.StringToHash("Dead");
	protected readonly int m_HashHurtPara = Animator.StringToHash("Hurt");
	protected readonly int m_HashAttackPara = Animator.StringToHash("Attack");
	protected readonly int m_HashRunPara = Animator.StringToHash("Run");

	public enum EnumBattleAnim
	{
		Idle,
		Walk,
		Attack,
		Skill,
		Hurt,
		Dead,
	}
	
	void Awake()
	{
		var jumper = transform.Find("Jumper");
		if (jumper != null)
		{
			m_Animator = jumper.GetComponentInChildren<Animator>();
		}
	}

	public void DisplayAnimation(EnumBattleAnim battleAnim)
	{
		switch (battleAnim)
		{
			case EnumBattleAnim.Idle:
			{
				m_Animator.SetBool(m_HashRunPara, false);
				m_Animator.SetBool(m_HashAttackPara, false);
			}
				break;
			case EnumBattleAnim.Walk:
			{
				m_Animator.SetBool(m_HashRunPara, true);
				m_Animator.SetBool(m_HashAttackPara, false);
			}
				break;
			case EnumBattleAnim.Attack:
			{
				m_Animator.SetTrigger(m_HashAttackPara);
			}
				break;
			case EnumBattleAnim.Skill:
			{
				
			}
				break;
			case EnumBattleAnim.Hurt:
			{
				m_Animator.SetTrigger(m_HashHurtPara);
			}
				break;
			case EnumBattleAnim.Dead:
			{
				m_Animator.SetTrigger(m_HashDeadPara);
			}
				break;
		}
		
		Vector2 lookDirection = Vector2.zero;
		switch (dir)
		{
			case Directions.North:
			{
				lookDirection = new Vector2(-1, 1);
			}
				break;
			case Directions.South:
			{
				lookDirection = new Vector2(1, -1);
			}
				break;
			case Directions.East:
			{
				lookDirection = new Vector2(1, 1);
			}
				break;
			case Directions.West:
			{
				lookDirection = new Vector2(-1, -1);
			}
				break;
		}
		m_Animator.SetFloat(m_HashHorizontalSpeedPara, lookDirection.x);
		m_Animator.SetFloat(m_HashVerticalSpeedPara, lookDirection.y);
	}
	#endif
}