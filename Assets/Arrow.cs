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
            // Fige la flèche
            Debug.Log("Touché");
            rbArrow.velocity = Vector3.zero;
            rbArrow.isKinematic = true;

            // Désactive ses colliders
            DisableColliders();

            // Bruit à l'impact
            impactSound.Play();

            // Détruit la flèche après un delais
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
        // Désactive les colliders
        bodyCollider.enabled = false;
        headColldier.enabled = false;
    }


}
