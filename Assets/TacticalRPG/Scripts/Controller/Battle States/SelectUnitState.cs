using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class SelectUnitState : BattleState 
	{
		public override void Enter ()
		{
			base.Enter ();
			StartCoroutine("ChangeCurrentUnit");
		}
	
		public override void Exit ()
		{
			base.Exit ();
			statPanelController.HidePrimary();
		}
	
		IEnumerator ChangeCurrentUnit ()
		{
			owner.round.MoveNext();
			SelectTile(turn.actor.tile.pos);
			RefreshPrimaryStatPanel(pos);
			yield return null;
			owner.ChangeState<CommandSelectionState>();
		}
	
#if !OLD_TRPG	
		protected override void SelectTile(Point p)
		{
			base.SelectTile(p);
			owner.MakeCameraFollowCurrentUnit(turn.actor);
		}
#endif
	}
}
