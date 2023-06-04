using DG.Tweening;
using Interfaces.Hittable;
using Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitch : MonoBehaviour, IHittable
{
    [SerializeField] private Vector3 cameraPos;
    [SerializeField] private GameObject door;
    [SerializeField] private float target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WhenHit(other.gameObject);
        }
    }

    public void WhenHit(GameObject other)
    {
        if (gameObject.CompareTag("StageSwitch"))
        {
            door.transform.DOMoveY(target, 1);
            CoreGameSignals.Instance.onStageAreaEntered?.Invoke(cameraPos);
        }

        else if (gameObject.CompareTag("StageEnd"))
        {
            CoreGameSignals.Instance.onLevelSuccessful?.Invoke();

        }
    }
}
