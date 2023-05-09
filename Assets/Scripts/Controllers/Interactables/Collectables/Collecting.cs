using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Controllers.Player;

public class Collecting : MonoSingleton<Collecting>
{
    [SerializeField] private GameObject itemToCollect;
    public bool Collectable = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovementController>().SelectedBomb = this.gameObject;
            Collectable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovementController>().SelectedBomb = itemToCollect;
            Collectable = false;
        }
    }

    public bool Collect(GameObject collectableObject)
    {
        if (Collectable)
        {
            itemToCollect.transform.position = collectableObject.transform.position;
            itemToCollect.transform.parent = collectableObject.transform;

            Destroy(gameObject);
            return true;
        }

        return false;
    }
}