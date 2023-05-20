using UnityEngine;
using Extensions;
using Controllers.Player;

public class Collecting : MonoSingleton<Collecting>
{
    [SerializeField] private GameObject collectableItem;
    private bool _isCollectable = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetCollectables(collision, gameObject, true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        SetCollectables(collision, collectableItem, false);
    }

    private void SetCollectables(Collider2D collision, GameObject item, bool isCollectable)
    {
        if (collision.CompareTag("Player") && !collision.GetComponent<PlayerMovementController>().ThrowBomb)
        {
            collision.GetComponent<PlayerMovementController>().SelectedBomb = item;
            _isCollectable = isCollectable;
        }
    }
    public bool Collect(GameObject collectableObject)
    {
        if (_isCollectable)
        {
            collectableItem.transform.position = collectableObject.transform.position;
            collectableItem.transform.SetParent(collectableObject.transform);

            Destroy(gameObject);
            return true;
        }

        return false;
    }
}