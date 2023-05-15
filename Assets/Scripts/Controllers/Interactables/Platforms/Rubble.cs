using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces.Hittable;

public class Rubble : MonoBehaviour, IHittable
{
    [SerializeField] private GameObject rubble1, rubble2;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        WhenHit(collision.gameObject);
    }
    public void WhenHit(GameObject other)
    {
        if (other.CompareTag("Explosive"))
        {
            if (gameObject.CompareTag("DoublePlatform"))
            {
                rubble1.SetActive(true);
                rubble2.SetActive(true);
                rubble1.GetComponent<ChaosControl>().Direction = Vector2.left;
                rubble1.GetComponent<ChaosControl>().Shoot = true;
                rubble2.GetComponent<ChaosControl>().Direction = Vector2.right;
                rubble2.GetComponent<ChaosControl>().Shoot = true;
            }
            Destroy(gameObject);
        }
    }
}
