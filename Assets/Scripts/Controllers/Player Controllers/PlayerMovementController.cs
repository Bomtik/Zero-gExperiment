using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Data.ValueObjects;
using Enums;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region Self Variables
        public enum PlayerState
        {
            Idle,
            Walking,
            Jumping,
            Collect,
            Shooting,
            Dead
        }
        #region Serialzed Variables

        [SerializeField] private LayerMask PlatformLayerMask;
        #endregion

        #region Private Variables

        [ShowInInspector] private bool _isReadyToPlay = true;

        private Rigidbody2D playerRigidbody2D;
        private BoxCollider2D boxCollider2D;
        private MovementData _data;
        private bool bAllowJumping;
        private float jumpPressedRemember, groundedRemember, move;
        private Vector2 _velocity;

        #endregion
        #region Public Variables

        public PlayerState playerState;
        public GameObject collectableObject, selectedBomb;
        public static bool bFacingRight, canThrowBomb = false;

        #endregion
        #endregion

        // set what needs to be set
        private void Awake()
        {
            playerRigidbody2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            playerState = PlayerState.Idle;
        }

        // get player movement data
        internal void GetMovementData(MovementData movementData)
        {
            _data = movementData;
        }

        // works for rigidbody
        private void FixedUpdate()
        {
            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }

            if (playerState is not PlayerState.Dead) 
            {
                MovePlayer();
            }
        }

        // responsible for every frame update (inputs)
        private void Update()
        {
            jumpPressedRemember -= Time.deltaTime;
            groundedRemember -= Time.deltaTime;

            if (!Grounded() && playerState is not PlayerState.Shooting)
            {
                playerState = PlayerState.Jumping;
            }

            if (playerState is not PlayerState.Jumping)
            {
                groundedRemember = 0.1f;
            }

            if (Input.GetButtonDown("Shoot") && canThrowBomb)
            {
                selectedBomb.GetComponent<LineRenderer>().enabled = true;
                playerState = PlayerState.Shooting;
            }

            if (playerState is PlayerState.Shooting && Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if (Input.GetButtonDown("Jump") && (playerState is PlayerState.Walking || playerState is PlayerState.Jumping))
            {
                playerState = PlayerState.Jumping;
                jumpPressedRemember = 0.2f;
                bAllowJumping = true;
            }

            if (Input.GetButtonDown("Collect") && !canThrowBomb && selectedBomb != null)
            {
                //playerState = PlayerState.Collect;
                if (selectedBomb.GetComponent<Collecting>() == null)
                {
                    return;
                }
                canThrowBomb = selectedBomb.GetComponent<Collecting>().Collect(collectableObject);
            }
        }

        // responsible for player movement 
        private void MovePlayer()
        {
            if (playerState is PlayerState.Shooting)
            {
                StopPlayer();

                Vector2 DragEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _velocity = (DragEndPosition - (Vector2)collectableObject.transform.position);
                _velocity.y -= Physics2D.gravity.y / 2;


                Vector2[] trajectory = Explosive.Instance.Plot(selectedBomb.GetComponent<Explosive>().rigidBody, (Vector2)collectableObject.transform.position, _velocity, 500);
                selectedBomb.GetComponent<Explosive>().lineRenderer.positionCount = trajectory.Length;

                Vector3[] positions = new Vector3[trajectory.Length];
                for (int i = 0; i < trajectory.Length; i++)
                {
                    positions[i] = trajectory[i];
                }
                selectedBomb.GetComponent<Explosive>().lineRenderer.SetPositions(positions);
            }

            if (playerState is PlayerState.Walking || playerState is PlayerState.Jumping)
            {
                move = Input.GetAxis("Horizontal");
                playerRigidbody2D.velocity = new Vector2(move * _data.ForwardSpeed, playerRigidbody2D.velocity.y);

                if (bAllowJumping && jumpPressedRemember > 0 && groundedRemember > 0)
                {
                    jumpPressedRemember = 0f;
                    groundedRemember = 0f;

                    Jump();
                }

                Flip();
            }
        }

        // calls the shooting method form Explosive.cs class + change what needs to be 
        private void Shoot()
        {
            if (collectableObject.transform.childCount > 0)
            {
                collectableObject.GetComponentInChildren<Explosive>().state = ExplosiveState.Thrown;
                collectableObject.transform.DetachChildren();
            }

            Destroy(selectedBomb.GetComponent<LineRenderer>());
            selectedBomb.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            selectedBomb.GetComponent<Explosive>().rigidBody.AddForce(_velocity, ForceMode2D.Impulse);
            canThrowBomb = false;
            playerState = PlayerState.Walking;
        }
        
        // Jump method
        private void Jump()
        {
            playerRigidbody2D.AddForce(Vector2.up * _data.JumpVelocity, ForceMode2D.Impulse);
            bAllowJumping = false;
        }

        // Flipping the player when switching movement direction
        private void Flip()
        {
            if (bFacingRight && move > 0f || !bFacingRight && move < 0f)
            {
                Vector3 LocalScale = this.transform.localScale;
                bFacingRight = !bFacingRight;
                LocalScale.x *= -1f;
                this.transform.localScale = LocalScale;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((other.gameObject.CompareTag("Ground") || Grounded()) && (playerState is not PlayerState.Shooting))
            {
                playerState = PlayerState.Walking;
            }
        }

        // checks when player on ground
        private bool Grounded()
        {
            RaycastHit2D Hitcast2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.05f, PlatformLayerMask);
            return Hitcast2D.collider != null;
        }

        // completely stops player
        private void StopPlayer()
        {
            playerRigidbody2D.velocity = float2.zero;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        internal void OnReset()
        {
            StopPlayer();
            _isReadyToPlay = false;
        }
    }
}