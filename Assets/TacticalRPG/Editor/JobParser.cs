using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using TKSR;
using zFramework.Extension;
using Object = UnityEngine.Object;

namespace TacticalRPG {	
	public static class JobParser 
	{
		[MenuItem("Pre Production/Parse Jobs")]
		public static void Parse()
		{
#if !OLD_TRPG	
			s_bReCreateAllPrefabs = true;
#endif
			CreateDirectories ();
			ParseStartingStats ();
			ParseGrowthStats ();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
	#if !OLD_TRPG
		private static bool s_bReCreateAllPrefabs = false;
		private static readonly string TACTICAL_ASSETS_RES_PATH = "Assets/TKS-R/TacticalRPGResources";
		private static readonly string TACTICAL_SCHEMAS_PATH = "Assets/TKS-R/Schemas";
		private static readonly string TACTICAL_DESIGN_PATH = "Assets/TKS-R/Design";
		
		/// <summary>
		/// 由于数据被固化在Prefab上,当数据结构发生变化的时候,单纯进行Parse是无法增减数据字段的,只能删除重建
		/// </summary>
		[MenuItem("Pre Production/Init Jobs")]
		public static void Init()
		{
			s_bReCreateAllPrefabs = true;
			CreateDirectories ();
			ParseStartingStats ();
			ParseGrowthStats ();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	#endif
	
		static void CreateDirectories ()
		{
#if OLD_TRPG
			if (!AssetDatabase.IsValidFolder("Assets/Resources/Jobs"))
				AssetDatabase.CreateFolder("Assets/Resources", "Jobs");
#else
			if (!AssetDatabase.IsValidFolder(TACTICAL_ASSETS_RES_PATH + "/Jobs"))
				AssetDatabase.CreateFolder(TACTICAL_ASSETS_RES_PATH, "Jobs");
#endif
		}
	
		static void ParseStartingStats ()
		{
#if OLD_TRPG		
			string readPath = string.Format("{0}/Settings/JobStartingStats.csv", Application.dataPath);
			string[] readText = File.ReadAllLines(readPath);
			for (int i = 1; i < readText.Length; ++i)
				PartsStartingStats(readText[i]);
#else
			string readPath = TACTICAL_SCHEMAS_PATH + "/" + ResourceUtils.AB_SCHEMA_CHARACTERS + ".json";
			string json = AssetDatabase.LoadAssetAtPath<TextAsset>(readPath).text;
			var schemaCharConfigs = JsonConvert.DeserializeObject<SchemaCharacters>(json);
			for (int i = 0; i < schemaCharConfigs.JobStartingStats.Count; i++)
			{
				if (s_bReCreateAllPrefabs)
				{
					string fullPath = string.Format(TACTICAL_ASSETS_RES_PATH + "/Jobs/{0}.prefab", schemaCharConfigs.JobStartingStats[i].Name);
					AssetDatabase.DeleteAsset(fullPath);
				}
				
				PartsStartingStats(schemaCharConfigs.JobStartingStats[i]);
			}
#endif
		}
	
#if OLD_TRPG
		static void PartsStartingStats (string line)
		{
			string[] elements = line.Split(',');
			GameObject obj = GetOrCreate(elements[0]);
			Job job = obj.GetComponent<Job>();
			for (int i = 1; i < Job.statOrder.Length + 1; ++i)
				job.baseStats[i-1] = Convert.ToInt32(elements[i]);
	
			StatModifierFeature evade = GetFeature (obj, StatTypes.EVD);
			evade.amount = Convert.ToInt32(elements[8]);
	
			StatModifierFeature res = GetFeature (obj, StatTypes.RES);
			res.amount = Convert.ToInt32(elements[9]);
	
			StatModifierFeature move = GetFeature (obj, StatTypes.MOV);
			move.amount = Convert.ToInt32(elements[10]);
	
			StatModifierFeature jump = GetFeature (obj, StatTypes.JMP);
			jump.amount = Convert.ToInt32(elements[11]);
		}
#else
		static void PartsStartingStats(JobStartingStats jobStartStats)
		{
			var templateName = jobStartStats.Name;
			GameObject obj = GetOrCreate(templateName);
			
			Job job = obj.GetComponent<Job>();
			for (int i = 0; i < Job.statOrder.Length; i++)
			{
				var state = Job.statOrder[i];
				int stateValue = 0;
				switch (state)
				{
					case StatTypes.MHP:
					{
						stateValue = jobStartStats.MHP;
					}
						break;
					case StatTypes.MMP:
					{
						stateValue = jobStartStats.MMP;
					}
						break;
					case StatTypes.ATK:
					{
						stateValue = jobStartStats.ATK;
					}
						break;
					case StatTypes.DEF:
					{
						stateValue = jobStartStats.DEF;
					}
						break;
					case StatTypes.MAT:
					{
						stateValue = jobStartStats.MAT;
					}
						break;
					case StatTypes.MDF:
					{
						stateValue = jobStartStats.MDF;
					}
						break;
					case StatTypes.HIT:
					{
						stateValue = jobStartStats.HIT;
					}
						break;
					case StatTypes.EVD:
					{
						stateValue = jobStartStats.EVD;
					}
						break;
					case StatTypes.SPD:
					{
						stateValue = jobStartStats.SPD;
					}
						break;
				}
				
				job.baseStats[i] = stateValue;
			}
			
#if OLD_TRPG
			StatModifierFeature evade = GetFeature (obj, StatTypes.EVD);
			evade.amount = jobStartStats.EVD;
	
			StatModifierFeature res = GetFeature (obj, StatTypes.RES);
			res.amount = jobStartStats.RES;
#endif
			StatModifierFeature move = GetFeature (obj, StatTypes.MOV);
			move.amount = jobStartStats.MOV;
	
#if OLD_TRPG
			StatModifierFeature jump = GetFeature (obj, StatTypes.JMP);
			jump.amount = jobStartStats.JMP;
#endif
		}
		
#endif
	
		static void ParseGrowthStats ()
		{
#if OLD_TRPG
			string readPath = string.Format("{0}/Settings/JobGrowthStats.csv", Application.dataPath);
			string[] readText = File.ReadAllLines(readPath);
			for (int i = 1; i < readText.Length; ++i)
				ParseGrowthStats(readText[i]);
#else
			string readPath = TACTICAL_SCHEMAS_PATH + "/" + ResourceUtils.AB_SCHEMA_CHARACTERS + ".json";
			string json = AssetDatabase.LoadAssetAtPath<TextAsset>(readPath).text;
			var schemaCharConfigs = JsonConvert.DeserializeObject<SchemaCharacters>(json);
			for (int i = 0; i < schemaCharConfigs.JobGrowthStats.Count; i++)
			{
				ParseGrowthStats(schemaCharConfigs.JobGrowthStats[i]);
			}
#endif
		}
		
#if OLD_TRPG
		static void ParseGrowthStats (string line)
		{
			string[] elements = line.Split(',');
			GameObject obj = GetOrCreate(elements[0]);
			Job job = obj.GetComponent<Job>();
			for (int i = 1; i < elements.Length; ++i)
				job.growStats[i-1] = Convert.ToSingle(elements[i]);
		}
#else
		static void ParseGrowthStats(JobGrowthStats jobGrowthStats)
		{
			var templateName = jobGrowthStats.Name;
			GameObject obj = GetOrCreate(templateName);
			
			Job job = obj.GetComponent<Job>();
			for (int i = 0; i < Job.statOrder.Length; i++)
			{
				var state = Job.statOrder[i];
				int stateValueBase = 0;
				int stateValueSub = 0;
				switch (state)
				{
					case StatTypes.MHP:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.MHP, ref stateValueBase, ref stateValueSub);
					}
						break;
					case StatTypes.MMP:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.MMP, ref stateValueBase, ref stateValueSub);
					}
						break;
					case StatTypes.ATK:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.ATK, ref stateValueBase, ref stateValueSub);
					}
						break;
					case StatTypes.DEF:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.DEF, ref stateValueBase, ref stateValueSub);
					}
						break;
					case StatTypes.MAT:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.MAT, ref stateValueBase, ref stateValueSub);
					}
						break;
					case StatTypes.MDF:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.MDF, ref stateValueBase, ref stateValueSub);
					}
						break;
	
					case StatTypes.HIT:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.HIT, ref stateValueBase, ref stateValueSub);
					}
						break;
					case StatTypes.EVD:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.EVD, ref stateValueBase, ref stateValueSub);
					}
						break;
	
					case StatTypes.SPD:
					{
						CharacterDataUtils.ParseGrowthParam(jobGrowthStats.SPD, ref stateValueBase, ref stateValueSub);
					}
						break;
				}
				
				job.growStatsWhole[i] = stateValueBase;
				job.growStatsFraction[i] = stateValueSub;
			}
		}
#endif
	
		static StatModifierFeature GetFeature (GameObject obj, StatTypes type)
		{
			StatModifierFeature[] smf = obj.GetComponents<StatModifierFeature>();
			for (int i = 0; i < smf.Length; ++i)
			{
				if (smf[i].type == type)
					return smf[i];
			}
	
			StatModifierFeature feature = obj.AddComponent<StatModifierFeature>();
			feature.type = type;
			return feature;
		}
	
		static GameObject GetOrCreate (string jobName)
		{
#if OLD_TRPG
			string fullPath = string.Format("Assets/Resources/Jobs/{0}.prefab", jobName);
#else
			string fullPath = string.Format(TACTICAL_ASSETS_RES_PATH + "/Jobs/{0}.prefab", jobName);
#endif
	
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
			if (obj == null)
				obj = Create(fullPath);
			return obj;
		}
	
		static GameObject Create (string fullPath)
		{
			GameObject instance = new GameObject ("temp");
			instance.AddComponent<Job>();
#if OLD_TRPG		
			GameObject prefab = PrefabUtility.CreatePrefab( fullPath, instance );
			GameObject.DestroyImmediate(instance);
#else
			GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, fullPath);
			Object.DestroyImmediate(instance);
#endif		
			return prefab;
		}
		
#if !OLD_TRPG
		/// <summary>
		/// 由于数据被固化在Prefab上,当数据结构发生变化的时候,单纯进行Parse是无法增减数据字段的,只能删除重建
		/// </summary>
		[MenuItem("Pre Production/Parse LevelUp Data")]
		public static void ParseLevelUp()
		{
			if (!AssetDatabase.IsValidFolder(TACTICAL_DESIGN_PATH))
				AssetDatabase.CreateFolder(TACTICAL_DESIGN_PATH, "LevelUp");
			
			string readPath = TACTICAL_SCHEMAS_PATH + "/" + ResourceUtils.AB_SCHEMA_CHARACTERS + ".json";
			string json = AssetDatabase.LoadAssetAtPath<TextAsset>(readPath).text;
			var schemaCharConfigs = JsonConvert.DeserializeObject<SchemaCharacters>(json);
			
			for (int i = 0; i < schemaCharConfigs.JobStartingStats.Count; i++)
			{
				var jobStartStats = schemaCharConfigs.JobStartingStats[i];
				var jobGrowStats = schemaCharConfigs.JobGrowthStats[i];
	
				List<LevelUpData> listAllLevelUpData = CharacterDataUtils.CalcDesignLevelUpData(jobStartStats, jobGrowStats);
				string cvsPath = string.Format("{0}/LevelUp/{1}.csv", TACTICAL_DESIGN_PATH, jobStartStats.Name);
				CsvUtility.Write(listAllLevelUpData, cvsPath);
			}
	
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
#endif
		
		
	}
}
