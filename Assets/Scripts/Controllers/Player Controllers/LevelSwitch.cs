using Controllers.Player;
using DG.Tweening;
using Interfaces.Hittable;
using Signals;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelSwitch : MonoBehaviour, IHittable
{
    [SerializeField] private Vector3 cameraPos;
    [SerializeField] private GameObject door;
    [SerializeField] private Transform target, savePoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WhenHit(other.gameObject);
        }
    }

    public void WhenHit(GameObject other)
    {
        if (gameObject.CompareTag("StageEnd"))
        {
            CoreGameSignals.Instance.onNextLevel?.Invoke();
        }
        if (savePoint != null)
        {
            PlayerMovementController.PlayerPosition = savePoint;
        }
        ChangePositions();
    }

    private void ChangePositions()
    {
        CoreGameSignals.Instance.onStageAreaEntered?.Invoke(cameraPos);
        if (door != null)
        {
            door.transform.DOMove(target.position, 0.5f);
        }
    }
}
