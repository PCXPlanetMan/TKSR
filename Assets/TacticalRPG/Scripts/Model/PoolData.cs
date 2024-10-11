using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public class PoolData
	{
		public GameObject prefab;
		public int maxCount;
		public Queue<Poolable> pool;
	}
}
