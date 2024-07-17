using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace TKSR
{
    public class UISettingInfoStatus : MonoBehaviour
    {
        public TextMeshProUGUI level;
        public TextMeshProUGUI hp;
        public TextMeshProUGUI mp;
        public TextMeshProUGUI exp;
        public TextMeshProUGUI needUpExp;
        public TextMeshProUGUI attack;
        public TextMeshProUGUI defense;
        public TextMeshProUGUI hit;
        public TextMeshProUGUI evade;
        public TextMeshProUGUI speed;
        public TextMeshProUGUI luck;
        public TextMeshProUGUI understand;
        public TextMeshProUGUI move;
        public TextMeshProUGUI skillPoints;

        private UIInfoPanel m_parentPanel;
        void Awake()
        {
            m_parentPanel = GetComponentInParent<UIInfoPanel>();
        }

        void OnEnable()
        {
            UpdateStatusInfo();
        }

        public void UpdateStatusInfo()
        {
            var currentDocument = DocumentDataManager.Instance.GetCurrentDocument();
            // 刷新状态面板
            GameCharDisplayInfo displayInfo = null;
            if (m_parentPanel.SelectTeamMemberIndex == 0)
            {
                displayInfo = currentDocument.MainRoleInfo;
            }
            else
            {
                
            }

            DisplayStatusInfo(displayInfo);
        }
        
        private void DisplayStatusInfo(GameCharDisplayInfo displayInfo)
        {
            var localManager = level.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_LV, displayInfo.Level.ToString());
            
            localManager = hp.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_HP, $"{displayInfo.HP} / {displayInfo.MaxHP}");
            
            localManager = mp.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_MP, $"{displayInfo.MP} / {displayInfo.MaxMP}");
            
            localManager = exp.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_EXP, displayInfo.Exp.ToString());
            
            localManager = needUpExp.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_NEEDUPEXP, displayInfo.NeedUpExp.ToString());
            
            localManager = attack.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_ATK, displayInfo.AttackValue.ToString());
            
            localManager = defense.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_DEF, displayInfo.DefenseValue.ToString());
            
            localManager = hit.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_HIT, displayInfo.HitValue.ToString());
            
            localManager = evade.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_EVD, displayInfo.EvadeValue.ToString());
            
            localManager = speed.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_SPD, displayInfo.SpeedValue.ToString());
            
            localManager = luck.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_LCK, displayInfo.LuckValue.ToString());
            
            localManager = understand.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_USD, displayInfo.UnderstandValue.ToString());
            
            localManager = move.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_MOV, displayInfo.MovementValue.ToString());
            
            localManager = skillPoints.GetComponent<LocalizationParamsManager>();
            localManager.SetParameterValue(ResourceUtils.I2PARAM_STATUS_SPS, displayInfo.SkillPoints.ToString());
        }
    }
}