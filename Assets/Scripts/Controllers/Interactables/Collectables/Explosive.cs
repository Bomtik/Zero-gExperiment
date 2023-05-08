using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces.Hittable;
using Enums;
using Extensions;

public class Explosive : MonoSingleton<Explosive>, IHittable
{
    public Rigidbody2D rigidBody;
    public ExplosiveState state;
    public LineRenderer lineRenderer;
    private new void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        state = ExplosiveState.Picked;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state is ExplosiveState.Thrown)
        {
            OnHit(collision.gameObject);
        }
    }

    public void OnHit(GameObject other)
    {
        //Play particle and destroy bomb
        Destroy(gameObject);
    }

    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;

        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        return results;
    }

}
