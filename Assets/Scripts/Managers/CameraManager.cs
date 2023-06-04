using System;
using Cinemachine;
using DG.Tweening;
using Signals;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        #endregion

        #region Private Variables

        private Vector3 _initialPosition;

        #endregion

        #endregion

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CameraSignals.Instance.onSetCameraTarget += OnSetCameraTarget;
            CoreGameSignals.Instance.onReset += OnReset;
            CoreGameSignals.Instance.onStageAreaEntered += OnStageAreaEntered;
        }

        private void OnSetCameraTarget()
        {
            virtualCamera.Follow = FindObjectOfType<PlayerManager>().transform;
        }

        private void UnSubscribeEvents()
        {
            CameraSignals.Instance.onSetCameraTarget -= OnSetCameraTarget;
            CoreGameSignals.Instance.onReset -= OnReset;
            CoreGameSignals.Instance.onStageAreaEntered -= OnStageAreaEntered;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void Start()
        {
            GetTheInitialPosition();
        }

        private void OnStageAreaEntered(Vector3 cameraPos)
        {
            transform.DOMove(cameraPos, 0.5f);
        }
        private void GetTheInitialPosition()
        {
            _initialPosition = transform.localPosition;
        }

        private void OnReset()
        {
            transform.localPosition = _initialPosition;
            virtualCamera.Follow = null;
        }
    }
}