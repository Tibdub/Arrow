using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public Collider bodyCollider;
    public Collider headColldier;
    public float impactForce;

    public Rigidbody rbArrow;

    public AudioSource impactSound;

    private void Update()
    {
        Debug.Log(rbArrow.velocity);
    }


    void OnCollisionEnter(Collision collision)
    {

        // --------------  Fige la fl�che  -------------- //
        rbArrow.velocity = Vector3.zero;
        rbArrow.isKinematic = true;

        // Cr�� un GameObject tampon pour garder le scale de la fl�che
        GameObject sharedParent = new GameObject("Arrow Impact");
        sharedParent.transform.position = collision.transform.position;
        sharedParent.transform.rotation = collision.transform.rotation;

        sharedParent.transform.parent = collision.gameObject.transform;
        transform.parent = sharedParent.transform;


        // --------------  Divers  -------------- //

        // D�sactive ses colliders
        DisableColliders();

        // Bruit � l'impact
        impactSound.Play();

        // D�truit la fl�che apr�s un delais
        // ...
        // ...
        


        if (collision.gameObject.CompareTag("Ennemi"))
        {
            GameObject ennemi = collision.gameObject;
            Debug.Log("Touch�");

            // Applique une force a l'ennemie
            Rigidbody rbEnnemi = collision.rigidbody;
            rbEnnemi.constraints = RigidbodyConstraints.None;
            rbEnnemi.useGravity = true;

            rbEnnemi.AddForceAtPosition((-collision.relativeVelocity).normalized * impactForce, collision.gameObject.transform.position, ForceMode.Impulse);
            Debug.Log( ((-collision.relativeVelocity).normalized * impactForce ). magnitude);


            // *****   D�sactive l'ennemi   ***** //


            // Supression de champ de vision 
            ennemi.transform.GetChild(0).gameObject.SetActive(false);
            ennemi.GetComponent<FieldOfView>().enabled = false;

        }
    }


    public void EnableColliders()
    {
        // Active les colliders
        headColldier.enabled = true;
    }

    public void DisableColliders()
    {
        // D�sactive les colliders
        headColldier.enabled = false;
    }


}
