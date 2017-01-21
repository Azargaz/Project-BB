using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupGravityController : MonoBehaviour {

    Controller2D controller;
    Vector2 velocity = Vector2.zero;
    float gravity = 10f;

	void Start ()
    {
        controller = GetComponent<Controller2D>();
	}
	
	void Update ()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
