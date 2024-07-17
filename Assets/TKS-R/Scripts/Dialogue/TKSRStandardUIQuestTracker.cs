// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace TKSR
{
    public class TKSRStandardUIQuestTracker : StandardUIQuestTracker
    {
        public void OnQuestStateChange(string title)
        {
            var state = QuestLog.GetQuestState(title);
            Debug.Log($"[TKSR] OnQuestStateChange quest = {title}, state = {state}, Try to Save Quest.");
            DocumentDataManager.Instance.SaveGameQuestItemState(title, state);
        }

        public void OnQuestEntryStateChange(QuestEntryArgs args)
        {
            var entryState = QuestLog.GetQuestEntryState(args.questName, args.entryNumber);
            Debug.Log($"[TKSR] OnQuestEntryStateChange args.questName = {args.questName}, args.entryNumber = {args.entryNumber}, entryState = {entryState}, Try to Save Quest Entry.");
            DocumentDataManager.Instance.SaveGameQuestEntryItemState(args.questName, args.entryNumber, entryState);
        }
    }
}
