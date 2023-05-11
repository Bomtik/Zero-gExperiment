using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using Controllers.Player;

public class ChaosControl : MonoBehaviour
{
    public static bool Shoot;
    private RaycastHit2D raycastHit2D;
    public static Vector2 Direction;

    private void Update()
    {
        if (PlayerMovementController.States is PlayerState.Controlling || Shoot)
        {
            ControlChaos();
        }
    }

    private void ControlChaos()
    {
        raycastHit2D = Physics2D.Raycast(transform.position, Direction);
        transform.position = Vector2.Lerp(transform.position, (raycastHit2D.point - new Vector2(Direction.x * 0.5f, Direction.y * 0.5f)), Time.deltaTime);

        if (transform.position == raycastHit2D.transform.position)
        {
            PlayerMovementController.States = PlayerState.Walking;
            Shoot = false;
        }

    }
}
