﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using TKSR;
using zFramework.Extension;
using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;

public static class TKSRTimelineDesignEditor 
{
	[MenuItem("Tools/TKS-R/All Characters Show")]
	public static void EditorMakeAllCharactersShow()
	{
		var allCharacters = GameObject.FindObjectsOfType<CharacterController2D>(true);
		foreach (var character in allCharacters)
		{
			character.gameObject.SetActive(true);
		}
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
	
	[MenuItem("Tools/TKS-R/All Characters Hide")]
	public static void EditorMakeAllCharactersHide()
	{
		var allCharacters = GameObject.FindObjectsOfType<CharacterController2D>(true);
		foreach (var character in allCharacters)
		{
			character.gameObject.SetActive(false);
		}

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}
}