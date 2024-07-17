using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using TKSRPlayables;
using UnityEngine;

namespace TKSR
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterController2D : MonoBehaviour
    {
        Rigidbody2D m_Rigidbody2D;
        Vector2 m_PreviousPosition;
        Vector2 m_CurrentPosition;
        Vector2 m_NextMovement;
        private Collider2D m_Collider2D;

        public Vector2 Velocity { get; protected set; }

        public Rigidbody2D Rigidbody2D
        {
            get { return m_Rigidbody2D; }
        }

        public Collider2D Collider2D
        {
            get
            {
                return m_Collider2D;
            }
        }

        void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Collider2D = GetComponent<Collider2D>();

            m_CurrentPosition = m_Rigidbody2D.position;
            m_PreviousPosition = m_Rigidbody2D.position;
            
            Physics2D.queriesStartInColliders = false;
        }

        void FixedUpdate()
        {
            m_PreviousPosition = m_Rigidbody2D.position;
            m_CurrentPosition = m_PreviousPosition + m_NextMovement;
            Velocity = (m_CurrentPosition - m_PreviousPosition) / Time.deltaTime;

            m_Rigidbody2D.MovePosition(m_CurrentPosition);
            m_NextMovement = Vector2.zero;
        }

        /// <summary>
        /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
        /// </summary>
        /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
        public void Move(Vector2 movement)
        {
            m_NextMovement += movement;
        }

        public void ZeroMovement()
        {
            m_NextMovement = Vector2.zero;
        }

        /// <summary>
        /// This moves the character without any implied velocity.
        /// </summary>
        /// <param name="position">The new position of the character in global space.</param>
        public void Teleport(Vector2 position)
        {
            Vector2 delta = position - m_CurrentPosition;
            m_PreviousPosition += delta;
            m_CurrentPosition = position;
            m_Rigidbody2D.MovePosition(position);
        }

        public static EnumFaceDirection VectorToQuadDirection(Vector2 vecDirection)
        {
            EnumFaceDirection face = EnumFaceDirection.SW;
            if (vecDirection.x >= 0 && vecDirection.y >= 0)
            {
                face = EnumFaceDirection.NE;
            }
            else if (vecDirection.x >= 0 && vecDirection.y < 0)
            {
                face = EnumFaceDirection.SE;
            }
            else if (vecDirection.x < 0 && vecDirection.y >= 0)
            {
                face = EnumFaceDirection.NW;
            }

            return face;
        }

        /// <summary>
        /// TODO:在椭圆ISO 2.5 TileMap的视角下,计算八方向角度
        /// </summary>
        /// <param name="vecDirection"></param>
        /// <returns></returns>
        public static EnumFaceDirection VectorToOctDirection(Vector2 vecDirection)
        {
            EnumFaceDirection face = EnumFaceDirection.N;
            if (vecDirection.x == 0 && vecDirection.y == 0)
            {
                return face;
            }
            else if (vecDirection.x * vecDirection.y == 0)
            {
                if (vecDirection.x > 0)
                {
                    face = EnumFaceDirection.E;
                }
                else if (vecDirection.x < 0)
                {
                    face = EnumFaceDirection.W;
                }
                else if (vecDirection.y > 0)
                {
                    face = EnumFaceDirection.N;
                }
                else if (vecDirection.y < 0)
                {
                    face = EnumFaceDirection.S;
                }
            }
            else
            {
                float abs_X = Mathf.Abs(vecDirection.x);
                float abs_Y = Mathf.Abs(vecDirection.y);
                var degree = Mathf.Rad2Deg * Mathf.Atan2(abs_Y, abs_X);
                if (vecDirection.x > 0 && vecDirection.y > 0)
                {
                    // if (degree < 22.5f)
                    if (degree < 15f)
                    {
                        face = EnumFaceDirection.E;
                    }
                    // else if (degree > 67.5f)
                    else if (degree > 75f)    
                    {
                        face = EnumFaceDirection.N;
                    }
                    else
                    {
                        face = EnumFaceDirection.NE;
                    }
                }
                else if (vecDirection.x > 0 && vecDirection.y < 0)
                {
                    // if (degree < 22.5f)
                    if (degree < 15f)
                    {
                        face = EnumFaceDirection.E;
                    }
                    // else if (degree > 67.5f)
                    else if (degree > 75f)
                    {
                        face = EnumFaceDirection.S;
                    }
                    else
                    {
                        face = EnumFaceDirection.SE;
                    }
                }
                else if (vecDirection.x < 0 && vecDirection.y > 0)
                {
                    // if (degree < 22.5f)
                    if (degree < 15f)
                    {
                        face = EnumFaceDirection.W;
                    }
                    // else if (degree > 67.5f)
                    else if (degree > 75f)
                    {
                        face = EnumFaceDirection.N;
                    }
                    else
                    {
                        face = EnumFaceDirection.NW;
                    }
                }
                else
                {
                    // if (degree < 22.5f)
                    if (degree < 15f)
                    {
                        face = EnumFaceDirection.W;
                    }
                    // else if (degree > 67.5f)
                    else if (degree > 75f)
                    {
                        face = EnumFaceDirection.S;
                    }
                    else
                    {
                        face = EnumFaceDirection.SW;
                    }
                }
            }
            
            return face;
        }
    }
}