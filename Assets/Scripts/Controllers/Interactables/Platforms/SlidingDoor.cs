using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Interfaces.Hittable;

public class SlidingDoor : MonoBehaviour, IHittable
{
    [SerializeField] private GameObject door, button;
    [SerializeField] private Vector3 doorMovement, buttonMovement;
    public void WhenHit(GameObject other)
    {
        button.transform.DOMove(button.transform.position + buttonMovement, 1);
        door.transform.DOMove(door.transform.position + doorMovement, 4);
    }
}
