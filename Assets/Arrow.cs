using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public Collider bodyCollider;
    public Collider headColldier;

    public Rigidbody rbArrow;

    public AudioSource impactSound;

    void OnCollisionEnter(Collision collision)
    {
        Collider myCollider = collision.GetContact(0).thisCollider;

        if (myCollider.tag == "Arrow_head")
        {
            // Fige la fl�che
            Debug.Log("Touch�");
            rbArrow.velocity = Vector3.zero;
            rbArrow.isKinematic = true;

            // D�sactive ses colliders
            DisableColliders();

            // Bruit � l'impact
            impactSound.Play();

            // D�truit la fl�che apr�s un delais
            // ...
            // ...
        }
    }


    public void EnableColliders()
    {
        // Active les colliders
        bodyCollider.enabled = true;
        headColldier.enabled = true;
    }

    public void DisableColliders()
    {
        // D�sactive les colliders
        bodyCollider.enabled = false;
        headColldier.enabled = false;
    }


}
