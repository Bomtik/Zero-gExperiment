using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces.Hittable;

public class Rubble : MonoBehaviour, IHittable
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        WhenHit(collision.gameObject);
    }
    public void WhenHit(GameObject other)
    {
        if (other.CompareTag("Explosive"))
        {
            Destroy(gameObject);
        }
    }
}
