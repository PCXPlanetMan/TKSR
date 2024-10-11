using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !OLD_TRPG
using UnityEngine.Tilemaps;
#endif

namespace TacticalRPG {	
	public abstract class BattleState : State 
	{
		protected BattleController owner;
		protected Driver driver;
#if OLD_TRPG	
		public CameraRig cameraRig { get { return owner.cameraRig; }}
#endif
		public Board board { get { return owner.board; }}
		public LevelData levelData { get { return owner.levelData; }}
#if !OLD_TRPG
		public Grid gridMap
		{
			get
			{
				return owner.gridMap;
			}
		}
#else
		public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; }}
#endif
		public Point pos { get { return owner.pos; } set { owner.pos = value; }}
		public Tile currentTile { get { return owner.currentTile; }}
		public AbilityMenuPanelController abilityMenuPanelController { get { return owner.abilityMenuPanelController; }}
		public StatPanelController statPanelController { get { return owner.statPanelController; }}
#if OLD_TRPG
		public HitSuccessIndicator hitSuccessIndicator { get { return owner.hitSuccessIndicator; }}
#endif	
		public Turn turn { get { return owner.turn; }}
		public List<Unit> units { get { return owner.units; }}
	
		protected virtual void Awake ()
		{
			owner = GetComponent<BattleController>();
		}
	
		protected override void AddListeners ()
		{
			if (driver == null || driver.Current == Drivers.Human)
			{
				InputController.moveEvent += OnMove;
				InputController.fireEvent += OnFire;
			}
		}
		
		protected override void RemoveListeners ()
		{
			InputController.moveEvent -= OnMove;
			InputController.fireEvent -= OnFire;
		}
	
		public override void Enter ()
		{
			driver = (turn.actor != null) ? turn.actor.GetComponent<Driver>() : null;
			base.Enter ();
		}
	
		protected virtual void OnMove (object sender, InfoEventArgs<Point> e)
		{
			
		}
		
		protected virtual void OnFire (object sender, InfoEventArgs<int> e)
		{
			
		}
	
		protected virtual void SelectTile (Point p)
		{
			if (pos == p || !board.tiles.ContainsKey(p))
				return;
	
			pos = p;
#if OLD_TRPG		
			tileSelectionIndicator.localPosition = board.tiles[p].center;
#else
			owner.RefreshTileStatus(BattleController.EnumTileDisplayMode.Target, new Vector3Int(board.tiles[p].pos.x, board.tiles[p].pos.y, 0), true);
#endif
		}
	
		protected virtual Unit GetUnit (Point p)
		{
			Tile t = board.GetTile(p);
			GameObject content = t != null ? t.content : null;
			return content != null ? content.GetComponent<Unit>() : null;
		}
	
		protected virtual void RefreshPrimaryStatPanel (Point p)
		{
			Unit target = GetUnit(p);
			if (target != null)
				statPanelController.ShowPrimary(target.gameObject);
			else
				statPanelController.HidePrimary();
		}
	
		protected virtual void RefreshSecondaryStatPanel (Point p)
		{
			Unit target = GetUnit(p);
			if (target != null)
				statPanelController.ShowSecondary(target.gameObject);
			else
				statPanelController.HideSecondary();
		}
	
		protected virtual bool DidPlayerWin ()
		{
			return owner.GetComponent<BaseVictoryCondition>().Victor == Alliances.Hero;
		}
		
		protected virtual bool IsBattleOver ()
		{
			return owner.GetComponent<BaseVictoryCondition>().Victor != Alliances.None;
		}
	}
}
