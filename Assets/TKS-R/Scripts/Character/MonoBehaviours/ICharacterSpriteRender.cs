using System.Collections;
using System.Collections.Generic;
using TKSRPlayables;
using UnityEngine;

namespace TKSR
{
    // [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class ICharacterSpriteRender : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public float maxSpeed = 10f;
        
        protected Vector2 lookDirection = new Vector2(0, -1);
        private bool m_SpriteOriginallyFacesLeft = false;
        
        protected CharacterController2D m_CharacterController2D;
        protected Animator m_Animator;
        protected Vector2 m_MoveVector;

        public static readonly string s_AnimStandingName = "Standing";
        public static readonly string s_AnimSayingName = "Saying";
        public static readonly string s_AnimDrinkingName = "Drinking";
        
        protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("Look X");
        protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("Look Y");
        protected readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
        protected readonly int m_HashDeadPara = Animator.StringToHash("Dead");
        protected readonly int m_HashHurtPara = Animator.StringToHash("Hurt");
        protected readonly int m_HashRunPara = Animator.StringToHash("Run");
        protected readonly int m_HashStandingParam = Animator.StringToHash(s_AnimStandingName);
        protected readonly int m_HashSayingParam = Animator.StringToHash(s_AnimSayingName);
        protected readonly int m_HashDrinkingParam = Animator.StringToHash(s_AnimDrinkingName);

        [SerializeField]
        protected bool isMainPlayer = false;
        
        public CharacterController2D Controller2D
        {
            get
            {
                return m_CharacterController2D;
            }
        }

        protected virtual void Awake()
        {
            m_CharacterController2D = GetComponent<CharacterController2D>();
            m_Animator = GetComponent<Animator>();
        }
        
        protected void UpdateSpriteRendererFlip()
        {
            bool faceLeft = m_MoveVector.x < 0f;
            bool faceRight = m_MoveVector.x > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = true;
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = false;
            }
        }
        
        public void SetMoveVector(Vector2 newMoveVector)
        {
            m_MoveVector = newMoveVector;
            
            // if (m_MoveVector.x * m_MoveVector.y != 0)
            // {
            //     m_MoveVector *= 0.7f;
            // }
        }
        
        public void UpdateFacingDirection(EnumFaceDirection face)
        {
            Vector2 move = Vector2.zero;
            switch (face)
            {
                case EnumFaceDirection.N:
                {
                    move = Vector2.up;
                    UpdateSpriteRendererFlip(false);
                }
                    break;
                case EnumFaceDirection.E:
                {
                    move = Vector2.right;
                    UpdateSpriteRendererFlip(false);
                }
                    break;
                case EnumFaceDirection.S:
                {
                    move = Vector2.down;
                    UpdateSpriteRendererFlip(false);
                }
                    break;
                case EnumFaceDirection.W:
                {
                    move = Vector2.left;
                    UpdateSpriteRendererFlip(true);
                }
                    break;
                case EnumFaceDirection.NE:
                {
                    move = new Vector2(1, 1);                    
                    UpdateSpriteRendererFlip(false);
                }
                    break;
                case EnumFaceDirection.SE:
                {
                    move = new Vector2(1, -1);
                    UpdateSpriteRendererFlip(false);
                }
                    break;
                case EnumFaceDirection.SW:
                {
                    move = new Vector2(-1, -1);
                    UpdateSpriteRendererFlip(true);
                }
                    break;
                case EnumFaceDirection.NW:
                {
                    move = new Vector2(-1, 1);
                    UpdateSpriteRendererFlip(true);
                }
                    break;
                default:
                {
                    throw new UnityException("Error mainRole face direction : " + face);
                }
            }

            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            
            // [TKSR] Bug:防止切换场景时,MainPlayer被SetActive(true)并设置方向的时候,出现一帧默认帧.
            if (m_Animator != null)
            {
                m_Animator.SetFloat(m_HashHorizontalSpeedPara, lookDirection.x);
                m_Animator.SetFloat(m_HashVerticalSpeedPara, lookDirection.y);
            }
            else
            {
                Debug.LogError("[TKSR] No Animator component inited before Awake, maybe in Setter Callback when debug scene with no Transition Point.");
            }
        }
        
        protected void UpdateSpriteRendererFlip(bool faceLeft)
        {
            if (spriteRenderer == null)
            {
                return;
            }
            
            if (faceLeft)
            {
                spriteRenderer.flipX = !m_SpriteOriginallyFacesLeft;
            }
            else
            {
                spriteRenderer.flipX = m_SpriteOriginallyFacesLeft;
            }
        }
        
        public void UpdateSpriteRendererFlip(Vector2 vecDirection)
        {
            bool faceLeft = vecDirection.x < 0f;
            bool faceRight = vecDirection.x > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = !m_SpriteOriginallyFacesLeft;
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = m_SpriteOriginallyFacesLeft;
            }
        }

        protected float GetSpriteRenderFlipLeft()
        {
            return spriteRenderer.flipX != m_SpriteOriginallyFacesLeft ? -1f : 1f;
        }
        
        public Vector2 GetMoveVector()
        {
            return m_MoveVector;
        }
        
        public void StandFaceToTarget(FaceParam param)
        {
            if (param.faceType == FaceParam.FaceType.ToTransform)
            {
                StandFaceToTarget(param.target);
            }
            else if (param.faceType == FaceParam.FaceType.ToPosition)
            {
                StandFaceToTarget(param.destination);
            }
            else
            {
                Debug.LogError("Error Face Type");
            }
        }

        private void StandFaceToTarget(Transform target)
        {
            Vector2 vecTarget = new Vector2();
            var position = target.position;
            vecTarget.x = position.x;
            vecTarget.y = position.y;
            Vector2 vecSelf = new Vector2();
            var position1 = transform.position;
            vecSelf.x = position1.x;
            vecSelf.y = position1.y;
            Vector2 vecDirection = vecTarget - vecSelf;
            StandFaceToByDirection(vecDirection);
        }

        private void StandFaceToTarget(Vector2 vecTarget)
        {
            Vector2 vecSelf = new Vector2();
            var position = transform.position;
            vecSelf.x = position.x;
            vecSelf.y = position.y;
            Vector2 vecDirection = vecTarget - vecSelf;
            StandFaceToByDirection(vecDirection);
        }

        protected virtual void StandFaceToByDirection(Vector2 vecDirection)
        {
            vecDirection = Vector2.ClampMagnitude(vecDirection, 1f);
            var enumDirection = CharacterController2D.VectorToOctDirection(vecDirection);
            if (!isMainPlayer)
            {
                enumDirection = CharacterController2D.VectorToQuadDirection(vecDirection);
            }
            UpdateFacingDirection(enumDirection);
            SetMoveVector(Vector2.zero);
            m_Animator.speed = 1f;
            m_Animator.SetBool(m_HashRunPara, false);
        }
        
        public void MoveFaceToTarget(FaceParam param)
        {
            if (param.faceType == FaceParam.FaceType.ToTransform)
            {
                MoveFaceToTarget(param.target, param.animSpeed);
            }
            else if (param.faceType == FaceParam.FaceType.ToPosition)
            {
                MoveFaceToTarget(param.destination, param.animSpeed);
            }
            else
            {
                Debug.LogError("Error Face Type");
            }
        }

        private void MoveFaceToTarget(Transform target, float speed)
        {
            Vector2 vecTarget = new Vector2();
            var position = target.position;
            vecTarget.x = position.x;
            vecTarget.y = position.y;
            Vector2 vecSelf = new Vector2();
            var position1 = transform.position;
            vecSelf.x = position1.x;
            vecSelf.y = position1.y;
            Vector2 vecDirection = vecTarget - vecSelf;
            MoveFaceToByDirection(vecDirection, speed);
        }

        private void MoveFaceToTarget(Vector2 vecTarget, float speed)
        {
            Vector2 vecSelf = new Vector2();
            var transform1 = transform;
            var position = transform1.position;
            vecSelf.x = position.x;
            vecSelf.y = position.y;
            Vector2 vecDirection = vecTarget - vecSelf;
            MoveFaceToByDirection(vecDirection, speed);
        }

        protected virtual void MoveFaceToByDirection(Vector2 vecDirection, float speed = 1f)
        {
            vecDirection = Vector2.ClampMagnitude(vecDirection, 1f);
            var enumDirection = CharacterController2D.VectorToOctDirection(vecDirection);
            if (!isMainPlayer)
            {
                enumDirection = CharacterController2D.VectorToQuadDirection(vecDirection);
            }
            UpdateFacingDirection(enumDirection);
            m_Animator.SetBool(m_HashRunPara, true);
            m_Animator.speed = speed;
        }
    }
}