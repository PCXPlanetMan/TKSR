using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class Job : MonoBehaviour
	{
	#region Fields / Properties
		public static readonly StatTypes[] statOrder = new StatTypes[]
		{
			StatTypes.MHP,
			StatTypes.MMP,
			StatTypes.ATK,
			StatTypes.DEF,
			StatTypes.MAT,
			StatTypes.MDF,
	#if !OLD_TRPG	
			StatTypes.HIT,
			StatTypes.EVD,
	#endif	
			StatTypes.SPD
		};
	
		public int[] baseStats = new int[ statOrder.Length ];
#if OLD_TRPG	
		public float[] growStats = new float[ statOrder.Length ];
#else	
		public int[] growStatsWhole = new int[statOrder.Length];
		public int[] growStatsFraction = new int[statOrder.Length];
#endif
		
		Stats stats;
	#endregion
	
	#region MonoBehaviour
		void OnDestroy ()
		{
			this.RemoveObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL), stats);
		}
	#endregion
	
	#region Public
		public void Employ ()
		{
			stats = gameObject.GetComponentInParent<Stats>();
			this.AddObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL), stats);
	
			Feature[] features = GetComponentsInChildren<Feature>();
			for (int i = 0; i < features.Length; ++i)
				features[i].Activate(gameObject);
		}
	
		public void UnEmploy ()
		{
			Feature[] features = GetComponentsInChildren<Feature>();
			for (int i = 0; i < features.Length; ++i)
				features[i].Deactivate();
	
			this.RemoveObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL), stats);
			stats = null;
		}
	
		public void LoadDefaultStats ()
		{
			for (int i = 0; i < statOrder.Length; ++i)
			{
				StatTypes type = statOrder[i];
				stats.SetValue(type, baseStats[i], false);
			}
	
			stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
			stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
		}
	#endregion
	
	#region Event Handlers
		protected virtual void OnLvlChangeNotification (object sender, object args)
		{
			int oldValue = (int)args;
			int newValue = stats[StatTypes.LVL];
	
			for (int i = oldValue; i < newValue; ++i)
				LevelUp();
		}
	#endregion
	
	#region Private
		void LevelUp ()
		{
			for (int i = 0; i < statOrder.Length; ++i)
			{
				StatTypes type = statOrder[i];
#if OLD_TRPG			
				int whole = Mathf.FloorToInt(growStats[i]);
				float fraction = growStats[i] - whole;
	
				int value = stats[type];
				value += whole;
				if (UnityEngine.Random.value > (1f - fraction))
					value++;
#else
				int whole = growStatsWhole[i];
				int fraction = growStatsFraction[i];
	
				int value = stats[type];
				value += whole;
#endif
	
				stats.SetValue(type, value, false);
			}
	
			stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
			stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
		}
	#endregion
	}
}
