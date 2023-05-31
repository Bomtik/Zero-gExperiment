using Interfaces.Hittable;
using Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitch : MonoBehaviour, IHittable
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WhenHit(other.gameObject);
        }
    }

    public void WhenHit(GameObject other)
    {
        if (gameObject.CompareTag("CameraSwitch"))
        {
            CoreGameSignals.Instance.onStageAreaEntered?.Invoke();
        }

        else if (gameObject.CompareTag("LevelSwitch"))
        {
            CoreGameSignals.Instance.onLevelSuccessful?.Invoke();

        }
    }
}
