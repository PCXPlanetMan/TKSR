using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !OLD_TRPG
using TKSR;
#endif

public class CommandSelectionState : BaseAbilityMenuState 
{
	public override void Enter ()
	{
		base.Enter ();
		statPanelController.ShowPrimary(turn.actor.gameObject);
		if (driver.Current == Drivers.Computer)
			StartCoroutine( ComputerTurn() );
	}

	public override void Exit ()
	{
		base.Exit ();
		statPanelController.HidePrimary();
	}

	protected override void LoadMenu ()
	{
#if OLD_TRPG		
		if (menuOptions == null)
		{
			menuTitle = "Commands";
			menuOptions = new List<string>(3);
			menuOptions.Add("Move");
			menuOptions.Add("Action");
			menuOptions.Add("Wait");
		}
#endif
		
		abilityMenuPanelController.Show(menuTitle, menuOptions);
		abilityMenuPanelController.SetLocked(0, turn.hasUnitMoved);
		abilityMenuPanelController.SetLocked(1, turn.hasUnitActed);
	}

	protected override void Confirm ()
	{
#if OLD_TRPG		
		switch (abilityMenuPanelController.selection)
		{
		case 0: // Move
			owner.ChangeState<MoveTargetState>();
			break;
		case 1: // Action
			owner.ChangeState<CategorySelectionState>();
			break;
		case 2: // Wait
			owner.ChangeState<EndFacingState>();
			break;
		}
#else
		switch (abilityMenuPanelController.selection)
		{
		case 0: // Move
			owner.ChangeState<MoveTargetState>();
			break;
		case 1: // Attack
			owner.ChangeState<AttackState>();
			break;
		case 2: // Action
			owner.ChangeState<CategorySelectionState>();
			break;
		case 4: // Defense
			owner.ChangeState<DefenseState>();
			break;
		case 5: // Wait
			owner.ChangeState<EndFacingState>();
			break;
		// [TKSR] 当selection=-1时,可以保证左击不再触发切换状态;通过Button Click+设置selection+模拟鼠标点击来实现状态切换
		default:
			Debug.Log($"[TKSR] abilityMenuPanelController.selection = {abilityMenuPanelController.selection}");
			break;
		}
#endif
	}

	protected override void Cancel ()
	{
		if (turn.hasUnitMoved && !turn.lockMove)
		{
			turn.UndoMove();
			abilityMenuPanelController.SetLocked(0, false);
			SelectTile(turn.actor.tile.pos);
		}
		else
		{
			owner.ChangeState<ExploreState>();
		}
	}

	IEnumerator ComputerTurn ()
	{
		if (turn.plan == null)
		{
			turn.plan = owner.cpu.Evaluate();
			turn.ability = turn.plan.ability;
		}

		yield return new WaitForSeconds (1f);

		if (turn.hasUnitMoved == false && turn.plan.moveLocation != turn.actor.tile.pos)
			owner.ChangeState<MoveTargetState>();
		else if (turn.hasUnitActed == false && turn.plan.ability != null)
			owner.ChangeState<AbilityTargetState>();
		else
			owner.ChangeState<EndFacingState>();
	}
}