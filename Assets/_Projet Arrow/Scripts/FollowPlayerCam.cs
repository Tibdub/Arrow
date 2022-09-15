using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCam : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float smoothSpeed;
    

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed);
    }

    public Vector3 GetCamOffest()
    {
        return offset;
    }
}
