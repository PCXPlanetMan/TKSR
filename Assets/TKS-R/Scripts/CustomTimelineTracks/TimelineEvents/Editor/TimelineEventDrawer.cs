using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using TKSRPlayables.Extensions;
using UnityEditor;
using UnityEngine;

namespace TKSRPlayables.Editor
{
    [CustomPropertyDrawer(typeof(TimelineEventBehaviour))]
    public class TimelineEventDrawer : PropertyDrawer
    {
        private List<string> _eventHandlerListStart = new List<string> {"None"};
        public static Type[] GeneralTypes = {typeof(string), typeof(float), typeof(int)};
        private static GUIStyle _errorStyle;

        private static GUIStyle GetErrorStyle()
        {
            if (_errorStyle == null)
            {
                _errorStyle = new GUIStyle();
                _errorStyle.normal.textColor = Color.red;
            }

            return _errorStyle;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 1;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty handlerKeyProperty =
                property.FindPropertyRelative("HandlerKey");
            SerializedProperty isMethodWithParamProperty = property.FindPropertyRelative("IsMethodWithParam");
            SerializedProperty invokeEventsInEditModeProperty = property.FindPropertyRelative("InvokeEventsInEditMode");

            SerializedProperty argValueProperty = property.FindPropertyRelative("ArgValue");
            SerializedProperty hasToPauseProperty = property.FindPropertyRelative("hasToPause");
            

            TimelineEventClip clip = property.serializedObject.targetObject as TimelineEventClip;
            MethodInfo selectedMethod;

            EditorGUILayout.Space();

            isMethodWithParamProperty.boolValue =
                EditorGUILayout.Toggle("Method with param?", isMethodWithParamProperty.boolValue);


            selectedMethod = AddMethodsPopup("Method", handlerKeyProperty,
                clip.TrackTargetObject,
                isMethodWithParamProperty.boolValue);
            
            if (selectedMethod != null && isMethodWithParamProperty.boolValue)
            {
                var isSpecialType = AddEnumValuePopup(selectedMethod, argValueProperty) ||
                                    AddBoolValuePopup(selectedMethod, argValueProperty);
                if (!isSpecialType)
                {
                    AddGeneralValueInput(selectedMethod, argValueProperty, clip.TrackTargetObject);
                }
            }
            else
            {
                argValueProperty.stringValue = "";
            }

            if (selectedMethod == null)
            {
                EditorGUILayout.HelpBox("Unable to find event handlers. ", MessageType.Warning);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            invokeEventsInEditModeProperty.boolValue = EditorGUILayout.Toggle("Invoke in Edit Mode",
                invokeEventsInEditModeProperty.boolValue);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            hasToPauseProperty.boolValue = EditorGUILayout.Toggle("Has to pause for dialogue",
                hasToPauseProperty.boolValue);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();          
        }

        private MethodInfo AddMethodsPopup(string label, SerializedProperty property, GameObject gameObject,
            bool listSingleArgMethods = false)
        {
            if (gameObject == null)
            {
                return null;
            }

            Behaviour[] behaviours = gameObject.GetComponents<Behaviour>();

            var allMethods = behaviours.SelectMany(
                    x => x.GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance))
				.Where(
                    x =>
                    {
                        // [TKSR] 只保留TKSR空间下以Timeline开头的函数
                        if (!x.DeclaringType.ToString().StartsWith("TKSR") || !x.Name.StartsWith("Timeline"))
                            return false;
                        if (listSingleArgMethods) // [TKSR] TODO:考虑扩展编辑器使得可以显示多参数以及List类型等,目前只能通过格式化string进行参数解析
                        {
                            return (x.ReturnType == typeof(void)) && (x.GetParameters().Length == 1) &&
                                   (x.GetParameters()[0].ParameterType == typeof(string) ||
                                    x.GetParameters()[0].ParameterType == typeof(int) ||
                                    x.GetParameters()[0].ParameterType == typeof(float) ||
                                    x.GetParameters()[0].ParameterType == typeof(bool) ||
                                    x.GetParameters()[0].ParameterType.IsEnum);
                        }
                        else
                        {
                            return (x.ReturnType == typeof(void)) && (x.GetParameters().Length == 0);
                        }
                    }).ToArray();

            var callbackMethodsEnumarable = allMethods.Select(
                x => x.DeclaringType.ToString() + "." + x.Name);

            if (callbackMethodsEnumarable.Count() == 0)
            {
                property.stringValue = string.Empty;
                return null;
            }
            
            string[] callbackMethods = _eventHandlerListStart.Concat(callbackMethodsEnumarable).ToArray();
            
            var lastTwoDotPattern = @"[^\.]+\.[^\.]+$";

            var callbackMethodNames = callbackMethods.Select(m =>
            {
                var result = Regex.Match(m, lastTwoDotPattern, RegexOptions.RightToLeft);
                return result.Success ? result.Value : m;
            }).ToArray();

            int index = Array.IndexOf(callbackMethods, property.stringValue);

            index = EditorGUILayout.Popup(label, index, callbackMethodNames, GUILayout.ExpandWidth(true));

            if (index >= 0)
            {
                property.stringValue = callbackMethods[index];
            }

            return index > 0 ? allMethods[index - 1] : null;
        }

        private bool AddEnumValuePopup(MethodInfo selectedMethod, SerializedProperty property)
        {
            var param = selectedMethod.GetParameters()[0];
            if (param.ParameterType.IsEnum)
            {
                var names = Enum.GetNames(param.ParameterType);
                var values = Enum.GetValues(param.ParameterType).Cast<int>().ToArray();
                var enumValue = property.stringValue.ConvertToType<int>();
                var index = Math.Max(Array.IndexOf(values, enumValue), 0);
                index = EditorGUILayout.Popup(param.Name, index, names, GUILayout.ExpandWidth(true));
                if (index >= 0)
                {
                    property.stringValue = values[index].ToString();
                }
                else
                {
                    property.stringValue = "-1";
                }
            }
            else
            {
                return false;
            }

            return true;
        }


        private bool AddBoolValuePopup(MethodInfo selectedMethod, SerializedProperty property)
        {
            var param = selectedMethod.GetParameters()[0];
            if (param.ParameterType == typeof(bool))
            {
                var boolValue = false;
                bool.TryParse(property.stringValue, out boolValue);
                var value = EditorGUILayout.Toggle(param.Name, boolValue);
                property.stringValue = value.ToString();
                return true;
            }

            return false;
        }

        private void AddGeneralValueInput(MethodInfo selectedMethod, SerializedProperty property)
        {
            var param = selectedMethod.GetParameters()[0];
            var type = param.ParameterType;
            var label = string.Format("{0} ({1})", param.Name, type);
            var oldColor = EditorStyles.label.normal.textColor;
            if (GeneralTypes.Contains(type))
            {
                var isValid = property.stringValue.IsValidAsType(type);
                if (!isValid)
                {
                    EditorStyles.label.normal.textColor = Color.red;
                }
            }

            EditorGUILayout.PropertyField(property, new GUIContent(label));

            EditorStyles.label.normal.textColor = oldColor;
        }
        
        // [TKSR] 在编辑器窗口显示对话内容
        Vector2 scroll;
        private void AddGeneralValueInput(MethodInfo selectedMethod, SerializedProperty property, GameObject gameObject)
        {
            var param = selectedMethod.GetParameters()[0];
            var type = param.ParameterType;
            var label = string.Format("{0} ({1})", param.Name, type);
            var oldColor = EditorStyles.label.normal.textColor;
            if (GeneralTypes.Contains(type))
            {
                var isValid = property.stringValue.IsValidAsType(type);
                if (!isValid)
                {
                    EditorStyles.label.normal.textColor = Color.red;
                }
            }

            EditorGUILayout.PropertyField(property, new GUIContent(label));

            EditorStyles.label.normal.textColor = oldColor;
            
            if (selectedMethod.Name.StartsWith("TimelineSim"))
            {
                if (GeneralTypes.Contains(type))
                {
                    var isValid = property.stringValue.IsValidAsType(type);
                    if (isValid)
                    {
                        var database = PixelCrushers.DialogueSystem.EditorTools.FindInitialDatabase();
                        if (type == typeof(int))
                        {
                            int intValue;
                            if (int.TryParse(property.stringValue, out intValue))
                            {
                                DrawDialogueInfo(intValue, database, gameObject);
                            }
                        }
                        else if (type == typeof(string))
                        {
                            var allIds = property.stringValue.Split(',');
                            for (int i = 0; i < allIds.Length; i++)
                            {
                                int intValue = int.Parse(allIds[i]);
                                DrawDialogueInfo(intValue, database, gameObject);
                            }
                        }
                    }
                }
            }
        }

        private void DrawDialogueInfo(int intValue, PixelCrushers.DialogueSystem.DialogueDatabase database, GameObject gameObject)
        {
            // Debug.Log($"author.Name = {database.author}");
            var dialogTrigger = gameObject.GetComponent<PixelCrushers.DialogueSystem.DialogueSystemTrigger>();
            // Debug.Log($"dialogTrigger.conversation = {dialogTrigger.conversation}");

            var conversation = database.GetConversation(dialogTrigger.conversation);
            var entry = conversation.GetDialogueEntry(intValue);
            Debug.Log($"entry = {entry.conversationID}, {entry.subtitleText}");
            var actor = database.GetActor(entry.ActorID);

            EditorGUILayout.Space();
                    
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(25));
            var field = actor.fields.Find(x => x.title.CompareTo("Display Name") == 0);
            var name = string.Empty;
            if (field != null)
            {
                name = field.value;
            }
            if (string.IsNullOrEmpty(name))
            {
                EditorGUILayout.TextArea(actor.Name);
            }
            else
            {
                EditorGUILayout.TextArea(name);
            }
            EditorGUILayout.EndScrollView();
                    
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(80));
            EditorGUILayout.TextArea(entry.subtitleText);
            EditorGUILayout.EndScrollView();
        }
    }
}