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
        #region Serialzed Variables

        [SerializeField] private LayerMask _platformLayerMask;
        #endregion

        #region Private Variables

        [ShowInInspector] private bool _isReadyToPlay = true;

        private Rigidbody2D _playerRigidbody2D;
        private BoxCollider2D _boxCollider2D;
        private MovementData _data;
        private bool _allowJumping;
        private float _jumpPressedRemember, _groundedRemember, _move;
        private Vector2 _velocity;
        private PlayerState _playerState;

        #endregion
        #region Public Variables
        public GameObject CollectableObject, SelectedBomb;
        public static bool FacingRight, ThrowBomb = false;

        #endregion
        #endregion

        // set what needs to be set
        private void Awake()
        {
            SetComponents();
        }

        private void SetComponents()
        {
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _playerState = PlayerState.Idle;
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

            if (_playerState is not PlayerState.Dead) 
            {
                MovePlayer();
            }
        }

        // responsible for every frame update (inputs)
        private void Update()
        {
            _jumpPressedRemember -= Time.deltaTime;
            _groundedRemember -= Time.deltaTime;

            if (!Grounded() && _playerState is not PlayerState.Shooting)
            {
                _playerState = PlayerState.Jumping;
            }

            if (_playerState is not PlayerState.Jumping)
            {
                _groundedRemember = 0.1f;
            }

            if (Input.GetButtonDown("Shoot") && ThrowBomb && _playerState is not PlayerState.Jumping)
            {
                SelectedBomb.GetComponent<LineRenderer>().enabled = true;
                _playerState = PlayerState.Shooting;
            }

            if (_playerState is PlayerState.Shooting && Input.GetButtonUp("Shoot"))
            {
                Shoot();
            }

            if (Input.GetButtonDown("Jump") && (_playerState is PlayerState.Walking || _playerState is PlayerState.Jumping))
            {
                _playerState = PlayerState.Jumping;
                _jumpPressedRemember = 0.2f;
                _allowJumping = true;
            }

            if (Input.GetButtonDown("Collect") && !ThrowBomb && SelectedBomb != null)
            {
                //_playerState = PlayerState.Collect;
                if (SelectedBomb.GetComponent<Collecting>() == null)
                {
                    return;
                }
                ThrowBomb = SelectedBomb.GetComponent<Collecting>().Collect(CollectableObject);
            }
        }

        // responsible for player movement 
        private void MovePlayer()
        {
            if (_playerState is PlayerState.Shooting)
            {
                DisplayTrajectoryLine();
            }

            if (_playerState is PlayerState.Walking || _playerState is PlayerState.Jumping)
            {
                _move = Input.GetAxis("Horizontal");
                _playerRigidbody2D.velocity = new Vector2(_move * _data.ForwardSpeed, _playerRigidbody2D.velocity.y);

                if (_allowJumping && _jumpPressedRemember > 0 && _groundedRemember > 0)
                {
                    _jumpPressedRemember = 0f;
                    _groundedRemember = 0f;

                    Jump();
                }

                Flip();
            }
        }

        // calls the shooting method form Explosive.cs class + change what needs to be 
        private void Shoot()
        {
            if (CollectableObject.transform.childCount > 0)
            {
                CollectableObject.GetComponentInChildren<Explosive>().State = ExplosiveState.Thrown;
                CollectableObject.transform.DetachChildren();
            }

            Destroy(SelectedBomb.GetComponent<LineRenderer>());
            SelectedBomb.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            SelectedBomb.GetComponent<Explosive>().RigidBody.AddForce(_velocity, ForceMode2D.Impulse);
            ThrowBomb = false;
            _playerState = PlayerState.Walking;
        }

        // Trajectory line
        private void DisplayTrajectoryLine()
        {
            StopPlayer();

            Vector2 DragEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _velocity = (DragEndPosition - (Vector2)CollectableObject.transform.position);
            _velocity.y -= Physics2D.gravity.y / 2;

            Vector2[] trajectory = Explosive.Instance.Plot(SelectedBomb.GetComponent<Explosive>().RigidBody, (Vector2)CollectableObject.transform.position, _velocity, 500);
            SelectedBomb.GetComponent<Explosive>().LineRenderer.positionCount = trajectory.Length;

            Vector3[] positions = new Vector3[trajectory.Length];
            for (int i = 0; i < trajectory.Length; i++)
            {
                positions[i] = trajectory[i];
            }
            SelectedBomb.GetComponent<Explosive>().LineRenderer.SetPositions(positions);
        }

        // Jump method
        private void Jump()
        {
            _playerRigidbody2D.AddForce(Vector2.up * _data.JumpVelocity, ForceMode2D.Impulse);
            _allowJumping = false;
        }

        // Flipping the player when switching movement direction
        private void Flip()
        {
            if (FacingRight && _move > 0f || !FacingRight && _move < 0f)
            {
                Vector3 LocalScale = this.transform.localScale;
                FacingRight = !FacingRight;
                LocalScale.x *= -1f;
                this.transform.localScale = LocalScale;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((other.gameObject.CompareTag("Ground") || Grounded()) && (_playerState is not PlayerState.Shooting))
            {
                _playerState = PlayerState.Walking;
            }
        }

        // checks when player on ground
        private bool Grounded()
        {
            RaycastHit2D Hitcast2D = Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0f, Vector2.down, 0.05f, _platformLayerMask);
            return Hitcast2D.collider != null;
        }

        // completely stops player
        private void StopPlayer()
        {
            _playerRigidbody2D.velocity = float2.zero;
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