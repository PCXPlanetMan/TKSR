using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class EndFacingState : BattleState 
	{
		Directions startDir;
	
		public override void Enter ()
		{
			base.Enter ();
		#if !OLD_TRPG
			// [TKSR] 忽略Face状态,直接进行下个状态
			StartCoroutine(DirectGotoNextState());
			return;
		#endif
			startDir = turn.actor.dir;
			SelectTile(turn.actor.tile.pos);
			owner.facingIndicator.gameObject.SetActive(true);
#if OLD_TRPG
			owner.facingIndicator.SetDirection(turn.actor.dir);
#else
			owner.facingIndicator.SetDirection(turn.actor);
#endif
			if (driver.Current == Drivers.Computer)
				StartCoroutine(ComputerControl());
		}
	
		public override void Exit ()
		{
			owner.facingIndicator.gameObject.SetActive(false);
			base.Exit ();
		}
		
		protected override void OnMove (object sender, InfoEventArgs<Point> e)
		{
			turn.actor.dir = e.info.GetDirection();
			Debug.Log($"[TKSR] EndFacingState OnMove dir = {turn.actor.dir}");
			turn.actor.Match();
#if OLD_TRPG
			owner.facingIndicator.SetDirection(turn.actor.dir);
#else
			owner.facingIndicator.SetDirection(turn.actor);
#endif
		}
		
		protected override void OnFire (object sender, InfoEventArgs<int> e)
		{
			switch (e.info)
			{
			case 0:
				owner.ChangeState<SelectUnitState>();
				break;
			case 1:
				turn.actor.dir = startDir;
				turn.actor.Match();
				owner.ChangeState<CommandSelectionState>();
				break;
			}
		}
	
		IEnumerator ComputerControl ()
		{
			yield return new WaitForSeconds(0.5f);
			turn.actor.dir = owner.cpu.DetermineEndFacingDirection();
			turn.actor.Match();
#if OLD_TRPG
			owner.facingIndicator.SetDirection(turn.actor.dir);
#else
			owner.facingIndicator.SetDirection(turn.actor);
#endif
			yield return new WaitForSeconds(0.5f);
			owner.ChangeState<SelectUnitState>();
		}
		
	#if !OLD_TRPG
		IEnumerator DirectGotoNextState()
		{
			yield return new WaitForEndOfFrame();
			owner.ChangeState<SelectUnitState>();
		}
	#endif
	}
}
