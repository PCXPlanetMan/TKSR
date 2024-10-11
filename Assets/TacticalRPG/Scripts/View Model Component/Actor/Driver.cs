using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class Driver : MonoBehaviour 
	{
		public Drivers normal;
		public Drivers special;
	
		public Drivers Current
		{
			get
			{
				return special != Drivers.None ? special : normal;
			}
		}
	}
}
