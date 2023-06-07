using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Player;
using Controllers.Bullet;
using Enums;

namespace Controllers.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        Animator anim;
        #region Self Vars

        #region Serialzed Vars

        [SerializeField] private Transform gunPoint;
        [SerializeField] private GameObject bulletTrail;
        #endregion

        #region Private Vars
        private Vector3 _mousePos, _rotationDegree;
        private float _distance;
        private Vector2 _direction;
        private bool _facingRight;
        private PlayerMovementController player;
        private GameObject _hitObject;

        #endregion
        #endregion

        private void Awake()
        {
            anim = GetComponent<Animator>();
            player = FindObjectOfType<PlayerMovementController>();
        }
        void Update()
        {
            HandleInput();
            Shoot();
        }

        private bool HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("pew pew");
                if (Input.GetButton("Up"))
                {
                    _direction = Vector2.up;
                }
                else if (Input.GetButton("Down"))
                {
                    _direction = Vector2.down;
                }
                else if (Input.GetButton("Left"))
                {
                    _direction = Vector2.left;
                }
                else if (Input.GetButton("Right"))
                {
                    _direction = Vector2.right;
                }
                return true;
            }
            return false;
        }

        private void Shoot()
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _rotationDegree = _mousePos - transform.position;
            _distance = _mousePos.x - gunPoint.position.x;

            if (((int)_distance) < 0 && !_facingRight)
            {
                Flip();
            }

            else if (((int)_distance) >  0 && _facingRight)
            {
                Flip();
            }

            float RotZ = Mathf.Atan2(_rotationDegree.y, _rotationDegree.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, RotZ);

            if (_facingRight)
            {
                transform.rotation = Quaternion.Euler(0, 0, RotZ + 180);
            }

            if (HandleInput())
            {
                var hitCast = Physics2D.Raycast(gunPoint.position, _rotationDegree);
                var trail = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
                var trailScript = trail.GetComponent<BulletTrail>();

                if (hitCast.collider != null)
                {
                    trailScript.SetTargetPosition(hitCast.point);
                    _hitObject = hitCast.collider.gameObject;
                    if (!_hitObject.TryGetComponent<ChaosControl>(out ChaosControl chaosControl))
                    {
                        return;
                    }
                    if (!_hitObject.GetComponent<ChaosControl>().IsShooting)
                    {
                        ChaosActivate(_hitObject, _direction);
                    }
                }

                else
                {
                    trailScript.SetTargetPosition(_mousePos);
                    StartCoroutine(DestoryTrail(trail));
                }
            }
        }
        IEnumerator DestoryTrail(GameObject trail)
        {
            yield return new WaitForSeconds(10f);
            Destroy(trail);
        }

        // Flipping the player when switching movement direction
        private void Flip()
        {
            Vector3 LocalScale = player.transform.localScale;
            _facingRight = !_facingRight;
            LocalScale.x *= -1f;
            player.transform.localScale = LocalScale;
        }

        private void ChaosActivate(GameObject control, Vector2 direction)
        {
            if (control.CompareTag("Controllable"))
            {
                PlayerMovementController.States = PlayerState.Controlling;
                var chaosControl = control.GetComponent<ChaosControl>();
                if (chaosControl == null) { return; }
                chaosControl.Direction = direction;
                chaosControl.IsShooting = true;
            }
        }
    }
}                    