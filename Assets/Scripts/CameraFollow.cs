using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public int roomSize = 16;
    Vector2 currentRoom;
    public Vector2 offset;
    public float smoothnessX;
    public float smoothnessY;

    void Update ()
    {
		if(target != null)
        {
            currentRoom = new Vector2((int)target.position.x / roomSize, (int)target.position.y / roomSize);
            Vector2 camPos = currentRoom * roomSize + offset;
            Vector2 targetDistance = transform.position - target.position;
            
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, target.position.y, -100), smoothnessY);
            transform.position = Vector3.Lerp(transform.position, new Vector3(camPos.x - targetDistance.x, transform.position.y, -100), smoothnessX);
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
