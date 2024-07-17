using System.Collections;
using I2.Loc;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace TKSR
{
    public enum EnumUIPanelType
    {
        None = 0,
        MainTitle,
        Save,
        Load,
        Info,
    }

    public enum EnumUIPanelShowMode
    {
        Default,
        Pop
    }
    
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = FindFirstObjectByType<GameUI> ();

                if (s_Instance != null)
                    return s_Instance;

                Create ();

                return s_Instance;
            }
        }
        

        protected static GameUI s_Instance;

        public static void Create ()
        {
            // GameUI controllerPrefab = Resources.Load<GameUI> ("GameUI");
            // s_Instance = Instantiate (controllerPrefab);
            AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("GameUI");
            GameObject s_Instance = handle.InstantiateSync();
        }

        public RectTransform mainPanel;
        public RectTransform documentPanel;
        public RectTransform infoPanel;
        public UIToastPanel toastPanel;

        private EnumUIPanelType m_curPanelType = EnumUIPanelType.None;
        
        void Awake ()
        {
            if (Instance != this)
            {
                Destroy (gameObject);
                return;
            }
        
            DontDestroyOnLoad (gameObject);

            ShowMainTitlePanel();
        }
        
        void Start()
        {
#if UNITY_EDITOR && TKSR_DEV
            if (LocalizationManager.CurrentLanguage == "Chinese (Traditional)")
            {
                toolbarInt = 1;
                DialogueManager.SetLanguage("zh-TW");
            }
            else
            {
                toolbarInt = 0;
                DialogueManager.SetLanguage("zh-CN");
            }
#endif
        }
        
#if UNITY_EDITOR && TKSR_DEV
        
        private int toolbarInt = 0;
        private int oldToolbarInt = 0;
        private string[] toolbarStrings = {"简体", "繁体"};
    
        void OnGUI () 
        {
            GUIStyle  myButtonStyle = new GUIStyle(GUI.skin.button);
            myButtonStyle.fontSize = 36;
            // myButtonStyle.normal.textColor = Color.white;
            myButtonStyle.onNormal.textColor = Color.blue;
            myButtonStyle.onHover.textColor = Color.blue;
            myButtonStyle.onActive.textColor = Color.blue;
            myButtonStyle.onFocused.textColor = Color.blue;


            oldToolbarInt = toolbarInt;
            toolbarInt = GUI.Toolbar (new Rect(Screen.width - 200, Screen.height - 80, 200, 80), toolbarInt, toolbarStrings, myButtonStyle);
            if (oldToolbarInt != toolbarInt)
            {
                if (toolbarInt == 0)
                {
                    LocalizationManager.CurrentLanguage = "Chinese (Simplified)";
                    // DialogueManager.SetLanguage(Localization.GetLanguage(SystemLanguage.ChineseSimplified));
                    DialogueManager.SetLanguage("zh-CN");

                }
                else
                {
                    LocalizationManager.CurrentLanguage = "Chinese (Traditional)";
                    // DialogueManager.SetLanguage(Localization.GetLanguage(SystemLanguage.ChineseTraditional));
                    DialogueManager.SetLanguage("zh-TW");
                }
            }
        }
#endif

        private void ShowMainTitlePanel()
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                Debug.Log("[TKSR] This is Game Scene, so show main title panel.");
                if (mainPanel != null)
                {
                    mainPanel.gameObject.SetActive(true);
                }
            }
        }

        public void HideAllPanels()
        {
            mainPanel.gameObject.SetActive(false);
            documentPanel.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(false);
        }
        
        public void ShowUIPanel(EnumUIPanelType panelType, EnumUIPanelShowMode panelMode = EnumUIPanelShowMode.Default)
        {
            var oldPanelType = panelType;
            m_curPanelType = panelType;
            
            if (panelMode == EnumUIPanelShowMode.Default)
                HideAllPanels();

            RectTransform targetPanel = null;
            switch (panelType)
            {
                case EnumUIPanelType.MainTitle:
                {
                    targetPanel = mainPanel;
                    mainPanel.gameObject.SetActive(true);
                }
                    break;
                case EnumUIPanelType.Load:
                {
                    targetPanel = documentPanel;
                    documentPanel.GetComponent<UISaveLoadPanel>().docStatus = DocumentStatus.Load;
                    documentPanel.gameObject.SetActive(true);
                }
                    break;
                case EnumUIPanelType.Save:
                {
                    targetPanel = documentPanel;
                    documentPanel.GetComponent<UISaveLoadPanel>().docStatus = DocumentStatus.Save;
                    documentPanel.gameObject.SetActive(true);
                }
                    break;
                case EnumUIPanelType.Info:
                {
                    targetPanel = infoPanel;
                    infoPanel.gameObject.SetActive(true);
                }
                    break;
            }

            if (targetPanel != null)
            {
                if (panelMode == EnumUIPanelShowMode.Pop)
                {
                    targetPanel.transform.SetAsLastSibling();
                }
            }
        }

        public void HideUIPanel(EnumUIPanelType panelType)
        {
            switch (panelType)
            {
                case EnumUIPanelType.MainTitle:
                {
                    mainPanel.gameObject.SetActive(false);
                }
                    break;
                case EnumUIPanelType.Load:
                case EnumUIPanelType.Save:
                {
                    documentPanel.gameObject.SetActive(false);
                }
                    break;
                case EnumUIPanelType.Info:
                {
                    infoPanel.gameObject.SetActive(false);
                }
                    break;
            }
        }

        #region Show InputName Panel

        private AssetHandle m_InputNameHandle;
        private GUIInputNameController m_InputNameController;
        public void AttachGUIInputNamePanel()
        {
            FilterAllCharactersChatAction(true);
            
            StartCoroutine(LoadInputNamePanelAsync());
        }

        private IEnumerator LoadInputNamePanelAsync()
        {
            var package = YooAssets.GetPackage(ResourceUtils.AB_YOO_PACKAGE);
            m_InputNameHandle = package.LoadAssetAsync<GameObject>("GUIInputNamePanel");
            yield return m_InputNameHandle;
            
            var goHandle = m_InputNameHandle.InstantiateAsync();
            yield return goHandle;

            GameObject go = goHandle.Result;
            if (go != null)
            {
                m_InputNameController = go.GetComponent<GUIInputNameController>();
            }
        }

        public void DetachGUIInputNamePanel()
        {
            if (m_InputNameController != null)
                DestroyImmediate(m_InputNameController.gameObject);
            if (m_InputNameHandle != null)
            {
                m_InputNameHandle.Release();
                m_InputNameHandle = null;
            }

            FilterAllCharactersChatAction(false);
        }

        /// <summary>
        /// 在Timeline过程中当出现UI的时候,则禁止对话框的Continue功能，防止在UI上的点击触发Dialog的Continue功能
        /// </summary>
        /// <param name="enableFilter"></param>
        private void FilterAllCharactersChatAction(bool enableFilter)
        {
            PlayerCharacter.PlayerInstance.gameObject.GetComponentInChildren<TKSRChatBubbleSubtitlePanel>()
                .filterContinueByUI = enableFilter;

            var allNPCS = FindObjectsOfType<NPCCharacter>();
            foreach (var npc in allNPCS)
            {
                npc.gameObject.GetComponentInChildren<TKSRChatBubbleSubtitlePanel>()
                    .filterContinueByUI = enableFilter;
            }
        }
        #endregion
    }
}