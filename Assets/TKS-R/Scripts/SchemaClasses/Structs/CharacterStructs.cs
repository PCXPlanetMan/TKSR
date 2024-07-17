using System;
using System.Collections.Generic;

namespace TKSR
{
	[Serializable]
	public class SchemaCharacters
	{
		public List<CharacterParam> CharacterParam;
		public List<JobStartingStats> JobStartingStats;
		public List<JobGrowthStats> JobGrowthStats;
	}

	public class CharacterConfigItem
	{
		public JobStartingStats StartingStat;
		public JobGrowthStats GrowthStat;
	}
}