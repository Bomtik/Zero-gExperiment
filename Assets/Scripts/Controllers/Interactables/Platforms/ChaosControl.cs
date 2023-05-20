using UnityEngine;
using Enums;
using Controllers.Player;

public class ChaosControl : MonoBehaviour
{
    public bool IsShooting;
    public Vector2 Direction;

    private RaycastHit2D raycastHit2D;

    private void Update()
    {
        if (PlayerMovementController.States is PlayerState.Controlling || IsShooting)
        {
            ControlChaos();
        }
    }

    private void ControlChaos()
    {
        if (Direction == Vector2.zero)
        {
            SetPlayerWalkingState();
            IsShooting = false;
            return;
        }

        PerformRaycast();

        var targetPosition = raycastHit2D.point - new Vector2(Direction.x * 0.5f, Direction.y * 0.5f);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3);

        if ((Vector2)transform.position == targetPosition)
        {
            if (raycastHit2D.collider.gameObject.CompareTag("Button"))
            {
                raycastHit2D.collider.GetComponent<SlidingDoor>().WhenHit(gameObject);
            }
            SetPlayerWalkingState();
            IsShooting = false;
        }
    }
    private void PerformRaycast()
    {
        var raycastHit = Physics2D.Raycast(transform.position, Direction);
        if (raycastHit.point != Vector2.zero)
        {
            raycastHit2D = raycastHit;
        }
    }
    private void SetPlayerWalkingState()
    {
        if (PlayerMovementController.States is PlayerState.Shooting)
        {
            return;
        }
        PlayerMovementController.States = PlayerState.Walking;
    }
}
