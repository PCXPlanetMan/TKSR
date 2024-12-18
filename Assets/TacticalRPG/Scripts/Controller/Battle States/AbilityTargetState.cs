using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public class AbilityTargetState : BattleState 
	{
		List<Tile> tiles;
		AbilityRange ar;
		
		public override void Enter ()
		{
			base.Enter ();
			ar = turn.ability.GetComponent<AbilityRange>();
			SelectTiles ();
			statPanelController.ShowPrimary(turn.actor.gameObject);
			if (ar.directionOriented)
				RefreshSecondaryStatPanel(pos);
			if (driver.Current == Drivers.Computer)
				StartCoroutine(ComputerHighlightTarget());
		}
		
		public override void Exit ()
		{
			base.Exit ();
			board.DeSelectTiles(tiles);
			statPanelController.HidePrimary();
			statPanelController.HideSecondary();
		}
		
		protected override void OnMove (object sender, InfoEventArgs<Point> e)
		{
#if OLD_TRPG
			if (ar.directionOriented)
			{
				ChangeDirection(e.info);
			}
			else
			{
				SelectTile(e.info + pos);
				RefreshSecondaryStatPanel(pos);
			}
#else
			if (ar.directionOriented)
			{
				ChangeDirection(e.info + pos - owner.turn.actor.tile.pos);
			}
			SelectTile(e.info + pos);
			RefreshSecondaryStatPanel(pos);
			RefreshPrimaryStatPanel(pos);
#endif
		}
		
		protected override void OnFire (object sender, InfoEventArgs<int> e)
		{
			if (e.info == 0)
			{
				if (ar.directionOriented || tiles.Contains(board.GetTile(pos)))
					owner.ChangeState<ConfirmAbilityTargetState>();
			}
			else
			{
				owner.ChangeState<CategorySelectionState>();
			}
		}
		
		void ChangeDirection (Point p)
		{
			Directions dir = p.GetDirection();
			if (turn.actor.dir != dir)
			{
				board.DeSelectTiles(tiles);
				turn.actor.dir = dir;
				turn.actor.Match();
				SelectTiles ();
			}
		}
		
		void SelectTiles ()
		{
			tiles = ar.GetTilesInRange(board);
			board.SelectTiles(tiles);
		}
	
		IEnumerator ComputerHighlightTarget ()
		{
			if (ar.directionOriented)
			{
				ChangeDirection(turn.plan.attackDirection.GetNormal());
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				Point cursorPos = pos;
				while (cursorPos != turn.plan.fireLocation)
				{
					if (cursorPos.x < turn.plan.fireLocation.x) cursorPos.x++;
					if (cursorPos.x > turn.plan.fireLocation.x) cursorPos.x--;
					if (cursorPos.y < turn.plan.fireLocation.y) cursorPos.y++;
					if (cursorPos.y > turn.plan.fireLocation.y) cursorPos.y--;
					SelectTile(cursorPos);
					yield return new WaitForSeconds(0.25f);
				}
			}
			yield return new WaitForSeconds(0.5f);
			owner.ChangeState<ConfirmAbilityTargetState>();
		}
		
#if !OLD_TRPG
		protected override void RefreshPrimaryStatPanel(Point p)
		{
			base.RefreshPrimaryStatPanel(p);
			
			Unit target = GetUnit(p);
			if (target == null)
			{
				statPanelController.ShowCmdPanel(owner.turn.actor);
			}
		}
#endif
	}
}
