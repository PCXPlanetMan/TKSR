using UnityEngine;
using System.Collections;
using TKSR;

namespace TacticalRPG {	
	public class EndBattleState : BattleState 
	{
	#if !OLD_TRPG
		ConversationController conversationController;
		protected override void Awake ()
		{
			base.Awake ();
			conversationController = owner.GetComponentInChildren<ConversationController>();
		}
	#endif
		
		
		public override void Enter ()
		{
			base.Enter ();
#if OLD_TRPG	
			Application.LoadLevel(0);
#else
			Debug.Log("[TKSR] Show Result Panel when Game win or Go to Main Title when lose.");
			if (IsBattleOver())
			{
				if (DidPlayerWin())
					conversationController.SetWinDisplay();
				else
					conversationController.SetLoseDisplay();
				conversationController.Show(false);
				BackgroundMusicPlayer.Instance.Stop();
			}
#endif
		}
		
#if !OLD_TRPG
		protected override void OnFire (object sender, InfoEventArgs<int> e)
		{
			if (e.info == 0)
			{
				if (IsBattleOver())
				{
					if (DidPlayerWin())
					{
						Debug.Log("[TKSR] Battle Win, Go to next Scenario or Continue in Big World.");
						if (owner.ResultTransitionPoint == null)
						{
							Debug.LogError("[TKSR] No transition point set after battle is win.");
						}
						else
						{
							var targetTransPoint = owner.ResultTransitionPoint;
							targetTransPoint.pauseSceneLoading = false;
							targetTransPoint.tipsWhenSceneLoading = string.Empty;
							targetTransPoint.Transition();
						}
					}
					else
					{
						if (owner.FailedTransitionPoint != null)
						{
							Debug.Log("[TKSR] Battle Lose, May be scenario force Lose.");
							var targetTransPoint = owner.FailedTransitionPoint;
							targetTransPoint.pauseSceneLoading = false;
							targetTransPoint.tipsWhenSceneLoading = string.Empty;
							targetTransPoint.Transition();
						}
						else
						{
							Debug.Log("[TKSR] Battle Lose, Return to Main Title Panel.");
						}
					}
				}
				else
				{
					Debug.LogError("[TKSR] Some error happened when Battle Result UI panel is displayed but No battle Result.");
				}
			}
		}
#endif
	}
}
