using System;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using CharacterInfo = PixelCrushers.DialogueSystem.CharacterInfo;

namespace TKSR
{
    public class TimelineCharData
    {
        public GameObject CharGo;
        public bool InitColliderEnabled;
    }
    
    public class TimelineScenarioItem : MonoBehaviour
    {
        private PlayableDirector m_playableDirector;
        private TimelineAsset m_someTimelineAsset;
        private List<PlayableDirector> m_subPlayableDirectors;
        private Dictionary<string, TimelineCharData> m_dictCharacters = new Dictionary<string, TimelineCharData>();
        
#if UNITY_EDITOR && TKSR_DEV
        [Tooltip("仅在调试状态下提示当前Timeline表达的是何种剧情")]
        public string debugScenarioDesc;
#endif
        // Timeline播放结束后是否跳转到其他场景或者在当前场景进行Teleport?
        // 如果没有则需要进行Deployment的部署尝试
        [Tooltip("Timeline播放结束后执行何种操作:部署NPC?场景跳转?")]
        public TransitionPoint.TransitionType finishTransitionType = TransitionPoint.TransitionType.Nothing;
        [SceneName]
        public string toNewSceneName;
        public SceneTransitionDestination.DestinationTag transitionDestinationTag;
        public TransitionPoint destinationPoint;
        [BattleName]
        public string toBattleSceneName;
        public EnumFaceDirection finishedMainPlayerDirection = EnumFaceDirection.Invalid;
        [Tooltip("剧情结束的时候,有可能需要检测某些Collider的最新状态")]
        public TransitionCollider[] attachedColliders;

        [Tooltip("剧情中如果有分支,则需要跳转执行")] 
        public TimelineSelectorSwitchItem[] selectorSwitches;

        // 之所以不放在Timeline本身下面,是因为有些效果和路径有自己的坐标.而为了编辑方便(显示Gizmo),一般Timeline自己的坐标都不在Zero,而是放在场景中和剧情相关的大概位置以起到提示作用.
        [Tooltip("和当前Timeline相关的特效/动画等数据放在TimelineScenarioItem_Effects节点下,需要手动设置并在End时手动清除")]
        public GameObject attachedEffects;
        [Tooltip("和当前Timeline相关的路径节点数据放在TimelineScenarioItem_PathNodes节点下,需要手动设置并在End时手动清除")]
        public GameObject attachedPathNodes;

        [Tooltip("BGM some timeline neeed")] 
        public AudioClip audioBGM;
        
        private Conversation m_curConversation;
        private bool m_isInited = false;

        
        public void InitTimeline(bool releaseInput = false)
        {
            if (releaseInput)
            {
                PlayerInput.Instance.ReleaseControl();
            }
            
            var dialogueSystem = this.gameObject.GetComponent<DialogueSystemTrigger>();
            if (dialogueSystem != null)
            {
                var conversationName = dialogueSystem.conversation;
                Debug.LogFormat($"[TKSR] Current Timeline with onversationName = {conversationName}");
                var db = DialogueManager.masterDatabase;
                m_curConversation = db.GetConversation(conversationName);
            }
            else
            {
                Debug.LogWarning("[TKSR] Not any DialogueSystemTrigger found");
            }
            
            var mainRole = FindFirstObjectByType<PlayerCharacter>(FindObjectsInactive.Include);
            if (mainRole != null)
            {
                m_dictCharacters.TryAdd(mainRole.name, new TimelineCharData()
                {
                    CharGo = mainRole.gameObject,
                    InitColliderEnabled = mainRole.GetComponent<Collider2D>().enabled
                });
            }
            
            
            m_playableDirector = this.gameObject.GetComponent<PlayableDirector>();
            if (m_playableDirector != null)
            {
                m_someTimelineAsset = (TimelineAsset)m_playableDirector.playableAsset;
                if (m_someTimelineAsset != null)
                {
                    for (int i = 0; i < m_someTimelineAsset.outputTrackCount; i++)
                    {
                        DebugTrack(i);
                        // if (TryToSyncMainPlayerTrackPositionOffset(i))
                        // {
                        //     break;
                        // }
                    }
                }
            }

            int index = 0;
            foreach (var binding in m_playableDirector.playableAsset.outputs)
            {
                var bindingObj = m_playableDirector.GetGenericBinding(binding.sourceObject);
                var animator = bindingObj as Animator;
                if (animator == null)
                {
                    var go = bindingObj as NPCCharacter;
                    if (go != null)
                    {
                        animator = go.GetComponent<Animator>();
                    }
                }
                if (animator != null)
                {
                    Debug.Log($"[TKSR] {index} binding.sourceObject.name = {binding.sourceObject.name}, animator.name = {animator.name}");
                    if (animator.gameObject.CompareTag("NPC")) 
                    {
                        var hideChar = animator.gameObject.GetComponent<HideCharacter>();
                        if (hideChar == null || hideChar.IsUsedInDlg) // 默认排除HideActor的影响,除非设置了IsUsedInDlg
                        {
                            animator.gameObject.SetActive(true);
                            if (!m_dictCharacters.ContainsKey(animator.name))
                            {
                                m_dictCharacters.Add(animator.name, new TimelineCharData()
                                {
                                    CharGo = animator.gameObject,
                                    InitColliderEnabled = animator.gameObject.GetComponent<Collider2D>().enabled
                                });

                                // [TKSR] 剧情中的对象最好能关闭Collider2D,这样才能在Tween Track中正确控制对象的移动
                                animator.gameObject.GetComponent<Collider2D>().enabled = false;
                            }
                        }
                        
                    }
                }
                
                index++;
            }

            var allChildren = this.gameObject.GetComponentsInChildren<PlayableDirector>(true);
            m_subPlayableDirectors = new List<PlayableDirector>();
            for (int i = 0; i < allChildren.Length; i++)
            {
                var child = allChildren[i];
                if (String.Compare(child.gameObject.name, this.gameObject.name, StringComparison.Ordinal) != 0)
                {
                    m_subPlayableDirectors.Add(child);
                }
            }
            
            m_playableDirector.Play();

            m_isInited = true;
        }

        private bool IsPlayableDirectorPlaying()
        {
            if (m_playableDirector != null)
            {
                if (m_playableDirector.extrapolationMode == DirectorWrapMode.None)
                {
                    return m_playableDirector.state == PlayState.Playing;
                }
                else if (m_playableDirector.extrapolationMode == DirectorWrapMode.Hold)
                {
                    return m_playableDirector.time < m_playableDirector.duration;
                }
            }

            return false;
        }
        
#if UNITY_EDITOR && TKSR_DEV
        public static bool s_IsDialogAutoInTimeline = false;
        void OnGUI()
        {
            if (m_playableDirector != null && m_playableDirector.state == PlayState.Playing)
            {
                if (!string.IsNullOrEmpty(debugScenarioDesc))
                {
                    GUIStyle labelFont = new GUIStyle();
                    labelFont.normal.textColor = Color.green;
                    labelFont.fontSize = 36;
                    GUI.Label(new Rect(0, 0, 1500, 80), debugScenarioDesc, labelFont);
                }

                GUIStyle  myButtonStyle = new GUIStyle(GUI.skin.button);
                myButtonStyle.fontSize = 32;
                s_IsDialogAutoInTimeline = GUI.Toggle(new Rect(0, 60, 120, 60), s_IsDialogAutoInTimeline, "Auto",
                    myButtonStyle);
                
                if (GUI.Button(new Rect(120, 60, 120, 60), "Skip", myButtonStyle))
                {
                    m_playableDirector.time = m_playableDirector.duration;
                }
            }
        }
#endif

        enum EnumEndStatus
        {
            Normal,
            WithLoadingText,
            Fade,
            FadeOut,
        }

        private int m_EndStatusWithEntryId = 0;
        private EnumEndStatus m_enumEndStatus = EnumEndStatus.Normal;

        private void DoEndTimeline()
        {
            switch (m_enumEndStatus)
            {
                case EnumEndStatus.Normal:
                {
                    DoTimelineEndEventTransition();
                }
                    break;
                case EnumEndStatus.WithLoadingText:
                {
                    DoTimelineEndEventTransition_WithLoadingText();
                }
                    break;
                case EnumEndStatus.Fade:
                {
                    DoTimelineEndEventTransition_WithWhiteFader();
                }
                    break;
                case EnumEndStatus.FadeOut:
                {
                    DoTimelineEndEventTransition_WithWhiteOnlyFadeOut();
                }
                    break;
            }
        }
        
        private void DoTimelineEndEventTransition()
        {
            Debug.Log("[TKSR] DoTimelineEndEventTransition");
            
            if (finishTransitionType == TransitionPoint.TransitionType.OnlyDeploy)
            {
                // [TKSR] TODO Bug:如果当前剧情是TransitionCollider触发的(例如被空气墙阻挡),则此时CurrentEntrance就是这个TransitionCollider;
                // 那么在这个TransitionCollider上调用父类SceneTransitionDestination的DoNextActionsByQuests,实际上是不合理的,会自动触发某些不可预知问题;
                // 而此时意图是做Deploy,只能通过在TransitionCollider上设置defaultDeploy且必须使得所有Quests不成立,才能实现这个目的。
                SceneController.Instance.CurrentEntrance.DoNextActionsByQuests();
                // 如果剧情播放完成最后一帧主角的方向和进入场景时设定的Entrance布置的方向不一致则需要重新设置一下主角的方向
                if (finishedMainPlayerDirection != EnumFaceDirection.Invalid)
                {
                    PlayerCharacter.PlayerInstance.UpdateFacingDirection(finishedMainPlayerDirection);
                }

                // 一般来说只有OnlyDeploy和Nothing状态才需要检查一下场景中特定入口的开关情况
                if (attachedColliders != null && attachedColliders.Length > 0)
                {
                    foreach (var collider in attachedColliders)
                    {
                        collider.CheckQuestsToOpenColliderByOrder();
                    }
                }
            }
            else if (finishTransitionType == TransitionPoint.TransitionType.Nothing)
            {
                // 如果剧情播放完成最后一帧主角的方向和进入场景时设定的Entrance布置的方向不一致则需要重新设置一下主角的方向
                if (finishedMainPlayerDirection != EnumFaceDirection.Invalid)
                {
                    PlayerCharacter.PlayerInstance.UpdateFacingDirection(finishedMainPlayerDirection);
                }
                
                // 一般来说只有OnlyDeploy和Nothing状态才需要检查一下场景中特定入口的开关情况
                if (attachedColliders != null && attachedColliders.Length > 0)
                {
                    foreach (var collider in attachedColliders)
                    {
                        collider.CheckQuestsToOpenColliderByOrder();
                    }
                }
            }
            else
            {
                if (finishTransitionType == TransitionPoint.TransitionType.SameScene)
                {
                    var transitioningGameObject = PlayerCharacter.PlayerInstance.gameObject;
                    GameObjectTeleporter.Teleport(transitioningGameObject, destinationPoint.transform);
                }
                else if (finishTransitionType == TransitionPoint.TransitionType.BattleScene)
                {
                    if (string.IsNullOrEmpty(toBattleSceneName))
                    {
                        Debug.LogError("[TKSR] Transition to battle scene, but no battle scene name.");
                        return;
                    }
                    Debug.Log($"[TKSR] Timeline End and Jump to Battle : {toBattleSceneName}");
                    SceneController.TransitionToZone(toBattleSceneName, true, SceneTransitionDestination.DestinationTag.A, TransitionPoint.TransitionType.BattleScene, null);
                }
                else if (finishTransitionType == TransitionPoint.TransitionType.DifferentZone)
                {
                    if (string.IsNullOrEmpty(toNewSceneName))
                    {
                        Debug.LogError("[TKSR] Transition to normal scene, but no normal scene name.");
                        return;
                    }

                    string tipsWhenSceneLoading = null;
                    if (m_EndStatusWithEntryId > 0)
                    {
                        tipsWhenSceneLoading = ParseDialogueContentByEntryId(m_EndStatusWithEntryId);
                    }
                    
                    SceneController.TransitionToZone(toNewSceneName, true, transitionDestinationTag, TransitionPoint.TransitionType.DifferentZone, tipsWhenSceneLoading);
                }
            }

            ClearTimelineStatusData();
        }
        
        private void DoTimelineEndEventTransition_WithLoadingText()
        {
            if (m_EndStatusWithEntryId == 0)
            {
                Debug.LogWarning("[TKSR] Do Loading text is empty when timeline is end.");
                TimelineEndEventTransition();
                return;
            }
            
            DoTimelineEndEventTransition();
        }
        
        private void DoTimelineEndEventTransition_WithWhiteFader()
        {
            ScreenFader.Instance.loadingImage.color = Color.white;
            ScreenFader.Instance.fadeDuration = 2;
            DoTimelineEndEventTransition();
        }
        
        public void DoTimelineEndEventTransition_WithWhiteOnlyFadeOut()
        {
            ScreenFader.Instance.loadingCanvasGroup.alpha = 1f;
            ScreenFader.Instance.loadingImage.color = Color.white;
            ScreenFader.Instance.fadeDuration = 1;
            DoTimelineEndEventTransition();
        }
        
        void Update()
        {
            if (m_isInited && !IsPlayableDirectorPlaying())
            {
                DoEndTimeline();
                m_isInited = false;

                if (CallBackTimelineEnd != null)
                {
                    CallBackTimelineEnd.Invoke(0);
                }
            }
        }

        void DebugTrack(int trackIndex)
        {
            // Get track from TimelineAsset
            TrackAsset someTimelineTrackAsset = m_someTimelineAsset.GetOutputTrack(trackIndex);

            // Change TimelineAsset's muted property value
            //someTimelineTrackAsset.muted = !someTimelineTrackAsset.muted;
            Debug.LogFormat("i = {0}, name = {1}", trackIndex, someTimelineTrackAsset.name);

            //double t = playableDirector.time; // Store elapsed time
            //playableDirector.RebuildGraph(); // Rebuild graph
            //playableDirector.time = t; // Restore elapsed time
        }

        bool TryToSyncMainPlayerTrackPositionOffset(int trackIndex)
        {
            TrackAsset someTimelineTrackAsset = m_someTimelineAsset.GetOutputTrack(trackIndex);
            if (someTimelineTrackAsset != null)
            {
                if (someTimelineTrackAsset.name.CompareTo("Move_MainPlayer") == 0)
                {
                    Debug.Log("[TKSR] Found MainPlayer Movement Track and Set position offset Sync to MainPlayer");
                    var animTrack = (AnimationTrack)someTimelineTrackAsset;
                    var oldTrackPosition = animTrack.position;
                    //animTrack.position = PlayerCharacter.PlayerInstance.transform.position;
                    return true;
                }
            }
            
            return false;
        }

        public void MuteMainPlayerTracks(bool mute)
        {
            var allTracks = m_someTimelineAsset.GetOutputTracks().ToList();
            var foundMainPlayerAnimTrack = allTracks.Find(x => x.name.CompareTo("Anim_MainPlayer") == 0);
            var foundMainPlayerMoveTrack = allTracks.Find(x => x.name.CompareTo("Move_MainPlayer") == 0);
            if (foundMainPlayerAnimTrack != null)
            {
                foundMainPlayerAnimTrack.muted = mute;
            }

            if (foundMainPlayerMoveTrack != null)
            {
                foundMainPlayerMoveTrack.muted = mute;
            }
        }
        
        private void MuteMainPlayerMoveTracks(bool mute)
        {
            var allTracks = m_someTimelineAsset.GetOutputTracks().ToList();
            var foundMainPlayerMoveTrack = allTracks.Find(x => x.name.CompareTo("Move_MainPlayer") == 0);
            if (foundMainPlayerMoveTrack != null)
            {
                foundMainPlayerMoveTrack.muted = mute;
            }
        }
        
        #region Timeline Events
        private bool m_isDialogueClipContinued = false;
        public bool isDlogClipContinued
        {
            get => m_isDialogueClipContinued;
            set => m_isDialogueClipContinued = value;
        }

        /// <summary>
        /// Timeline主轴上播放对话
        /// </summary>
        /// <param name="entryId"></param>
        public void TimelineSimShowDialogue(int entryId)
        {
            if (entryId <= 0)
            {
                Debug.LogError("[TKSR] Wrong entry id of dialogues");
                return;
            }

            Debug.Log($"[TKSR] TimelineSimShowDialogue wieh entryId = {entryId}");
            
            var db = DialogueManager.masterDatabase;
            if (m_curConversation != null)
            {
                var entry = m_curConversation.GetDialogueEntry(entryId);
                if (entry != null)
                {
                    var actor = db.GetActor(entry.ActorID);
                    Sprite actorPortrait = null;
                    string actorDisplayName = null;
                    if (actor != null)
                    {
                        actorPortrait = actor.spritePortrait;
                        actorDisplayName = CharacterInfo.GetLocalizedDisplayNameInDatabase(actor.Name);
                    }

                    if (!string.IsNullOrEmpty(actor.Name))
                    {
                        string subtitleText = entry.subtitleText;
                        if (string.IsNullOrEmpty(subtitleText) && string.IsNullOrEmpty(entry.Sequence))
                        {
                            Debug.LogError($"[TKSR] Empty dialogue content and sequence with entryId = {entryId} in {m_curConversation.Name}");
                        }

                        var dialogue = TryGetChatBubbleByName(actor.Name);
                        if (dialogue != null)
                        {
                            if (!string.IsNullOrEmpty(subtitleText))
                            {
                                var localizedText = DialogueManager.GetLocalizedText(subtitleText);
                                var formattedText = FormattedText.Parse(localizedText, db.emphasisSettings);
                                dialogue.ShowSubtitleByTimeline(actorDisplayName, formattedText.text, actorPortrait);
                            }
                            else
                            {
                                Debug.LogError($"[TKSR] Current Dialog Subtitle Text is NULL");
                            }

                            // [TKSR] 注意顺序:先模拟执行Lua再调用事件
                            if (!string.IsNullOrEmpty(entry.userScript))
                            {
                                Debug.Log($"[TKSR] Current dialogue content with entryId = {entryId} has Lua scripts = {entry.userScript}");
                                Lua.Run(entry.userScript);
                            }

                            if (!string.IsNullOrEmpty(entry.sceneEventGuid))
                            {
                                var sceneEvent = DialogueSystemSceneEvents.GetDialogueEntrySceneEvent(entry.sceneEventGuid);
                                if (sceneEvent != null)
                                {
                                    try
                                    {
                                        sceneEvent.onExecute.Invoke(DialogueManager.instance.gameObject);
                                    }
                                    catch (System.Exception e)
                                    {
                                        if (DialogueDebug.logWarnings) Debug.LogWarning("Scene OnExecute() event failed on dialogue entry " + entry.conversationID + ":" + entry.id + ": " + e.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError($"[TKSR] Can't found TKSRChat by Actor = {actor.Name}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"[TKSR] Actor Name is null");
                    }
                }
                else
                {
                    Debug.LogError($"[TKSR] Not found Dialog Entry with id = {entryId}");
                }
            }
            else
            {
                Debug.LogError("[TKSR] Current Conversation is Empty.");
            }

            m_isDialogueClipContinued = false;
        }
        
        public void TimelineSimHideDialogue(int entryId)
        {
            var db = DialogueManager.masterDatabase;
            if (m_curConversation != null)
            {
                var entry = m_curConversation.GetDialogueEntry(entryId);
                if (entry != null)
                {
                    var actor = db.GetActor(entry.ActorID);
                    if (actor != null)
                    {
                        if (!string.IsNullOrEmpty(actor.Name))
                        {
                            var dialogue = TryGetChatBubbleByName(actor.Name);
                            if (dialogue != null)
                            {
                                dialogue.HideImmediate();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 在Timeline主轴上同一时刻播放多个对话,主要用于有多人同时说话的情形
        /// </summary>
        /// <param name="strMultiDlgs">dlgId1,dlgId2,...,dlgIdn</param>
        public void TimelineSimShowMultiDialoguesSametime(string strMultiDlgs)
        {
            if (string.IsNullOrEmpty(strMultiDlgs))
            {
                Debug.LogError("[TKSR] Error parameters format of TimelineSimShowMultiDialoguesSametime");
                return;
            }

            var paramsMultiDlgs = strMultiDlgs.Split(',');
            foreach (var entry in paramsMultiDlgs)
            {
                if (int.TryParse(entry, out int entryId))
                {
                    TimelineSimShowDialogue(entryId);
                }
            }

            m_isDialogueClipContinued = false;
        }

        public void TimelineSimHideMultiDialoguesSametime(string strMultiDlgs)
        {
            if (string.IsNullOrEmpty(strMultiDlgs))
            {
                Debug.LogError("[TKSR] Error parameters format of TimelineSimHideMultiDialoguesSametime");
                return;
            }

            var paramsMultiDlgs = strMultiDlgs.Split(',');
            foreach (var entry in paramsMultiDlgs)
            {
                if (int.TryParse(entry, out int entryId))
                {
                    TimelineSimHideDialogue(entryId);
                }
            }
        }

        /// <summary>
        /// 用于播放SubTrack上的动画,主要适用于目标播放细节动画
        /// </summary>
        /// <param name="subTimeline"></param>
        public void TimelinePlaySubPlayable(string subTimeline)
        {
            if (m_subPlayableDirectors != null)
            {
                bool foundSub = false;
                for (int i = 0; i < m_subPlayableDirectors.Count; i++)
                {
                    var playable = m_subPlayableDirectors[i];
                    if (String.Compare(playable.gameObject.name, subTimeline, StringComparison.Ordinal) == 0)
                    {
                        foreach (var binding in playable.playableAsset.outputs)
                        {
                            var animator = playable.GetGenericBinding(binding.sourceObject) as Animator;
                            if (animator != null)
                            {
                                animator.gameObject.SetActive(true);
                            }
                        }
                        
                        playable.gameObject.SetActive(true);
                        playable.Play();
                        foundSub = true;
                        break;
                    }
                }

                if (!foundSub)
                {
                    Debug.LogError($"[TKSR] Play sub timeline but ot found name : {subTimeline}, Check it!");
                }
            }
        }
        
        /// <summary>
        /// 用于同一时间播放SubTrack上的多个动画,主要适用于目标播放细节动画
        /// </summary>
        /// <param name="subTimeline">subTimeline1,subTimeline2,...,subTimeline</param>
        public void TimelinePlaySubPlayables(string subTimelines)
        {
            if (string.IsNullOrEmpty(subTimelines))
            {
                Debug.LogError("[TKSR] Empty parameter of TimelinePlaySubPlayables");
                return;
            }
            
            var paramTimelines = subTimelines.Split(',');
            foreach (var subTimeline in paramTimelines)
            {
                TimelinePlaySubPlayable(subTimeline);
            }
        }
        
        /// <summary>
        /// 停止播放SubTrack上的动画
        /// </summary>
        /// <param name="subTimelines"></param>
        public void TimelineStopSubPlayable(string subTimeline)
        {
            if (m_subPlayableDirectors != null)
            {
                bool foundSub = false;
                for (int i = 0; i < m_subPlayableDirectors.Count; i++)
                {
                    var playable = m_subPlayableDirectors[i];
                    if (String.Compare(playable.gameObject.name, subTimeline, StringComparison.Ordinal) == 0)
                    {
                        playable.Stop();
                        
                        foreach (var binding in playable.playableAsset.outputs)
                        {
                            var animator = playable.GetGenericBinding(binding.sourceObject) as Animator;
                            if (animator != null)
                            {
                                // [TKSR] 一般只有部分特效的SubTimeline需要同步关闭其GameObject,而对于CharactersPool下的人物则由Timeline本身控制其可见性
                                if (animator.gameObject.CompareTag("NPC") || animator.gameObject.CompareTag("Player"))
                                {
                                    continue;
                                }
                                animator.gameObject.SetActive(false);
                            }
                        }
                        
                        playable.gameObject.SetActive(false);
                        foundSub = true;
                        break;
                    }
                }
                
                if (!foundSub)
                {
                    Debug.LogError($"[TKSR] Stop sub timeline but ot found name : {subTimeline}, Check it!");
                }
            }
        }

        /// <summary>
        /// 关闭当前所有SubTracks上动画
        /// </summary>
        public void TimelineStopAllSubPlayables()
        {
            if (m_subPlayableDirectors != null)
            {
                for (int i = 0; i < m_subPlayableDirectors.Count; i++)
                {
                    var playable = m_subPlayableDirectors[i];
                    playable.Stop();
                    playable.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 在某一时刻,关闭某些SubTimeline的同时,播放另外的SubTimeline
        /// </summary>
        /// <param name="strSubManageParams">toStopSubTimeline1,toStopSubTimeline2,...,toStopSubTimeline|toPlaySubTimeline1,toPlaySubTimeline2,...,toPlaySubTimeline</param>
        public void TimelineManageSubPlayables(string strSubManageParams)
        {
            var funParams = strSubManageParams.Split('|');
            if (funParams.Length != 2)
            {
                Debug.LogError($"[TKSR] Error parameters format of Sub Playables Manager functions : {strSubManageParams}");
                return;
            }

            string strToStopParam = funParams[0];
            string strToPlayParam = funParams[1];
            if (string.IsNullOrEmpty(strToStopParam) || string.IsNullOrEmpty(strToPlayParam))
            {
                Debug.LogError($"[TKSR] Error parameters format of Sub Playables Manager functions : {strSubManageParams}");
                return;
            }

            var arrayToStopSubNames = strToStopParam.Split(',');
            var arrayToPlaySubNames = strToPlayParam.Split(',');
            
            foreach (var timeline in arrayToStopSubNames)
            {
                TimelineStopSubPlayable(timeline);
            }
            
            foreach (var timeline in arrayToPlaySubNames)
            {
                TimelinePlaySubPlayable(timeline);
            }
        }

        /// <summary>
        /// 剧情结束后跳往其他剧情或者场景
        /// </summary>
        public void TimelineEndEventTransition()
        {
            Debug.Log("[TKSR] TimelineEndEventTransition");
            m_enumEndStatus = EnumEndStatus.Normal;
        }
        
        /// <summary>
        /// 剧情结束后跳往其他剧情或者场景
        /// </summary>
        /// <param name="withEntryId">剧情Timeline播放完成后的动作,主要用于在剧情完成进行场景切换时,提供Loading显示的提示文字</param>
        public void TimelineEndEventTransition_WithLoadingText(int withEntryId = 0)
        {
            m_enumEndStatus = EnumEndStatus.WithLoadingText;
            m_EndStatusWithEntryId = withEntryId;
        }
        
        /// <summary>
        /// 跳转到其他场景的时候,显示完整的白屏淡入淡出
        /// </summary>
        public void TimelineEndEventTransition_WithWhiteFader()
        {
            m_enumEndStatus = EnumEndStatus.Fade;
        }
        
        /// <summary>
        /// 某些Timeline结束的时候为了防止穿帮(例如某些SubPlayable动画在主Timeline结束的时候会丢失),在主Timeline的结束位置额外设置了白色的全屏遮罩FadeIn效果,而在Scene切换的时候只设置ScreenFader的白屏FadeOut.
        /// </summary>
        public void TimelineEndEventTransition_WithWhiteOnlyFadeOut()
        {
            m_enumEndStatus = EnumEndStatus.FadeOut;
        }
        
        /// <summary>
        /// 剧情中显示黑屏提示语
        /// </summary>
        /// <param name="entryId"></param>
        public void TimelineShowBlackScreenText(int entryId)
        {
            var blackScreen = this.gameObject.GetComponentInChildren<UIBlackScreenEffect>();
            if (blackScreen != null)
            {
                blackScreen.infoText.text = ParseDialogueContentByEntryId(entryId);
            }
        }
        
        /// <summary>
        /// 显示特效(有可能是暂停当前Timeline等待特效结束后进行恢复,在具体的特效对象脚本上进行Timeline的恢复)
        /// </summary>
        /// <param name="effectParam">特效名称:延迟播放时间:动画额外参数(例如In/Out动画)</param>
        public void TimelineShowEffect(string effectParam)
        {
            Debug.Log($"[TKSR] TimelineShowEffect with effectParam = {effectParam}");
            var data = effectParam.Split(':');
            if (data.Length == 0)
            {
                return;
            }

            var effectName = data[0];
            float effectDuration = 0f;
            if (data.Length > 1)
            {
                effectDuration = float.Parse(data[1]);
            }

            string strExtraParam = string.Empty;
            if (data.Length > 2)
            {
                strExtraParam = data[2];
            }
            EffectManager.Instance.PlayDelayEffect(effectName, effectDuration, strExtraParam);
        }
        
        public void TimelineHideEffect(string effectName)
        {
            EffectManager.Instance.StopEffect(effectName);
        }

        /// <summary>
        /// 在剧情动画中需要隐藏某些Character
        /// </summary>
        /// <param name="characterName"></param>
        public void TimelineHideCharacter(string characterName)
        {
            if (m_dictCharacters.ContainsKey(characterName))
            {
                var gameObjectCharacter = m_dictCharacters[characterName];
                gameObjectCharacter.CharGo.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"[TKSR] Not found character by Name = {characterName}, when hide the character.");
            }
        }

        /// <summary>
        /// 在剧情中直接获取NPC给予的物品列表
        /// </summary>
        /// <param name="itemsList">id:count:duration,id:count:duration,...,[GOLD]:count:duration 最后一个元素如果是GOLD,则说明是金钱</param>
        public void TimelineTransportSomethings(string itemsList)
        {
            if (string.IsNullOrEmpty(itemsList))
            {
                Debug.LogError("[TKSR] Null parameters in TimelineTransportSomethings.");
                return;
            }
            Debug.Log($"[TKSR] TimelineTransportSomethings with parameters = {itemsList}.");
            DocumentDataManager.Instance.TransportDataByTimeline(itemsList);
        }
        
        /// <summary>
        /// 在当前UI最上层显示输入名字的面板
        /// </summary>
        public void TimelineShowGUI_InputName()
        {
            GameUI.Instance.AttachGUIInputNamePanel();
        }

        /// <summary>
        /// 在Timeline运行过程中记录团队记事
        /// </summary>
        /// <param name="strNode"></param>
        public void TimelineSaveStoryNote(string strNode)
        {
            DocumentDataManager.Instance.RecordNoteI2(strNode);
        }

        /// <summary>
        /// 某些剧情需要在运行过程中播放特定的BGM,需要事先在Timeline上设置AudioClip,例如:苍溟登场
        /// </summary>
        public void TimelinePlayBGMAttached()
        {
            if (audioBGM != null)
            {
                BackgroundMusicPlayer.Instance.PlayChangeAmbient(audioBGM);
            }
        }

        /// <summary>
        /// 对应TimelinePlayBGMAttached,在剧情结束后设置BGM不再Loop,而是播放完一次后自动停止
        /// </summary>
        public void TimelineDisableBGMLoop()
        {
            if (audioBGM != null)
            {
                BackgroundMusicPlayer.Instance.NotLoopAmbient();
            }
        }
        
        /// <summary>
        /// 在Timeline中显示Selector对话框
        /// </summary>
        /// <param name="strEntries">选项1,选项2,...选项N</param>
        public void TimelineSimShowResponsesByEntries(string strEntries)
        {
            if (string.IsNullOrEmpty(strEntries))
            {
                Debug.LogError("[TKSR] Null entries parameters.");
                return;
            }
            
            var entries = strEntries.Split(',');
            if (entries == null || entries.Length == 0)
            {
                Debug.LogError("[TKSR] Empty entries parameters.");
                return;
            }
            
            var db = DialogueManager.masterDatabase;
            if (db == null || m_curConversation == null)
            {
                Debug.LogError(("[TKSR] Dialogue database parameter is null."));
                return;
            }
            
            List<Response> listResponse = new List<Response>();
            
            for (int i = 0; i < entries.Length; i++)
            {
                bool tryResult = int.TryParse(entries[i], out int entryId);

                if (!tryResult)
                {
                    Debug.LogError($"[TKSR] Parse entry parameter failed, i = {i}, entry = {entries[i]}");
                    continue;
                }
                
                var entry = m_curConversation.GetDialogueEntry(entryId);
                if (entry != null)
                {
                    var actor = db.GetActor(entry.ActorID);
                    if (!string.IsNullOrEmpty(actor.Name))
                    {
                        if (actor.Name?.CompareTo(ResourceUtils.DIALOGUE_ACTOR_VIRTUAL_SELECTOR) != 0)
                        {
                            Debug.LogError($"[TKSR] There is any actor is not VirtualSelector, i = {i}, entry = {entries[i]}, actor = {actor.Name}");
                            continue;
                        }
                        
                        string subtitleText = entry.subtitleText;
                        if (string.IsNullOrEmpty(subtitleText))
                        {
                            Debug.LogError($"[TKSR] Empty dialogue content with entryId = {entryId} in {m_curConversation.Name}");
                            continue;
                        }
                        
                        
                        var localizedText = DialogueManager.GetLocalizedText(subtitleText);
                        var formattedText = FormattedText.Parse(localizedText, db.emphasisSettings);
                        Response response = new Response(formattedText, entry);

                        Debug.Log($"[TKSR] [{i}] Response.formattedText = {formattedText.text}");
                        listResponse.Add(response);
                    }
                    else
                    {
                        Debug.LogError($"[TKSR] Actor name is null, entryId = {entryId}");
                    }
                }
                else
                {
                    Debug.LogError($"[TKSR] Can't find entry = {entryId}");
                }
            }

            // TODO:优化寻找VirtualSelector
            if (m_VirtualSelector == null)
            {
                var allActors = FindObjectsOfType<DialogueActor>();
                var virtualActor = allActors.FirstOrDefault(x => x.GetActorName()?.CompareTo(ResourceUtils.DIALOGUE_ACTOR_VIRTUAL_SELECTOR) == 0);
                if (virtualActor != null && virtualActor.GetSubtitlePanelNumber() == SubtitlePanelNumber.Custom)
                {
                    var selectPanel = virtualActor.standardDialogueUISettings.customSubtitlePanel.GetComponent<TKSRSelectorSubtitlePanel>();
                    Debug.Log($"[TKSR] Show Responses Count = {listResponse.Count}");
                    m_VirtualSelector = selectPanel;
                }
            }

            if (m_VirtualSelector != null)
            {
                m_VirtualSelector.ShowResponses(listResponse.ToArray(), this);
            }
        }

        /// <summary>
        /// 在剧情播放的时候,显示一些剧情中需要的Text
        /// </summary>
        /// <param name="strI2ContentParam"></param>
        public void TimelineShowScenarioText(string strI2ContentParam)
        {
            if (!string.IsNullOrEmpty(strI2ContentParam))
            {
                var contentData = strI2ContentParam.Split(':');
                float showDuration = 0f;
                string strI2Content = null;
                if (contentData.Length > 0)
                {
                    strI2Content = contentData[0];
                }

                if (contentData.Length > 1)
                {
                    if (float.TryParse(contentData[1], out float duration))
                    {
                        showDuration = duration;
                    }
                }

                GameUI.Instance.toastPanel.ShowToastFixedFormation(strI2Content, showDuration);
            }
        }
        #endregion

        private void ClearTimelineStatusData()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }

            foreach (var charData in m_dictCharacters)
            {
                charData.Value.CharGo.GetComponent<Collider2D>().enabled = charData.Value.InitColliderEnabled;
            }
            
            SceneController.Instance.ResetMainPlayerControlledStatus();

            if (attachedEffects != null)
            {
                attachedEffects.gameObject.SetActive(false);
            }

            if (attachedPathNodes != null)
            {
                attachedPathNodes.gameObject.SetActive(false);
            }
        }
        
        private string ParseDialogueContentByEntryId(int entryId)
        {
            string strDialogContent = null;
            var db = DialogueManager.masterDatabase;
            if (m_curConversation != null)
            {
                var entry = m_curConversation.GetDialogueEntry(entryId);
                if (entry != null)
                {
                    string subtitleText = entry.subtitleText;
                    if (string.IsNullOrEmpty(subtitleText))
                    {
                        Debug.LogError(
                            $"[TKSR] Empty dialogue content with entryId = {entryId} in {m_curConversation.Name}");
                    }
                    else
                    {
                        var localizedText = DialogueManager.GetLocalizedText(subtitleText);
                        var formattedText = FormattedText.Parse(localizedText, db.emphasisSettings);
                        strDialogContent = formattedText.text;
                        Debug.Log($"[TKSR] Dialogue content by entryId = {entryId} is : {strDialogContent}");
                    }
                }
                else
                {
                    Debug.LogError($"[TKSR] Not found dialogue by entryId = {entryId}");
                }
            }
            else
            {
                Debug.LogError("[TKSR] Current conversation is Null");
            }

            return strDialogContent;
        }
        
        /// <summary>
        ///  继续播放当前Timeline
        /// </summary>
        public void ResumeMainTimeline()
        {
            // Debug.Log("[TKSR] ResumeMainTimeline");
            if (m_playableDirector != null)
            {
                //m_playableDirector.playableGraph.GetRootPlayable(0).Play();
                m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
                m_isDialogueClipContinued = true;
            }
        }
        
        private TKSRChatBubbleSubtitlePanel TryGetChatBubbleByName(string charName)
        {
            if (m_dictCharacters.TryGetValue(charName, out TimelineCharData character))
            {
                var actor = character.CharGo.GetComponent<DialogueActor>();
                if (actor.GetSubtitlePanelNumber() == SubtitlePanelNumber.Custom)
                {
                    var chatPanel = actor.standardDialogueUISettings.customSubtitlePanel.GetComponent<TKSRChatBubbleSubtitlePanel>();
                    return chatPanel;
                }
            }

            return null;
        }

        private TKSRSelectorSubtitlePanel m_VirtualSelector;
        
        /// <summary>
        /// 注册成为Dialogue Responses的点击回调
        /// 不在TKSRSelectorSubtitlePanel上注册此消息的原因是,TKSRSelectorSubTitlePanel会在完成一次对话中自动Disable,导致无法接受此消息.
        /// </summary>
        /// <param name="data"></param>
        public void OnClick(object data)
        {
            var resp = data as Response;
            Debug.Log($"[TKSR] OnClick Response conversationID = {resp.destinationEntry.conversationID}, EntryId = {resp.destinationEntry.id}");

            if (m_VirtualSelector != null)
            {
                m_VirtualSelector.HideResponses();
            }

            SwitchNextActionBySelectorResponse(resp);
        }
        
        /// <summary>
        /// 根据Selector Dlg UI的选择,决定接下来的Action
        /// </summary>
        /// <param name="response"></param>
        private void SwitchNextActionBySelectorResponse(Response response)
        {
            if (selectorSwitches == null || selectorSwitches.Length == 0)
            {
                Debug.Log("[TKSR] No selector existed");
                ResumeMainTimeline();
                return;
            }

            var foundSelector = selectorSwitches.FirstOrDefault(x => x.attachedEntryId == response.destinationEntry.id);
            if (foundSelector == null)
            {
                Debug.Log("[TKSR] Not Found a selector item, just run to end timeline.");

                ResumeMainTimeline();
                return;
            }

            m_isInited = false;
            ClearTimelineStatusData();

            TimelineScenarioItem item = foundSelector.gameObject.GetComponent<TimelineScenarioItem>();
            if (item != null)
            {
                // 重新设置当前入口的Timeline,这样Timeline进行对话的Resume的时候才能正常进行
                SceneController.Instance.CurrentEntrance.CurrentTimeline = item;
                
                PlayerCharacter.PlayerInstance.PrepareForPlayTimeline();
                item.InitTimeline(true);
                SceneController.Instance.EnableMainPlayerInput(false);
                SceneController.Instance.EnableMainPlayerPhysicAndGesture(false);
            }
            this.gameObject.SetActive(false);
        }


        public Action<int> CallBackTimelineEnd;
    }
}