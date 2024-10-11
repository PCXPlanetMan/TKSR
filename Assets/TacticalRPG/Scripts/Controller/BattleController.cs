using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !OLD_TRPG
using TKSR;
using UnityEngine.Tilemaps;
#endif

namespace TacticalRPG {	
	public class BattleController : StateMachine 
	{
#if OLD_TRPG
		public CameraRig cameraRig;
#endif
		public Board board;
#if OLD_TRPG
		public LevelData levelData;
		public Transform tileSelectionIndicator;
#else
		[HideInInspector]
		public LevelData levelData;
#endif
		public Point pos;
		public Tile currentTile { get { return board.GetTile(pos); }}
		public AbilityMenuPanelController abilityMenuPanelController;
		public StatPanelController statPanelController;
#if OLD_TRPG	
		public HitSuccessIndicator hitSuccessIndicator;
		public BattleMessageController battleMessageController;
#endif
		public FacingIndicator facingIndicator;
		public Turn turn = new Turn();
		public List<Unit> units = new List<Unit>();
		public IEnumerator round;
		public ComputerPlayer cpu;
		
	#if !OLD_TRPG
		public TransitionPoint ResultTransitionPoint;
		public TransitionPoint FailedTransitionPoint;
	#endif
	
#if !OLD_TRPG
		[HideInInspector]
		public Grid gridMap;
	
		public enum EnumTileMapLayer
		{
			Base = 0,
			Target,
			Collider
		}
	
		private Tilemap m_tileMapBase;
		private Tilemap m_tileMapTarget;
		private Tilemap m_tileMapCollider;
		private TileBase m_tileTarget;
		private TileBase m_maskWalkable;
		private TileBase m_maskNotWalkable;
		private TileBase m_maskDisable;
		private TileBase m_maskSelectable;
	
		public static BattleController Instance;
	
		private AutoCameraSetup autoCamera;
		
		void Awake()
		{
			Instance = this;
	
			autoCamera = FindFirstObjectByType<AutoCameraSetup>(FindObjectsInactive.Exclude);
			
			LoadGridTilesData();
		}
		
		private void LoadGridTilesData()
		{
			gridMap = GameObject.FindFirstObjectByType<Grid>(FindObjectsInactive.Include);
			if (gridMap != null)
			{
				m_tileMapBase = gridMap.transform.GetChild((int)EnumTileMapLayer.Base).gameObject.GetComponent<Tilemap>();
				if (m_tileMapBase != null)
				{
					foreach (var position in m_tileMapBase.cellBounds.allPositionsWithin)
					{
						if (!m_tileMapBase.HasTile(position))
						{
							continue;
						}
						TileBase tile = m_tileMapBase.GetTile(position);
						Debug.Log($"[TKSR] TileMap Base Init Tile : position = {position.ToString()}, tile.name = {tile.name}");
						if (tile.name.EndsWith("Disable"))
						{
							m_maskDisable = tile;
						}
						else if (tile.name.EndsWith("NotWalkable"))
						{
							m_maskNotWalkable = tile;
						}
						else if (tile.name.EndsWith("Selectable"))
						{
							m_maskSelectable = tile;
						}
						else if (tile.name.EndsWith("Walkable"))
						{
							m_maskWalkable = tile;
						}
					}
				}
				
				m_tileMapTarget = gridMap.transform.GetChild((int)EnumTileMapLayer.Target).gameObject.GetComponent<Tilemap>();
				if (m_tileMapTarget != null)
				{
					foreach (var position in m_tileMapTarget.cellBounds.allPositionsWithin)
					{
						if (!m_tileMapTarget.HasTile(position))
						{
							continue;
						}
						TileBase tile = m_tileMapTarget.GetTile(position);
						Debug.Log($"[TKSR] TileMap Flag Init Tile : position = {position.ToString()}, tile.name = {tile.name}");
						m_tileTarget = tile;
					}
				}
				m_tileMapCollider = gridMap.transform.GetChild((int)EnumTileMapLayer.Collider).gameObject.GetComponent<Tilemap>();
				if (m_tileMapCollider != null)
				{
					levelData = ScriptableObject.CreateInstance<LevelData>();
					levelData.tiles = new List<Vector3>();
					int index = 0;
					foreach (var position in m_tileMapCollider.cellBounds.allPositionsWithin)
					{
						if (m_tileMapCollider.HasTile(position))
						{
							Debug.Log($"[TKSR] TileMap Collider XY : [{index}] position = {position.ToString()}");
							levelData.tiles.Add(position);
							index++;
						}
					}
				}
			}
		}
	
		public enum EnumTileDisplayMode
		{
			Null,
			Target,
			Walkeable,
		}
	
		public void RefreshTileStatus(EnumTileDisplayMode mode, Vector3Int tilePos, bool enable)
		{
			Tilemap targetTileMap = null;
			TileBase targetTileMask = null;
			if (mode == EnumTileDisplayMode.Target)
			{
				targetTileMap = m_tileMapTarget;
				targetTileMap.ClearAllTiles();
				targetTileMask = m_tileTarget;
			}
			else if (mode == EnumTileDisplayMode.Walkeable)
			{
				targetTileMap = m_tileMapBase;
			}
			else
			{
				Debug.LogError($"[TKSR] Error EnumTileDisplayMode : {mode}");
				return;
			}
	
			if (targetTileMap != null)
			{
				targetTileMap.SetTile(tilePos, enable ? targetTileMask : null);
			}
		}
	
		/// <summary>
		/// 刷新地图上Tile的标志状态
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="tiles"></param>
		/// <param name="enable"></param>
		public void RefreshAnyTilesStatus(EnumTileDisplayMode mode, List<Tile> tiles, bool enable)
		{
			Tilemap targetTileMap = null;
			TileBase targetTileMask = null;
			if (mode == EnumTileDisplayMode.Target)
			{
				targetTileMap = m_tileMapTarget;
				targetTileMask = m_tileTarget;
			}
			else if (mode == EnumTileDisplayMode.Walkeable)
			{
				targetTileMap = m_tileMapBase;
				targetTileMask = m_maskWalkable;
			}
			else
			{
				Debug.LogError($"[TKSR] Error EnumTileDisplayMode : {mode}");
				return;
			}
	
			if (targetTileMap != null)
			{
				targetTileMap.ClearAllTiles();
				for (int i = 0; i < tiles.Count; i ++)
				{
					var tile = tiles[i];
					var tilePos = new Vector3Int(tile.pos.x, tile.pos.y, 0);
					Debug.Log($"[TKSR] RefreshAnyTilesStatus : mode = {mode}, {enable}, i = {i}, tile.pos = {tile.pos} tile.center = {tile.center}");
					targetTileMap.SetTile(tilePos, enable ? targetTileMask : null);
				}
			}
		}
	
		public void MakeCameraFollowCurrentUnit(Unit currentUnit)
		{
			var target = currentUnit.transform.Find("Jumper/Renderer/CameraFollowTarget");
			autoCamera.UpdateBattleCameraFollower(target);
		}
	
		public void MakeCameraFollowCurrentTile(Tile currentTile)
		{
			autoCamera.UpdateBattleCameraFollower(currentTile.transform);
		}
#endif
	
		void Start ()
		{
			ChangeState<InitBattleState>();
			
#if TKSR_DEV && UNITY_EDITOR
			ConversationController.completeEvent += DebugOnCompleteOpeningAnimation;
#endif
		}
		
#if TKSR_DEV && UNITY_EDITOR
		private bool m_isBattleStarted = false;
		void OnGUI()
		{
			if (!m_isBattleStarted)
			{
				return;
			}
			
			GUIStyle  myButtonStyle = new GUIStyle(GUI.skin.button);
			myButtonStyle.fontSize = 36;
			
			if (GUI.Button(new Rect(0, Screen.height - 80, 100, 80), "胜利", myButtonStyle))
			{
				GetComponent<BaseVictoryCondition>().DebugSetBattleWin();
				if (GetComponent<BaseVictoryCondition>().Victor == Alliances.Hero)
				{
					ChangeState<EndBattleState>();
				}
			}
	
			if (GUI.Button(new Rect(120, Screen.height - 80, 100, 80), "失败", myButtonStyle))
			{
				GetComponent<BaseVictoryCondition>().DebugSetBattleLose();
				if (GetComponent<BaseVictoryCondition>().Victor == Alliances.Enemy)
				{
					ChangeState<EndBattleState>();
				}
			}
		}
		
		void DebugOnCompleteOpeningAnimation (object sender, System.EventArgs e)
		{
			if (GetComponent<BaseVictoryCondition>().Victor == Alliances.None)
			{
				m_isBattleStarted = true;
			}
		}
		
		private void OnDestroy()
		{
			ConversationController.completeEvent -= DebugOnCompleteOpeningAnimation;
		}
	#endif
	}
}
