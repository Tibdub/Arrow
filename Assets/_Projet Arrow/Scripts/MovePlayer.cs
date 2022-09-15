using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{

    public Rigidbody rb;
    public float moveSpeed;

    void FixedUpdate()
    {
        float vertSpeed = Input.GetAxis("Vertical") * moveSpeed;
        float horzSpeed = Input.GetAxis("Horizontal") * moveSpeed;
        rb.velocity = (Vector3.forward * vertSpeed) + (Vector3.right * horzSpeed);
    }
}
