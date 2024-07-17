// Copyright (c) Pixel Crushers. All rights reserved.

using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.LivelyChatBubblesSupport;
using UnityEngine;
using UnityEditor;

namespace TKSR
{
    [CustomEditor(typeof(TKSRSelectorSubtitlePanel))]
    public class TKSRSelectorSubtitlePanelEditor : StandardUISubtitlePanelEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRSelectorSubtitlePanel.selectorDlgPrefab)));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
