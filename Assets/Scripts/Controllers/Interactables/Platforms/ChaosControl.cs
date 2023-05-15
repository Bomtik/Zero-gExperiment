using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using Controllers.Player;

public class ChaosControl : MonoBehaviour
{
    public bool Shoot;
    private RaycastHit2D raycastHit2D;
    public Vector2 Direction;

    private void Update()
    {
        if (PlayerMovementController.States is PlayerState.Controlling || Shoot)
        {
            ControlChaos();
        }
    }

    public void ControlChaos()
    {
        if (Direction == Vector2.zero)
        {
            PlayerMovementController.States = PlayerState.Walking;
            Shoot = false;
            return;
        }
        if (Physics2D.Raycast(transform.position, Direction).point != Vector2.zero)
        {
            raycastHit2D = Physics2D.Raycast(transform.position, Direction);
        }

        transform.position = Vector2.MoveTowards(transform.position, (raycastHit2D.point - new Vector2(Direction.x * 0.5f, Direction.y * 0.5f)), 0.01f);

        if ((Vector2)transform.position == raycastHit2D.point - new Vector2(Direction.x * 0.5f, Direction.y * 0.5f))
        {
            PlayerMovementController.States = PlayerState.Walking;
            Shoot = false;
        }
    }
}
