using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Interfaces.Hittable;

public class SlidingDoor : MonoBehaviour, IHittable
{
    [SerializeField] private GameObject door, button;
    public void WhenHit(GameObject other)
    {
        button.transform.DOMoveY(button.transform.position.y + 0.2f, 1);
        door.transform.DOMoveY(door.transform.position.y + 4f, 4);
    }
}
