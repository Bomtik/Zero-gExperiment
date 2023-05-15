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

        #endregion
        #endregion

        void Update()
        {
            Shoot();
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

            if (Input.GetMouseButtonDown(0) && PlayerMovementController.States is not PlayerState.Controlling)
            {
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
                else
                {
                    //_direction = Vector2.zero;
                }

                var Hit = Physics2D.Raycast(gunPoint.position, _rotationDegree);
                var Trail = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
                var TrailScript = Trail.GetComponent<BulletTrail>();

                if (Hit.collider != null)
                {
                    TrailScript.SetTargetPosition(Hit.point);
                    ChaosActivate(Hit.collider.gameObject, _direction);
                }

                else
                {
                    TrailScript.SetTargetPosition(_mousePos);
                    StartCoroutine(DestoryTrail());
                }

                IEnumerator DestoryTrail()
                {
                    yield return new WaitForSeconds(10f);
                    Destroy(Trail);
                }
            }
        }

        // Flipping the player when switching movement direction
        private void Flip()
        {
            GameObject player = FindObjectOfType<PlayerMovementController>().gameObject;
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
                control.GetComponent<ChaosControl>().Direction = direction;
                control.GetComponent<ChaosControl>().Shoot = true;
            }
        }
    }
}                    