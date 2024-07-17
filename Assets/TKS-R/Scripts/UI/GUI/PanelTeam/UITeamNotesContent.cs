using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UITeamNotesContent : INoteDataInterface
    {
        protected override void DoRefreshDataSource()
        {
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            mNoteDataList.Clear();
            if (document.I2StoryNotes == null)
            {
                Debug.LogWarning($"[TKSR] There is no Notes data.");
                return;
            }

            if (document.I2StoryNotes.Count == 0)
            {
                Debug.LogWarning($"[TKSR] Notes Count is Zero.");
                return;
            }
            m_totalDataCount = document.I2StoryNotes.Count;
            for (int i = 0; i < m_totalDataCount; ++i)
            {
                InfoNoteData tData = new InfoNoteData();
                var note = document.I2StoryNotes[i];
                var note_array = note.Split(',');
                if (note_array.Length > 1)
                {
                    tData.mNoteI2 = note_array[0];
                    int extra = int.Parse(note_array[1]);
                    tData.ExtraFormat = (EnumNoteExtraFormat)extra;
                }
                else
                {
                    tData.mNoteI2 = note;
                }

                tData.mNoteI2 = ResourceUtils.I2FORMAT_TEAM_CATEGORY_NOTES + tData.mNoteI2;
                mNoteDataList.Add(tData);
            }
            
            base.DoRefreshDataSource();
        }
    }
}