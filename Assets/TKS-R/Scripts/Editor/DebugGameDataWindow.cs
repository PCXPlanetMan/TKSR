using System.Collections;
using System.Collections.Generic;
using TKSR;
using UnityEditor;
using UnityEngine;

public class DebugGameDataWindow : EditorWindow
{
    private string charID;
    private string charLevel;
    private string itemID;
    private string itemCount;
    private string goldCount;
    private string mainRoleLv;
    private string intelligence;
    private string morality;
    private string courage;
    private string taskRatio;


    [MenuItem("Tools/TKS-R/Debug Game Window")]
    public static void ShowConfigCharWindow()
    {
        EditorWindow.GetWindow(typeof(DebugGameDataWindow));
    }

    DebugGameDataWindow()
    {
        this.titleContent = new GUIContent("配置数据调试");
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 24;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("配置数据", titleStyle, GUILayout.Width(300));

        GUILayout.Space(10);
        mainRoleLv = EditorGUILayout.TextField("主角Level", mainRoleLv);
        goldCount = EditorGUILayout.TextField("金钱数", goldCount);
        intelligence = EditorGUILayout.TextField("智慧", intelligence);
        morality = EditorGUILayout.TextField("仁德", morality);
        courage = EditorGUILayout.TextField("勇气", courage);


        GUILayout.Space(10);
        charID = EditorGUILayout.TextField("入队角色ID", charID);
        charLevel = EditorGUILayout.TextField("角色等级", charLevel);

        EditorGUILayout.Space();

        GUILayout.Space(10);
        itemID = EditorGUILayout.TextField("包裹物品ID", itemID);
        itemCount = EditorGUILayout.TextField("物品数量", itemCount);

        GUILayout.Space(10);
        taskRatio = EditorGUILayout.TextField("完成任务数", taskRatio);

        if (GUILayout.Button("Done"))
        {
            if (!string.IsNullOrEmpty(mainRoleLv))
            {
                int lv = int.Parse(mainRoleLv);
                DoneUpdateMainRoleLevel(lv);
            }

            if (!string.IsNullOrEmpty(goldCount))
            {
                int gold = int.Parse(goldCount);
                DoneUpdateGold(gold);
            }
            
            if (!string.IsNullOrEmpty(intelligence))
            {
                int intel = int.Parse(intelligence);
                DoneUpdateIntelligence(intel);
            }
            
            if (!string.IsNullOrEmpty(morality))
            {
                int mor = int.Parse(morality);
                DoneUpdateMorality(mor);
            }
            
            if (!string.IsNullOrEmpty(courage))
            {
                int cor = int.Parse(courage);
                DoneUpdateCourage(cor);
            }

            if (!string.IsNullOrEmpty(charID) && !string.IsNullOrEmpty(charLevel))
            {
                int id = int.Parse(charID);
                int level = int.Parse(charLevel);
                DoneCreateCharToTeam(id, level); 
            }

            if (!string.IsNullOrEmpty(itemID) && !string.IsNullOrEmpty(itemCount))
            {
                int id = int.Parse(itemID);
                int count = int.Parse(itemCount);
                DoneAddItemToPacket(id, count);
            }

            if (!string.IsNullOrEmpty(taskRatio))
            {
                float ratio = float.Parse(taskRatio);
                DoneUpdateTaskRatio(ratio);
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Quick Add All Items"))
        {
            QuickAddAllItems();
        }
        if (GUILayout.Button("Quick Add All Notes"))
        {
            QuickAddAllNotes();
        }
        GUILayout.EndVertical();
    }

    private void DoneUpdateMainRoleLevel(int lv)
    {
        if (Application.isPlaying)
        {
            lv = lv > 0 ? lv : 1;
#if TKSR_DEV
            DocumentDataManager.Instance.DebugUpdateMainPlayerLevel(lv);
#endif
        }
    }

    private void DoneUpdateGold(int newGold)
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugUpdateDocumentGolden(newGold);
#endif
        }
    }
    
    private void DoneUpdateIntelligence(int newIntelligence)
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugUpdateDocumentIntelligence(newIntelligence);
#endif
        }
    }
    
    private void DoneUpdateMorality(int newIntelligence)
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugUpdateDocumentMorality(newIntelligence);
#endif
        }
    }
    
    private void DoneUpdateCourage(int newIntelligence)
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugUpdateDocumentCourage(newIntelligence);
#endif
        }
    }

    private void DoneCreateCharToTeam(int nCharID, int nCharLevel)
    {
        if (Application.isPlaying)
        {
        }
    }

    private void DoneAddItemToPacket(int nItemID, int nItemCount)
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugAddItemToPackage(nItemID, nItemCount);
#endif
        }
    }

    private void DoneUpdateTaskRatio(float ratio)
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugUpdateTaskRatio(ratio);
#endif
        }
    }

    private void QuickAddAllItems()
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugAddAllSchemaItems();
#endif
        }
    }
    
    private void QuickAddAllNotes()
    {
        if (Application.isPlaying)
        {
#if TKSR_DEV
            DocumentDataManager.Instance.DebugAddAllDialogueNotes();
#endif
        }
    }
}
