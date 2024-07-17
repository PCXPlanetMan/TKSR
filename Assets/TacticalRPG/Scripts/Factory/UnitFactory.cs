using System;
using UnityEngine;
using System.IO;
using System.Collections;

public static class UnitFactory
{
	#region Public

	public static GameObject Create (string name, int level)
	{
#if OLD_TRPG
		UnitRecipe recipe = Resources.Load<UnitRecipe>("Unit Recipes/" + name);
#else
		YooAsset.AssetHandle handle = YooAsset.YooAssets.LoadAssetSync<UnitRecipe>(name);
		UnitRecipe recipe = handle.AssetObject as UnitRecipe;
#endif
		if (recipe == null)
		{
			Debug.LogError("No Unit Recipe for name: " + name);
			return null;
		}
		return Create(recipe, level);
	}

	public static GameObject Create (UnitRecipe recipe, int level)
	{
#if OLD_TRPG
		GameObject obj = InstantiatePrefab("Units/" + recipe.model);
#else
		GameObject obj = InstantiatePrefab(recipe.model);
#endif
		obj.name = recipe.name;
		obj.AddComponent<Unit>();
		AddStats(obj);
		AddLocomotion(obj, recipe.locomotion);
		obj.AddComponent<Status>();
		obj.AddComponent<Equipment>();
		AddJob(obj, recipe.job);
		AddRank(obj, level);
		obj.AddComponent<Health>();
		obj.AddComponent<Mana>();
		AddAttack(obj, recipe.attack);
		AddAbilityCatalog(obj, recipe.abilityCatalog);
		AddAlliance(obj, recipe.alliance);
		AddAttackPattern(obj, recipe.strategy);
		return obj;
	}
	#endregion

	#region Private
	static GameObject InstantiatePrefab (string name)
	{
#if OLD_TRPG
		GameObject prefab = Resources.Load<GameObject>(name);
		if (prefab == null)
		{
			Debug.LogError("No Prefab for name: " + name);
			return new GameObject(name);
		}
		GameObject instance = GameObject.Instantiate(prefab);
#else
		YooAsset.AssetHandle handle = YooAsset.YooAssets.LoadAssetSync<GameObject>(name);
		GameObject instance = handle.InstantiateSync();
#endif
		instance.name = instance.name.Replace("(Clone)", "");
		return instance;
	}

	static void AddStats (GameObject obj)
	{
		Stats s = obj.AddComponent<Stats>();
		s.SetValue(StatTypes.LVL, 1, false);
	}

	static void AddJob (GameObject obj, string name)
	{
#if OLD_TRPG
		GameObject instance = InstantiatePrefab("Jobs/" + name);
#else
		GameObject instance = InstantiatePrefab(name);
#endif
		instance.transform.SetParent(obj.transform);
		Job job = instance.GetComponent<Job>();
		job.Employ();
		job.LoadDefaultStats();
	}

	static void AddLocomotion (GameObject obj, Locomotions type)
	{
		switch (type)
		{
		case Locomotions.Walk:
			obj.AddComponent<WalkMovement>();
			break;
		case Locomotions.Fly:
			obj.AddComponent<FlyMovement>();
			break;
		case Locomotions.Teleport:
			obj.AddComponent<TeleportMovement>();
			break;
		}
	}

	static void AddAlliance (GameObject obj, Alliances type)
	{
		Alliance alliance = obj.AddComponent<Alliance>();
		alliance.type = type;
	}

	static void AddRank (GameObject obj, int level)
	{
		Rank rank = obj.AddComponent<Rank>();
		rank.Init(level);
	}

	static void AddAttack (GameObject obj, string name)
	{
#if OLD_TRPG
		GameObject instance = InstantiatePrefab("Abilities/" + name);
#else
		var skillArray = name.Split('/');
		name = skillArray[skillArray.Length - 1];
		GameObject instance = InstantiatePrefab(name);
#endif
		instance.transform.SetParent(obj.transform);
	}

	static void AddAbilityCatalog (GameObject obj, string name)
	{
		GameObject main = new GameObject("Ability Catalog");
		main.transform.SetParent(obj.transform);
		main.AddComponent<AbilityCatalog>();
#if OLD_TRPG
		AbilityCatalogRecipe recipe = Resources.Load<AbilityCatalogRecipe>("Ability Catalog Recipes/" + name);
#else
		YooAsset.AssetHandle handle = YooAsset.YooAssets.LoadAssetSync<AbilityCatalogRecipe>(name);
		AbilityCatalogRecipe recipe = handle.AssetObject as AbilityCatalogRecipe;
#endif
		if (recipe == null)
		{
			Debug.LogError("No Ability Catalog Recipe Found: " + name);
			return;
		}

		for (int i = 0; i < recipe.categories.Length; ++i)
		{
			GameObject category = new GameObject( recipe.categories[i].name );
			category.transform.SetParent(main.transform);

			for (int j = 0; j < recipe.categories[i].entries.Length; ++j)
			{
#if OLD_TRPG
				string abilityName = string.Format("Abilities/{0}/{1}", recipe.categories[i].name, recipe.categories[i].entries[j]);
#else
				string abilityName = recipe.categories[i].entries[j];
#endif
				GameObject ability = InstantiatePrefab(abilityName);
				ability.transform.SetParent(category.transform);
			}
		}
	}

	static void AddAttackPattern (GameObject obj, string name)
	{
		Driver driver = obj.AddComponent<Driver>();
		if (string.IsNullOrEmpty(name))
		{
			driver.normal = Drivers.Human;
		}
		else
		{
			driver.normal = Drivers.Computer;
#if OLD_TRPG
			GameObject instance = InstantiatePrefab("Attack Pattern/" + name);
#else
			GameObject instance = InstantiatePrefab(name);
#endif
			instance.transform.SetParent(obj.transform);
		}
	}
	#endregion
}