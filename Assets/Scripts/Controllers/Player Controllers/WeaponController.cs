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

        [SerializeField] private Transform GunPoint;
        [SerializeField] private GameObject BulletTrail;
        #endregion

        #region Private Vars

        private Camera MainCamera;
        private Vector3 MousePos, RotationDegree;

        #endregion
        #endregion

        void Start()
        {
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        void Update()
        {
            Shoot();
            MousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            RotationDegree = MousePos - transform.position;

            float RotZ = Mathf.Atan2(RotationDegree.y, RotationDegree.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, RotZ);

            if (PlayerMovementController.bFacingRight)
            {
                transform.rotation = Quaternion.Euler(0, 0, RotZ + 180);
            }
        }

        private void Shoot()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var Hit = Physics2D.Raycast(GunPoint.position, RotationDegree, (Mathf.Sqrt(MousePos.x) + Mathf.Sqrt(MousePos.y)));
                var Trail = Instantiate(BulletTrail, GunPoint.position, transform.rotation);
                var TrailScript = Trail.GetComponent<BulletTrail>();

                if (Hit.collider != null)
                {
                    TrailScript.SetTargetPosition(Hit.point);
                    /*var Hittable = Hit.collider.GetComponent<IHittable>();
                    Hittable?.Hit();*/
                }

                else
                {
                    TrailScript.SetTargetPosition(MousePos);
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