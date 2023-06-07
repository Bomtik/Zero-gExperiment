using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces.Hittable;

public class Rubble : MonoBehaviour, IHittable
{
    Animator anim;
    [SerializeField] private GameObject rubble1, rubble2, stage;

    private void Awake()
    {

        anim = GetComponent<Animator>();
    }
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
                ActivateRubble(rubble1, Vector2.left);
                ActivateRubble(rubble2, Vector2.right);
            }

            anim.SetTrigger("boom");
            StartCoroutine(DelayDestroy());
        }
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1f);
    }
    private void ActivateRubble(GameObject rubble, Vector2 direction)
    {
        rubble.transform.parent = stage.transform;
        rubble.SetActive(true);
        ChaosControl chaosControl = rubble.GetComponent<ChaosControl>();

        if (chaosControl == null) { return; }
        chaosControl.Direction = direction;
        chaosControl.IsShooting = true;
    }
}
