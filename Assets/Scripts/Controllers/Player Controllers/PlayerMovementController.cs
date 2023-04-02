using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Data.ValueObjects;

using Controllers.Player;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region Self Variables

        #region Serialzed Variables
        [SerializeField] private Rigidbody2D playerRigidbody2D;
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private LayerMask PlatformLayerMask;
        #endregion

        #region Private Variables

        [ShowInInspector] private bool _isReadyToMove, _isReadyToPlay;
        [ShowInInspector] private MovementData _data;
        private bool bJumping, bAllowJumping;
        private float JumpPressedRemember, GroundedRemember, Move;

        #endregion

        public static bool bFacingRight;
        #endregion

        private void Awake()
        {
            playerRigidbody2D = GetComponentInChildren<Rigidbody2D>();
        }

        internal void GetMovementData(MovementData movementData)
        {
            _data = movementData;
        }

        private void FixedUpdate()
        {
            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }

            if (_isReadyToMove)
            {
                MovePlayer();
            }
        }

        private void Update()
        {
            JumpPressedRemember -= Time.deltaTime;
            GroundedRemember -= Time.deltaTime;

            if (!Grounded())
            {
                bJumping = true;
            }

            if (!bJumping)
            {
                GroundedRemember = 0.1f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                JumpPressedRemember = 0.2f;
                bAllowJumping = true;
            }
        }

        private void MovePlayer()
        {
            Move = Input.GetAxis("Horizontal");
            playerRigidbody2D.velocity = new Vector2(Move * _data.ForwardSpeed, playerRigidbody2D.velocity.y);

            if (bAllowJumping && JumpPressedRemember > 0 && GroundedRemember > 0)
            {
                JumpPressedRemember = 0f;
                GroundedRemember = 0f;

                Jump();
            }

            Flip();
        }

        private void Jump()
        {
            bJumping = true;
            playerRigidbody2D.AddForce(Vector2.up * _data.JumpVelocity, ForceMode2D.Impulse);
            bAllowJumping = false;
        }

        private void Flip()
        {
            if (bFacingRight && Move > 0f || !bFacingRight && Move < 0f)
            {
                Vector3 LocalScale = this.transform.localScale;
                bFacingRight = !bFacingRight;
                LocalScale.x *= -1f;
                this.transform.localScale = LocalScale;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || Grounded())
            {
                bJumping = false;
            }
        }

        private bool Grounded()
        {
            RaycastHit2D Hitcast2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.05f, PlatformLayerMask);
            return Hitcast2D.collider != null;
        }

        private void StopPlayer()
        {
            playerRigidbody2D.velocity = float2.zero;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        internal void IsReadyToMove(bool condition)
        {
            _isReadyToMove = condition;
        }

        internal void OnReset()
        {
            StopPlayer();
            _isReadyToMove = false;
            _isReadyToPlay = false;
        }
    }
}
