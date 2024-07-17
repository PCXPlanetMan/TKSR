using UnityEngine;
using System.Collections;

public class ExploreState : BattleState 
{
	public override void Enter ()
	{
		base.Enter ();
		RefreshPrimaryStatPanel(pos);
	}

	public override void Exit ()
	{
		base.Exit ();
		statPanelController.HidePrimary();
	}

	protected override void OnMove (object sender, InfoEventArgs<Point> e)
	{
		SelectTile(e.info + pos);
		RefreshPrimaryStatPanel(pos);
	}
	
	protected override void OnFire (object sender, InfoEventArgs<int> e)
	{
#if OLD_TRPG
		if (e.info == 0)
			owner.ChangeState<CommandSelectionState>();
#else
		// [TKSR] 在退出技能菜单而进入Explore状态时,如果右键点击则重新进入技能菜单
		if (e.info == 1)
			owner.ChangeState<CommandSelectionState>();
#endif
	}
	
#if !OLD_TRPG	
	protected override void SelectTile(Point p)
	{
		base.SelectTile(p);

		if (board.tiles.ContainsKey(p))
		{
			var tile = board.tiles[p];
			owner.MakeCameraFollowCurrentTile(tile);
		}
	}
#endif
}