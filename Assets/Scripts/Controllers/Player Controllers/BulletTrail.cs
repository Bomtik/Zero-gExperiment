using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Bullet
{
    public class BulletTrail : MonoBehaviour
    {
        private Vector3 StartPosition, TargetPosition;
        private float Progress;

        [SerializeField] private float Speed = 40f;

        void Start()
        {
            StartPosition = transform.position.WithAxis(Axis.Z, 1);
        }

        void Update()
        {
            Progress += Time.deltaTime * Speed;
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, Progress);
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            TargetPosition = targetPosition.WithAxis(Axis.Z, 1);
        }
    }
}