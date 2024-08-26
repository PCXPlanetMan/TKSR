using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using PixelCrushers.DialogueSystem;
using UnityEngine;
using PixelCrushers.DialogueSystem.Wrappers;
using Unity.VisualScripting;

namespace TKSR
{
    public class CangMingSpecialDeploy : MonoBehaviour
    {
        public NPCCharacter CangMing;
        public EffectItem CangMingEffect;

        public void GenCangMingNextToMainPlayer()
        {
            var mainPlayer = PlayerCharacter.PlayerInstance;
            var face = mainPlayer.GetCurrentFaceDirectionByAnimation();

            var follow = mainPlayer.transform;

            EnumFaceDirection npcFace = EnumFaceDirection.Invalid;

            float distance = 1f;
            Vector3 cangmingPos = follow.position;
            if (face == EnumFaceDirection.E || face == EnumFaceDirection.N || face == EnumFaceDirection.NE)
            {
                cangmingPos = follow.position + new Vector3(-1, -1, 0) * distance;
                npcFace = EnumFaceDirection.NE;
            }
            else if (face == EnumFaceDirection.W || face == EnumFaceDirection.S || face == EnumFaceDirection.SW)
            {
                cangmingPos = follow.position + new Vector3(1, 1, 0) * distance;
                npcFace = EnumFaceDirection.SW;
            }
            else if (face == EnumFaceDirection.NW)
            {
                cangmingPos = follow.position + new Vector3(1, -1, 0) * distance;
                npcFace = EnumFaceDirection.NW;
            }
            else if (face == EnumFaceDirection.SE)
            {
                cangmingPos = follow.position + new Vector3(-1, 1, 0) * distance;
                npcFace = EnumFaceDirection.SE;
            }

            CangMing.transform.position = cangmingPos;
            CangMingEffect.transform.position = cangmingPos + new Vector3(0, 1f, 0);
            CangMing.faceDirection = npcFace;

            StartCoroutine(CountDownAndShowCangMing());
        }

        public void GenCangMingFrontToMainPlayer()
        {
            var mainPlayer = PlayerCharacter.PlayerInstance;
            var face = mainPlayer.GetCurrentFaceDirectionByAnimation();

            var follow = mainPlayer.transform;

            EnumFaceDirection npcFace = EnumFaceDirection.Invalid;

            float distance = 1f;
            Vector3 cangmingPos = follow.position;
            if (face == EnumFaceDirection.E || face == EnumFaceDirection.N || face == EnumFaceDirection.NE)
            {
                cangmingPos = follow.position + new Vector3(1, 1, 0) * distance;
                npcFace = EnumFaceDirection.SW;
            }
            else if (face == EnumFaceDirection.W || face == EnumFaceDirection.S || face == EnumFaceDirection.SW)
            {
                cangmingPos = follow.position + new Vector3(-1, -1, 0) * distance;
                npcFace = EnumFaceDirection.NE;
            }
            else if (face == EnumFaceDirection.NW)
            {
                cangmingPos = follow.position + new Vector3(-1, 1, 0) * distance;
                npcFace = EnumFaceDirection.SE;
            }
            else if (face == EnumFaceDirection.SE)
            {
                cangmingPos = follow.position + new Vector3(1, -1, 0) * distance;
                npcFace = EnumFaceDirection.NW;
            }

            CangMing.transform.position = cangmingPos;
            CangMingEffect.transform.position = cangmingPos + new Vector3(0, 1f, 0);
            CangMing.faceDirection = npcFace;

            StartCoroutine(CountDownAndShowCangMing());
        }

        public void GenCangMingAroundWithMainPlayer(string strNPCFace)
        {
            var mainPlayer = PlayerCharacter.PlayerInstance;
            var face = mainPlayer.GetCurrentFaceDirectionByAnimation();

            var follow = mainPlayer.transform;

            float distance = 1f;
            Vector3 cangmingPos = follow.position;
            EnumFaceDirection npcFace = EnumFaceDirection.Invalid;
            if (!string.IsNullOrEmpty(strNPCFace))
            {
                if (strNPCFace == "NE")
                {
                    cangmingPos = follow.position + new Vector3(-1, -1, 0) * distance;
                    npcFace = EnumFaceDirection.NE;
                }
                else if (strNPCFace == "SW")
                {
                    cangmingPos = follow.position + new Vector3(1, 1, 0) * distance;
                    npcFace = EnumFaceDirection.SW;
                }
                else if (strNPCFace == "NW")
                {
                    cangmingPos = follow.position + new Vector3(1, -1, 0) * distance;
                    npcFace = EnumFaceDirection.NW;
                }
                else if (strNPCFace == "SE")
                {
                    cangmingPos = follow.position + new Vector3(-1, 1, 0) * distance;
                    npcFace = EnumFaceDirection.SE;
                }
                else
                {
                    Debug.LogError($"[TKSR] Error parameter of NPC Face. strNPCFace = {strNPCFace}");
                }
            }
            else
            {
                if (face == EnumFaceDirection.E || face == EnumFaceDirection.N || face == EnumFaceDirection.NE)
                {
                    cangmingPos = follow.position + new Vector3(-1, -1, 0) * distance;
                    npcFace = EnumFaceDirection.NE;
                }
                else if (face == EnumFaceDirection.W || face == EnumFaceDirection.S || face == EnumFaceDirection.SW)
                {
                    cangmingPos = follow.position + new Vector3(1, 1, 0) * distance;
                    npcFace = EnumFaceDirection.SW;
                }
                else if (face == EnumFaceDirection.NW)
                {
                    cangmingPos = follow.position + new Vector3(1, -1, 0) * distance;
                    npcFace = EnumFaceDirection.NW;
                }
                else if (face == EnumFaceDirection.SE)
                {
                    cangmingPos = follow.position + new Vector3(-1, 1, 0) * distance;
                    npcFace = EnumFaceDirection.SE;
                }
            }

            CangMing.transform.position = cangmingPos;
            CangMingEffect.transform.position = cangmingPos + new Vector3(0, 1f, 0);
            CangMing.faceDirection = npcFace;

            StartCoroutine(CountDownAndShowCangMing());
        }

        IEnumerator CountDownAndShowCangMing()
        {
            CangMingEffect.gameObject.SetActive(true);
            CangMingEffect.PlayEffect("CangMingIn");
            yield return new WaitForSeconds(1);
            CangMing.gameObject.SetActive(true);
            CangMing.Usable = this.gameObject.GetComponent<Usable>();
            yield return new WaitForSeconds(1.5f);
            PixelCrushers.DialogueSystem.Sequencer.Message("KaboomIn");

            m_mainPlayerLastFace = PlayerCharacter.PlayerInstance.GetCurrentFaceDirectionByAnimation();
            if (CangMing.faceDirection == EnumFaceDirection.NE)
            {
                PlayerCharacter.PlayerInstance.UpdateFacingDirection(EnumFaceDirection.SW);
            }
            else if (CangMing.faceDirection == EnumFaceDirection.SE)
            {
                PlayerCharacter.PlayerInstance.UpdateFacingDirection(EnumFaceDirection.NW);
            }
            else if (CangMing.faceDirection == EnumFaceDirection.NW)
            {
                PlayerCharacter.PlayerInstance.UpdateFacingDirection(EnumFaceDirection.SE);
            }
            else if (CangMing.faceDirection == EnumFaceDirection.SW)
            {
                PlayerCharacter.PlayerInstance.UpdateFacingDirection(EnumFaceDirection.NE);
            }
        }

        public void RemoveCangMing()
        {
            StartCoroutine(CountDownAndHideCangMing());
        }

        IEnumerator CountDownAndHideCangMing()
        {
            CangMingEffect.PlayEffect("CangMingIn");
            yield return new WaitForSeconds(1);
            CangMing.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            CangMingEffect.gameObject.SetActive(false);
            PixelCrushers.DialogueSystem.Sequencer.Message("KaboomOut");
            PlayerCharacter.PlayerInstance.UpdateFacingDirection(m_mainPlayerLastFace);
        }

        private EnumFaceDirection m_mainPlayerLastFace;

        public void MakeMainPlayerFaceToCangMing()
        {
            m_mainPlayerLastFace = PlayerCharacter.PlayerInstance.GetCurrentFaceDirectionByAnimation();
        }

        public void ResetMainPlayerOldFace()
        {
            PlayerCharacter.PlayerInstance.UpdateFacingDirection(m_mainPlayerLastFace);
        }
    }
}
