using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEngine;

namespace TKSR
{
    [CustomPropertyDrawer(typeof(QuestEntryNameAttribute))]
    public class QuestEntryNameDrawer : PropertyDrawer
    {
        // int m_QuesteEntryIndex = -1;
        GUIContent[] m_QuesteEntryNames;
        private readonly string strTKSRDialogueDatabasePath = "Assets/TKS-R/Dialogues/TKSR Database.asset";

        private DialogueDatabase newDatabase = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (this.newDatabase == null)
            {
                newDatabase = AssetDatabase.LoadAssetAtPath<DialogueDatabase>(strTKSRDialogueDatabasePath);
                if (newDatabase != null)
                {
                    var databaseID = (newDatabase != null) ? newDatabase.GetInstanceID() : -1;
                    Debug.Log($"[TKSR] QuestEntryNameDrawer newDatabase = {newDatabase.name}, databaseId = {databaseID}");
                }
            }

            if (newDatabase == null)
                return;
            
            // if (m_QuesteEntryIndex == -1)
            //     Setup(property);
            //
            // int oldIndex = m_QuesteEntryIndex;
            // m_QuesteEntryIndex = EditorGUI.Popup(position, label, m_QuesteEntryIndex, m_QuesteEntryNames);
            //
            // if (oldIndex != m_QuesteEntryIndex)
            //     property.stringValue = m_QuesteEntryNames[m_QuesteEntryIndex].text;
            
            // [TKSR] 上述自定义属性只能用在单一QuestName属性上,如果用在List或者Array中,则无效(多个Property会共享同一个index,从而导致相同的属性string)
            int oldIndex = SetupProperty(property);
            int newIndex = EditorGUI.Popup(position, label, oldIndex, m_QuesteEntryNames);

            if (newIndex != oldIndex)
            {
                property.stringValue = m_QuesteEntryNames[newIndex].text;
            }
        }
        
        int SetupProperty(SerializedProperty property)
        {
            int questEntryIndex = -1;
            if (newDatabase == null)
                return questEntryIndex;

            var questList = newDatabase.items;
            List<string> questEntries = new List<string>();
            foreach (var quest in questList)
            {
                if (quest.FieldExists("Entry Count"))
                {
                    int entryCnt = quest.LookupInt("Entry Count");
                    for (int i = 1; i <= entryCnt; i++)
                    {
                        string strEntryId = $"Entry {i}";
                        string strEntryName = quest.LookupValue(strEntryId);
                        questEntries.Add(strEntryName);
                    }
                }
            }
            
            if (m_QuesteEntryNames == null)
                m_QuesteEntryNames = new GUIContent[questEntries.Count];
            
            for (int i = 0; i < m_QuesteEntryNames.Length; i++)
            {
                string questEntryName = questEntries[i];
                m_QuesteEntryNames[i] = new GUIContent(questEntryName);
            }

            if (m_QuesteEntryNames.Length == 0)
                m_QuesteEntryNames = new[] { new GUIContent("[No Quest Entry Item In Dialogue Database]") };

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                bool questEntryNameFound = false;
                for (int i = 0; i < m_QuesteEntryNames.Length; i++)
                {
                    if (m_QuesteEntryNames[i].text == property.stringValue)
                    {
                        questEntryIndex = i;
                        questEntryNameFound = true;
                        break;
                    }
                }
                if (!questEntryNameFound)
                    questEntryIndex = 0;
            }
            else questEntryIndex = 0;

            property.stringValue = m_QuesteEntryNames[questEntryIndex].text;

            return questEntryIndex;
        }
    }
}