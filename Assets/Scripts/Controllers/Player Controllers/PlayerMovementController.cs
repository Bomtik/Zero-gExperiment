using UnityEngine;
using Data.ValueObjects;
using Enums;
using Signals;
using Sirenix.OdinInspector;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region Self Variables
        #region Private Variables

        [SerializeField] private LayerMask _platformLayerMask;
        private bool _isReadyToPlay = true;
        private Animator anim;
        private Rigidbody2D _playerRigidbody2D;
        private BoxCollider2D _boxCollider2D;
        private MovementData _data;
        private bool _allowJumping;
        [ShowInInspector]private float _jumpPressedRemember, _groundedRemember, _move;
        private Vector2 _velocity;
        [ShowInInspector]public static PlayerState States;
        public static Transform PlayerPosition;

        #endregion

        #region Public Variables

        public GameObject CollectableObject, SelectedBomb;
        public bool ThrowBomb{ get; private set;  }

        #endregion
        #endregion

        // set what needs to be set
        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            States = PlayerState.Idle;
            anim = GetComponent<Animator>();
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

            if (States is not PlayerState.Dead) 
            {
                MovePlayer();
            }
        }

        // responsible for every frame update (inputs)
        private void Update()
        {
            HandleInputs();
        }

        private void HandleInputs()
        {
            HandleJumpInput();
            HandleShootInput();
            HandleCollectInput();
            HandleRestartInput();
        }

        private void HandleRestartInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CoreGameSignals.Instance.onRestartLevel?.Invoke();
            }
        }

        private void HandleJumpInput()
        {
            _jumpPressedRemember -= Time.deltaTime;
            _groundedRemember -= Time.deltaTime;

            if (!Grounded() && !(States is PlayerState.Shooting))
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("jump", true);
                States = PlayerState.Jumping;
            }

            if (States != PlayerState.Jumping)
            {
                _groundedRemember = 0.1f;
            }

            if (Input.GetButtonDown("Jump") && (States != PlayerState.Shooting || States != PlayerState.Dead))
            {
                States = PlayerState.Jumping;
                _jumpPressedRemember = 0.2f;
                _allowJumping = true;
            }
            else if (States != PlayerState.Jumping)
            {
                anim.SetBool("jump", false);
            }
        }

        private void HandleShootInput()
        {
            if (Input.GetButtonDown("Shoot") && ThrowBomb && States is not PlayerState.Jumping)
            {
                SelectedBomb.GetComponent<LineRenderer>().enabled = true;
                States = PlayerState.Shooting;
            }

            if (States is PlayerState.Shooting && Input.GetButtonUp("Shoot"))
            {
                Shoot();
            }
        }

        private void HandleCollectInput()
        {
            if (Input.GetButtonDown("Collect") && !ThrowBomb && SelectedBomb != null)
            {
                CollectSelectedBomb();
            }
        }

        private void CollectSelectedBomb()
        {
            if (SelectedBomb.GetComponent<Collecting>() == null)
            {
                return;
            }
            ThrowBomb = SelectedBomb.GetComponent<Collecting>().Collect(CollectableObject);
        }

        // responsible for player movement 
        private void MovePlayer()
        {
            if (States is PlayerState.Shooting)
            {
                DisplayTrajectoryLine();
            }

            if (States is PlayerState.Walking || States is PlayerState.Jumping || States is PlayerState.Controlling)
            {
                _move = Input.GetAxis("Horizontal");

                if (_move == 0) { anim.SetBool("isWalking", false); }
                else { anim.SetBool("isWalking", true); }
                _playerRigidbody2D.velocity = new Vector2(_move * _data.ForwardSpeed, _playerRigidbody2D.velocity.y);

                if (_allowJumping && _jumpPressedRemember > 0 && _groundedRemember > 0)
                {
                    _jumpPressedRemember = 0f;
                    _groundedRemember = 0f;

                    Jump();
                }
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
            States = PlayerState.Walking;
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((other.gameObject.CompareTag("Ground") || Grounded()) && (States is not PlayerState.Shooting))
            {
                States = PlayerState.Walking;
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
               _playerRigidbody2D.velocity = Vector2.zero;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        internal void OnReset()
        {
            transform.position = PlayerPosition.position;
        }
    }
}