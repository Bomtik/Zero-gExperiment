using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Player;
using Controllers.Bullet;

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

            float RotZ = Mathf.Atan2(_rotationDegree.y, _rotationDegree.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, RotZ);

            if (PlayerMovementController.FacingRight)
            {
                transform.rotation = Quaternion.Euler(0, 0, RotZ + 180);
            }

            if (Input.GetMouseButtonDown(0))
            {
                var Hit = Physics2D.Raycast(gunPoint.position, _rotationDegree, (Mathf.Sqrt(_mousePos.x) + Mathf.Sqrt(_mousePos.y)));
                var Trail = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
                var TrailScript = Trail.GetComponent<BulletTrail>();

                if (Hit.collider != null)
                {
                    TrailScript.SetTargetPosition(Hit.point);
                    /*var Hittable = Hit.collider.GetComponent<IHittable>();
                    Hittable?.Hit();*/
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
    }
}                    