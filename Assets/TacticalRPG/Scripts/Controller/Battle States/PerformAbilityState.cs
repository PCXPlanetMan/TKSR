using UnityEngine;
using System.Collections;

public class PerformAbilityState : BattleState 
{
	public override void Enter ()
	{
		base.Enter ();
		turn.hasUnitActed = true;
		if (turn.hasUnitMoved)
			turn.lockMove = true;
		StartCoroutine(Animate());
	}
	
	IEnumerator Animate ()
	{

		yield return PerformAbilityAnimations();
		// TODO play animations, etc
		yield return null;
		ApplyAbility();
		
		if (IsBattleOver())
			owner.ChangeState<CutSceneState>();
		else if (!UnitHasControl())
			owner.ChangeState<SelectUnitState>();
		else if (turn.hasUnitMoved)
			owner.ChangeState<EndFacingState>();
		else
			owner.ChangeState<CommandSelectionState>();
	}
	
	void ApplyAbility ()
	{
		turn.ability.Perform(turn.targets);
	}
	
	bool UnitHasControl ()
	{
		return turn.actor.GetComponentInChildren<KnockOutStatusEffect>() == null;
	}
	
	#if !OLD_TRPG
	private IEnumerator PerformAbilityAnimations()
	{
		var abilitySource = turn.actor;
		var abilityTargets = turn.targets;
		var ability = turn.ability;
		if (ability.name == "Attack")
		{
			abilitySource.DisplayAnimation(Unit.EnumBattleAnim.Attack);
			yield return new WaitForSeconds(1f);
			foreach (var tile in abilityTargets)
			{
				var unit = tile.content.GetComponent<Unit>();
				unit.DisplayAnimation(Unit.EnumBattleAnim.Hurt);
			}
		}
		yield return new WaitForSeconds(1.2f);
	}
	#endif
}