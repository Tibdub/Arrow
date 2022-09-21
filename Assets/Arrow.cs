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

        // --------------  Fige la flèche  -------------- //
        rbArrow.velocity = Vector3.zero;
        rbArrow.isKinematic = true;

        // Créé un GameObject tampon pour garder le scale de la flèche
        GameObject sharedParent = new GameObject("Arrow Impact");
        sharedParent.transform.position = collision.transform.position;
        sharedParent.transform.rotation = collision.transform.rotation;

        sharedParent.transform.parent = collision.gameObject.transform;
        transform.parent = sharedParent.transform;


        // --------------  Divers  -------------- //

        // Désactive ses colliders
        DisableColliders();

        // Bruit à l'impact
        impactSound.Play();

        // Détruit la flèche après un delais
        // ...
        // ...
        


        if (collision.gameObject.CompareTag("Ennemi"))
        {
            GameObject ennemi = collision.gameObject;
            Debug.Log("Touché");

            // Applique une force a l'ennemie
            Rigidbody rbEnnemi = collision.rigidbody;
            rbEnnemi.constraints = RigidbodyConstraints.None;
            rbEnnemi.useGravity = true;

            rbEnnemi.AddForceAtPosition((-collision.relativeVelocity).normalized * impactForce, collision.gameObject.transform.position, ForceMode.Impulse);
            Debug.Log( ((-collision.relativeVelocity).normalized * impactForce ). magnitude);


            // *****   Désactive l'ennemi   ***** //


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
        // Désactive les colliders
        headColldier.enabled = false;
    }


}
