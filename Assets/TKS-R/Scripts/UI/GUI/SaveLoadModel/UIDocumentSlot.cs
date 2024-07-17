using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class UIDocumentSlot : MonoBehaviour
    {
        public Image head1;
        public Image head2;
        public Image head3;
        public Image head4;
        public Image head5;
        public Image head6;

        public TextMeshProUGUI lv1;
        public TextMeshProUGUI lv2;
        public TextMeshProUGUI lv3;
        public TextMeshProUGUI lv4;
        public TextMeshProUGUI lv5;
        public TextMeshProUGUI lv6;

        public TextMeshProUGUI map;
        public TextMeshProUGUI timeBig;
        public TextMeshProUGUI timeSmall;
        public TextMeshProUGUI gold;

        public Button btnSaveLoad;
        public Button btnCancel;

        private UISaveLoadPanel m_parentUI;

        void Awake()
        {
            m_parentUI = this.gameObject.GetComponentInParent<UISaveLoadPanel>();
        }
        
        [HideInInspector] 
        [Range(1, 3)]
        public int slotIndex;

        private bool m_isSaveMode { get; set; }
        
        public void OnClickSaveLoadButton()
        {
            if (m_isSaveMode)
            {
                m_parentUI.SaveDocumentInSlotByIndex(slotIndex);
            }
            else
            {
                m_parentUI.LoadDocumentFromSlotByIndex(slotIndex);
            }

            ExitSaveLoadMode();
        }

        public void OnClickCancelButton()
        {
            ExitSaveLoadMode(true);
        }

        private void ExitSaveLoadMode(bool fromCancel = false)
        {
            if (m_isSaveMode)
                GameUI.Instance.HideUIPanel(EnumUIPanelType.Save);
            else
            {
                GameUI.Instance.HideUIPanel(EnumUIPanelType.Load);
                if (!fromCancel)
                    GameUI.Instance.HideAllPanels();
            }
        }
        

        public void SetSaveOrLoad(bool save)
        {
            m_isSaveMode = save;
            var localize = btnSaveLoad.GetComponent<Localize>();
            if (save)
            {
                // btnSaveLoad.GetComponent<TextMeshProUGUI>().text = ResourceUtils.I2FORMAT_DOC_SAVE;
                localize.SetTerm( ResourceUtils.I2FORMAT_DOC_SAVE);
            }
            else
            {
                // btnSaveLoad.GetComponent<TextMeshProUGUI>().text = ResourceUtils.I2FORMAT_DOC_LOAD;
                localize.SetTerm( ResourceUtils.I2FORMAT_DOC_LOAD);
            }
        }

        public void ClearContent()
        {
            head1.sprite = null;
            head2.sprite = null;
            head3.sprite = null;
            head4.sprite = null;
            head5.sprite = null;
            head6.sprite = null;
            map.text = string.Empty;
            timeBig.text = string.Empty;
            timeSmall.text = string.Empty;
            gold.text = string.Empty;

            head1.gameObject.SetActive(false);
            head2.gameObject.SetActive(false);
            head3.gameObject.SetActive(false);
            head4.gameObject.SetActive(false);
            head5.gameObject.SetActive(false);
            head6.gameObject.SetActive(false);
            map.gameObject.SetActive(false);
            timeBig.gameObject.SetActive(false);
            timeSmall.gameObject.SetActive(false);
            gold.gameObject.SetActive(false);
        }

        public void UpdateContent(GameDocument document)
        {
            var mainRoleInfo = document.MainRoleInfo;
            if (mainRoleInfo == null)
            {
                Debug.Log($"[TKSR] Current document index = {document.DocumentId} is not saved yet.");
                return;
            }
            
            map.gameObject.SetActive(true);
            var localize = map.GetComponent<Localize>();
            localize.SetTerm(ResourceUtils.I2FORMAT_SCENES_NAMES + document.SceneName);
            
            gold.gameObject.SetActive(true);
            gold.text = document.Gold.ToString();
            
            var dateFormat = new DateTime(document.Timestamp);
            timeBig.gameObject.SetActive(true);
  
            var localManager = timeBig.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_DATE_YEAR, dateFormat.Year.ToString());
            localManager.SetParameterValue(ResourceUtils.I2PARAM_DATE_MONTH, dateFormat.Month.ToString());
            localManager.SetParameterValue(ResourceUtils.I2PARAM_DATE_DAY, dateFormat.Day.ToString());

            
            timeSmall.gameObject.SetActive(true);
            timeSmall.text = string.Format(ResourceUtils.FORMAT_DATE_HMS, dateFormat.Hour, dateFormat.Minute,
                dateFormat.Second);
            
            head1.gameObject.SetActive(true);
            lv1.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, mainRoleInfo.Level);
            head1.sprite = CharactersController.Instance.LoadCharacterPortraitSprite(mainRoleInfo.CharName);
            
            if (document.Team == null || document.Team.Count == 0)
            {
                return;
            }
            
            // if (document.Team.Count > 0)
            // {
            //     Head2.gameObject.SetActive(true);
            //     GameCharDisplayInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[0]);
            //     Lv2.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            //     Head2.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
            // }
            // if (document.Team.Count > 1)
            // {
            //     Head3.gameObject.SetActive(true);
            //     GameCharDisplayInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[1]);
            //     Lv3.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            //     Head3.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
            // }
            // if (document.Team.Count > 2)
            // {
            //     Head4.gameObject.SetActive(true);
            //     GameCharDisplayInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[2]);
            //     Lv4.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            //     Head4.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
            // }
            // if (document.Team.Count > 3)
            // {
            //     Head5.gameObject.SetActive(true);
            //     GameCharDisplayInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[3]);
            //     Lv5.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            //     Head5.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
            // }
            // if (document.Team.Count > 4)
            // {
            //     Head6.gameObject.SetActive(true);
            //     GameCharDisplayInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[4]);
            //     Lv6.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            //     Head6.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
            // }
        }
    }
}