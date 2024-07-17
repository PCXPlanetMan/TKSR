using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public enum DocumentStatus
    {
        Save,
        Load
    }
    public class UISaveLoadPanel : MonoBehaviour
    {
        public Button btnPageUp;
        public Button btnPageDown;
        public TextMeshProUGUI pageInfo;
        
        public UIDocumentSlot slot1;
        public UIDocumentSlot slot2;
        public UIDocumentSlot slot3;

        private int m_curPageIndex = 0;
        private readonly int MAX_DOCUMENT_PAGES_CNT = 3;
        private readonly int SLOT_CNT_PER_PAGE = 3;
        
        [HideInInspector]
        public DocumentStatus docStatus = DocumentStatus.Load;

        void Awake()
        {
            slot1.slotIndex = 1;
            slot2.slotIndex = 2;
            slot3.slotIndex = 3;
        }
        
        private void OnEnable()
        {
            UpdatePageStatus();

            if (PlayerCharacter.PlayerInstance != null)
            {
                PlayerCharacter.PlayerInstance.PausePlayer(true);
            }
        }

        private void OnDisable()
        {
            if (PlayerCharacter.PlayerInstance != null)
            {
                PlayerCharacter.PlayerInstance.PausePlayer(false);
            }
        }

        private void UpdatePageStatus()
        {
            var localManager = pageInfo.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_PAGE_INDEX, (m_curPageIndex + 1).ToString());
            localManager.SetParameterValue(ResourceUtils.I2PARAM_PAGES_CNT, MAX_DOCUMENT_PAGES_CNT.ToString());
            
            if (m_curPageIndex == 0)
            {
                btnPageUp.interactable = false;
                btnPageDown.interactable = true;
            }
            else if (m_curPageIndex == MAX_DOCUMENT_PAGES_CNT - 1)
            {
                btnPageUp.interactable = true;
                btnPageDown.interactable = false;
            }
            else
            {
                btnPageUp.interactable = true;
                btnPageDown.interactable = true;
            }
            
            UpdateDocumentStatus(docStatus);
        }

        public void DoPageUp()
        {
            if (m_curPageIndex == 0)
            {
                return;
            }

            m_curPageIndex--;

            UpdatePageStatus();
        }

        public void DoPageDown()
        {
            if (m_curPageIndex == MAX_DOCUMENT_PAGES_CNT - 1)
            {
                return;
            }

            m_curPageIndex++;

            UpdatePageStatus();
        }

        public void UpdateDocumentStatus(DocumentStatus status)
        {
            docStatus = status;
            var archives = DocumentDataManager.Instance.LoadTKSArchives();
            if (archives != null)
            {
                int itemIndex = m_curPageIndex * SLOT_CNT_PER_PAGE + 1;
                if (itemIndex < archives.Documents.Count)
                {
                    var document = archives.Documents[itemIndex];
                    UpdateSlot(slot1, document);
                }
                else
                {
                    UpdateSlot(slot1, null);
                }

                itemIndex = m_curPageIndex * SLOT_CNT_PER_PAGE + 2;
                if (itemIndex < archives.Documents.Count)
                {
                    var document = archives.Documents[itemIndex];
                    UpdateSlot(slot2, document);
                }
                else
                {
                    UpdateSlot(slot2, null);
                }

                itemIndex = m_curPageIndex * SLOT_CNT_PER_PAGE + 3;
                if (itemIndex < archives.Documents.Count)
                {
                    var document = archives.Documents[itemIndex];
                    UpdateSlot(slot3, document);
                }
                else
                {
                    UpdateSlot(slot3, null);
                }
            }
            else
            {
                UpdateSlot(slot1, null);
                UpdateSlot(slot2, null);
                UpdateSlot(slot3, null);
            }

            if (docStatus == DocumentStatus.Save)
            {
                slot1.SetSaveOrLoad(true);
                slot2.SetSaveOrLoad(true);
                slot3.SetSaveOrLoad(true);
            }
            else
            {
                slot1.SetSaveOrLoad(false);
                slot2.SetSaveOrLoad(false);
                slot3.SetSaveOrLoad(false);
            }
        }

        private void UpdateSlot(UIDocumentSlot slot, GameDocument doc)
        {
            slot.ClearContent();
            
            if (doc != null && doc.DocumentId > -1)
            {
                slot.UpdateContent(doc);
            }
        }

        public void SaveDocumentInSlotByIndex(int index)
        {
            var documentIndex = m_curPageIndex * SLOT_CNT_PER_PAGE + index;
            DocumentDataManager.Instance.SaveGameDocument(documentIndex);
            UpdateDocumentStatus(docStatus);
        }

        public void LoadDocumentFromSlotByIndex(int index)
        {
            var documentIndex = m_curPageIndex * SLOT_CNT_PER_PAGE + index;
            bool bSucc = DocumentDataManager.Instance.LoadGameDocument(documentIndex);
            // TODO: 根据加载文档进入正确的场景
            if (bSucc)
            {
                var document = DocumentDataManager.Instance.GetCurrentDocument();
                var sceneName = document.SceneName;
                var vecPlayerPos = new Vector2(document.PosX, document.PosY);
                var playerDirection = (EnumFaceDirection)Enum.Parse(typeof(EnumFaceDirection), document.Direction);
                SceneController.TransitionToScene(sceneName, vecPlayerPos, playerDirection);
            }
            else
            {
                Debug.LogError("[TKSR] Load game document at index = {index} documentIndex = {documentIndex} Failed.");
            }
        }
    }
}