using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

namespace TacticalRPG {	
	public class Tile : MonoBehaviour 
	{
	#region Const
		public const float stepHeight = 0.25f;
	#endregion
	
	#region Fields / Properties
		public Point pos;
		public int height;
#if OLD_TRPG
		public Vector3 center { get { return new Vector3(pos.x, height * stepHeight, pos.y); }}
#else
		public Vector3 center
		{
			get
			{
				return ConvertTileXYVector3(new Vector3(pos.x, pos.y, height * stepHeight));
			}
		}
	
		private Grid gridMap;
#endif
		public GameObject content;
		[HideInInspector] public Tile prev;
		[HideInInspector] public int distance;
	#endregion
	
	#region Public
		public void Grow ()
		{
			height++;
			Match();
		}
		
		public void Shrink ()
		{
			height--;
			Match ();
		}
	
		public void Load (Point p, int h)
		{
			pos = p;
			height = h;
			Match();
		}
		
#if OLD_TRPG
		public void Load (Vector3 v)
		{
			Load (new Point((int)v.x, (int)v.z), (int)v.y);
		}
#else
		public void Load (Vector3 v, Grid gridmap)
		{
			this.gridMap = gridmap;
			Load(new Point((int)v.x, (int)v.y), (int)v.z);
		}
#endif
	#endregion
	
	#region Private
		void Match ()
		{
#if OLD_TRPG		
			transform.localPosition = new Vector3( pos.x, height * stepHeight / 2f, pos.y );
			transform.localScale = new Vector3(1, height * stepHeight, 1);
#else
			transform.localPosition = ConvertTileXYVector3(new Vector3( pos.x, pos.y, height * stepHeight / 2f ));
			// transform.localScale = new Vector3(1, 1, height * stepHeight);
			transform.localScale = new Vector3(1, 1, 1);
#endif
		}
		
#if UNITY_EDITOR && !OLD_TRPG
		private GUIStyle tagStyle = new GUIStyle();
	        
		private void OnDrawGizmos()
		{
			Gizmos.DrawCube(transform.position, new Vector3(0.2f, 0.2f, 0.2f));
		}
#endif
		
	#if !OLD_TRPG
		private Vector3 ConvertTileXYVector3(Vector3 vecTile)
		{
			if (gridMap != null)
			{
				return gridMap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
			}
			else
			{
				return vecTile;
			}
		}
	#endif
		
	#endregion
	}
}
