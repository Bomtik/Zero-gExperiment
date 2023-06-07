using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces.Hittable;

public class Rubble : MonoBehaviour, IHittable
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    [SerializeField] private GameObject rubble1, rubble2;
    private void OnCollisionEnter2D(Collision2D collision)
    {

        anim.SetTrigger("boom");
        WhenHit(collision.gameObject);
    }
    public void WhenHit(GameObject other)
    {
        if (other.CompareTag("Explosive"))
        {
            if (gameObject.CompareTag("DoublePlatform"))
            {
                ActivateRubble(rubble1, Vector2.left);
                ActivateRubble(rubble2, Vector2.right);
                transform.DetachChildren();
            }
            Destroy(gameObject);
        }
        
    }

    private void ActivateRubble(GameObject rubble, Vector2 direction)
    {
        rubble.SetActive(true);
        ChaosControl chaosControl = rubble.GetComponent<ChaosControl>();

        if (chaosControl == null) { return; }
        chaosControl.Direction = direction;
        chaosControl.IsShooting = true;
    }
}
