using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public class CutSceneState : BattleState 
	{
		ConversationController conversationController;
		ConversationData data;
	
		protected override void Awake ()
		{
			base.Awake ();
			conversationController = owner.GetComponentInChildren<ConversationController>();
		}
	
		public override void Enter ()
		{
			base.Enter ();
#if OLD_TRPG
			if (IsBattleOver())
			{
				if (DidPlayerWin())
					data = Resources.Load<ConversationData>("Conversations/OutroSceneWin");
				else
					data = Resources.Load<ConversationData>("Conversations/OutroSceneLose");
			}
			else
			{
				data = Resources.Load<ConversationData>("Conversations/IntroScene");
			}
			conversationController.Show(data);
#else
			if (IsBattleOver())
			{
				if (DidPlayerWin())
					conversationController.SetWinDisplay();
				else
					conversationController.SetLoseDisplay();
				conversationController.Show(false);
			}
			else
			{
				conversationController.SetOpeningDisplay();
				conversationController.Show();
			}
#endif
		}
	
		public override void Exit ()
		{
			base.Exit ();
#if OLD_TRPG		
			if (data)
				Resources.UnloadAsset(data);
#endif
		}
	
		protected override void AddListeners ()
		{
			base.AddListeners ();
			ConversationController.completeEvent += OnCompleteConversation;
		}
	
		protected override void RemoveListeners ()
		{
			base.RemoveListeners ();
			ConversationController.completeEvent -= OnCompleteConversation;
		}
	
		protected override void OnFire (object sender, InfoEventArgs<int> e)
		{
			base.OnFire (sender, e);
#if OLD_TRPG		
			conversationController.Next();
#else
			if (e.info == 0)
			{
				if (IsBattleOver())
					owner.ChangeState<EndBattleState>();
			}
#endif
		}
	
		void OnCompleteConversation (object sender, System.EventArgs e)
		{
#if OLD_TRPG		
			if (IsBattleOver())
				owner.ChangeState<EndBattleState>();
			else
				owner.ChangeState<SelectUnitState>();
#else
			if (!IsBattleOver())
				owner.ChangeState<SelectUnitState>();
#endif
		}
	}
}
