using UnityEngine;
using System;
using System.Collections;

namespace TacticalRPG {	
	public class StatComparisonCondition : StatusCondition 
	{
	#region Fields
		public StatTypes type { get; private set; }
		public int value { get; private set; }
		public Func<bool> condition { get; private set; }
		Stats stats;
	#endregion
		
	#region MonoBehaviour
		void Awake ()
		{
			stats = GetComponentInParent<Stats>();
		}
		
		void OnDisable ()
		{
			this.RemoveObserver(OnStatChanged, Stats.DidChangeNotification(type), stats);
		}
	#endregion
		
	#region Public
		public void Init (StatTypes type, int value, Func<bool> condition)
		{
			this.type = type;
			this.value = value;
			this.condition = condition;
			this.AddObserver(OnStatChanged, Stats.DidChangeNotification(type), stats);
		}
		
		public bool EqualTo ()
		{
			return stats[type] == value;
		}
		
		public bool LessThan ()
		{
			return stats[type] < value;
		}
		
		public bool LessThanOrEqualTo ()
		{
			return stats[type] <= value;
		}
		
		public bool GreaterThan ()
		{
			return stats[type] > value;
		}
		
		public bool GreaterThanOrEqualTo ()
		{
			return stats[type] >= value;
		}
	#endregion
		
	#region Notification Handlers
		void OnStatChanged (object sender, object args)
		{
			if (condition != null && !condition())
				Remove();
		}
	#endregion
	}
}
