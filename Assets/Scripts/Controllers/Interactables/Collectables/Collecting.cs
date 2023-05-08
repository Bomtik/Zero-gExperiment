using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Controllers.Player;

public class Collecting : MonoSingleton<Collecting>
{
    [SerializeField] private GameObject itemToCollect;
    public bool collectable = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag is "Player")
        {
            collision.GetComponent<PlayerMovementController>().selectedBomb = this.gameObject;
            collectable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag is "Player")
        {
            collision.GetComponent<PlayerMovementController>().selectedBomb = itemToCollect;
            collectable = false;
        }
    }

    public bool Collect(GameObject collectableObject)
    {
        if (collectable)
        {
            itemToCollect.transform.position = collectableObject.transform.position;
            itemToCollect.transform.parent = collectableObject.transform;

            Destroy(gameObject);
            return true;
        }

        return false;
    }
}