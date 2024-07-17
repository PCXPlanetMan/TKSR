using System;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEngine;

namespace TKSR
{
    [CustomPropertyDrawer(typeof(QuestNameAttribute))]
    public class QuestNameDrawer : PropertyDrawer
    {
        int m_QuestIndex = -1;
        GUIContent[] m_QuestNames;
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
                    Debug.Log($"[TKSR] QuestNameDrawer newDatabase = {newDatabase.name}, databaseId = {databaseID}");
                }
            }

            if (newDatabase == null)
                return;
            
            // if (m_QuestIndex == -1)
            //     Setup(property);
            //
            // int oldIndex = m_QuestIndex;
            // m_QuestIndex = EditorGUI.Popup(position, label, m_QuestIndex, m_QuestNames);
            //
            // if (oldIndex != m_QuestIndex)
            //     property.stringValue = m_QuestNames[m_QuestIndex].text;
            
            // [TKSR] 上述自定义属性只能用在单一QuestName属性上,如果用在List或者Array中,则无效(多个Property会共享同一个index,从而导致相同的属性string)
            int oldIndex = SetupProperty(property);
            int newIndex = EditorGUI.Popup(position, label, oldIndex, m_QuestNames);

            if (newIndex != oldIndex)
            {
                property.stringValue = m_QuestNames[newIndex].text;
            }
        }

        void Setup(SerializedProperty property)
        {
            if (newDatabase == null)
                return;

            var questList = newDatabase.items;
            m_QuestNames = new GUIContent[questList.Count];
            
            for (int i = 0; i < m_QuestNames.Length; i++)
            {
                string questName = questList[i].Name;
                m_QuestNames[i] = new GUIContent(questName);
            }

            if (m_QuestNames.Length == 0)
                m_QuestNames = new[] { new GUIContent("[No Quest Item In Dialogue Database]") };

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                bool questNameFound = false;
                for (int i = 0; i < m_QuestNames.Length; i++)
                {
                    if (m_QuestNames[i].text == property.stringValue)
                    {
                        m_QuestIndex = i;
                        questNameFound = true;
                        break;
                    }
                }
                if (!questNameFound)
                    m_QuestIndex = 0;
            }
            else m_QuestIndex = 0;

            property.stringValue = m_QuestNames[m_QuestIndex].text;
        }
        
        int SetupProperty(SerializedProperty property)
        {
            int questIndex = -1;
            if (newDatabase == null)
                return questIndex;

            var questList = newDatabase.items;
            if (m_QuestNames == null)
                m_QuestNames = new GUIContent[questList.Count];
            
            for (int i = 0; i < m_QuestNames.Length; i++)
            {
                string questName = questList[i].Name;
                m_QuestNames[i] = new GUIContent(questName);
            }

            if (m_QuestNames.Length == 0)
                m_QuestNames = new[] { new GUIContent("[No Quest Item In Dialogue Database]") };

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                bool questNameFound = false;
                for (int i = 0; i < m_QuestNames.Length; i++)
                {
                    if (m_QuestNames[i].text == property.stringValue)
                    {
                        questIndex = i;
                        questNameFound = true;
                        break;
                    }
                }
                if (!questNameFound)
                    questIndex = 0;
            }
            else questIndex = 0;

            property.stringValue = m_QuestNames[questIndex].text;

            return questIndex;
        }
    }
}