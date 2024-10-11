using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class AbilityCatalogRecipe : ScriptableObject 
	{
		[System.Serializable]
		public class Category
		{
			public string name;
			public string[] entries;
		}
		public Category[] categories;
	}
}
