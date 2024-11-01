using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if !OLD_TRPG
using UnityEngine.Tilemaps;
#endif

namespace TacticalRPG {	
	public class Board : MonoBehaviour 
	{
	#region Fields / Properties
#if OLD_TRPG
		[SerializeField] GameObject tilePrefab;
#endif
		public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();
		public Point min { get { return _min; }}
		public Point max { get { return _max; }}
		Point _min;
		Point _max;
		Point[] dirs = new Point[4]
		{
			new Point(0, 1),
			new Point(0, -1),
			new Point(1, 0),
			new Point(-1, 0)
		};
		Color selectedTileColor = new Color(0, 1, 1, 1);
		Color defaultTileColor = new Color(1, 1, 1, 1);
	#endregion
	
	#region Public
#if OLD_TRPG	
		public void Load (LevelData data)
		{
			_min = new Point(int.MaxValue, int.MaxValue);
			_max = new Point(int.MinValue, int.MinValue);
			
			for (int i = 0; i < data.tiles.Count; ++i)
			{
				GameObject instance = Instantiate(tilePrefab) as GameObject;
				instance.transform.SetParent(transform);
				Tile t = instance.GetComponent<Tile>();
				t.Load(data.tiles[i]);
				tiles.Add(t.pos, t);
				
				_min.x = Mathf.Min(_min.x, t.pos.x);
				_min.y = Mathf.Min(_min.y, t.pos.y);
				_max.x = Mathf.Max(_max.x, t.pos.x);
				_max.y = Mathf.Max(_max.y, t.pos.y);
			}
		}
#else	
		public void Load (LevelData data, Grid gridMap)
		{
			_min = new Point(int.MaxValue, int.MaxValue);
			_max = new Point(int.MinValue, int.MinValue);
			
			for (int i = 0; i < data.tiles.Count; ++i)
			{
				GameObject instance = new GameObject($"Tile[{i}]_({data.tiles[i].x},{data.tiles[i].y})");
				instance.transform.SetParent(transform);
				Tile t = instance.GetComponent<Tile>();
				if (t == null)
				{
					t = instance.AddComponent<Tile>();
				}
				t.Load(data.tiles[i], gridMap);
				tiles.Add(t.pos, t);
				
				_min.x = Mathf.Min(_min.x, t.pos.x);
				_min.y = Mathf.Min(_min.y, t.pos.y);
				_max.x = Mathf.Max(_max.x, t.pos.x);
				_max.y = Mathf.Max(_max.y, t.pos.y);
			}
		}
#endif
	
		public Tile GetTile (Point p)
		{
			return tiles.ContainsKey(p) ? tiles[p] : null;
		}
	
		public List<Tile> Search (Tile start, Func<Tile, Tile, bool> addTile)
		{
			List<Tile> retValue = new List<Tile>();
			retValue.Add(start);
	
			ClearSearch();
			Queue<Tile> checkNext = new Queue<Tile>();
			Queue<Tile> checkNow = new Queue<Tile>();
	
			start.distance = 0;
			checkNow.Enqueue(start);
	
			while (checkNow.Count > 0)
			{
				Tile t = checkNow.Dequeue();
				for (int i = 0; i < 4; ++i)
				{
					Tile next = GetTile(t.pos + dirs[i]);
					if (next == null || next.distance <= t.distance + 1)
						continue;
	
					if (addTile(t, next))
					{
						next.distance = t.distance + 1;
						next.prev = t;
						checkNext.Enqueue(next);
						retValue.Add(next);
					}
				}
	
				if (checkNow.Count == 0)
					SwapReference(ref checkNow, ref checkNext);
			}
	
			return retValue;
		}
	
		public void SelectTiles (List<Tile> tiles)
		{
#if OLD_TRPG
			for (int i = tiles.Count - 1; i >= 0; --i)
				tiles[i].GetComponent<Renderer>().material.SetColor("_Color", selectedTileColor);
			
			// [TKSR] 支持URP
			for (int i = tiles.Count - 1; i >= 0; --i)
				tiles[i].GetComponent<Renderer>().material.SetColor("_BaseColor", selectedTileColor);
#else
			BattleController.Instance.RefreshAnyTilesStatus(BattleController.EnumTileDisplayMode.Walkeable, tiles, true);
#endif
		}
	
		public void DeSelectTiles (List<Tile> tiles)
		{
#if OLD_TRPG
			for (int i = tiles.Count - 1; i >= 0; --i)
				tiles[i].GetComponent<Renderer>().material.SetColor("_Color", defaultTileColor);
			
			// [TKSR] 支持URP
			for (int i = tiles.Count - 1; i >= 0; --i)
					tiles[i].GetComponent<Renderer>().material.SetColor("_BaseColor", defaultTileColor);
#else
			BattleController.Instance.RefreshAnyTilesStatus(BattleController.EnumTileDisplayMode.Walkeable, tiles, false);
#endif
		}
	#endregion
	
	#region Private
		void ClearSearch ()
		{
			foreach (Tile t in tiles.Values)
			{
				t.prev = null;
				t.distance = int.MaxValue;
			}
		}
	
		void SwapReference (ref Queue<Tile> a, ref Queue<Tile> b)
		{
			Queue<Tile> temp = a;
			a = b;
			b = temp;
		}
	#endregion
	}
}
