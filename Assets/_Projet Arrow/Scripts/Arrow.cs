using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody rbArrow;
    public Collider colldier;
    public float impactForce;
    public float impactOffset;

    public AudioSource impactSound;

    [SerializeField]
    private bool hasTouched;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        hasTouched = false;
    }


    private void Update()
    {
        // Garde toujours en m�moire la derni�re position de la fl�che
        if (!hasTouched)
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        hasTouched = true;

        SnapArrow(collision);
        impactSound.Play();


        if (collision.gameObject.CompareTag("Ennemi"))
        {
            EnnemiTouched(collision);
        }

        StartCoroutine(Die());
    }


    // Snap manuellement la fl�che � l'endroit de son impacte
    public void SnapArrow(Collision colli)
    {

        // D�sactive le collider de la fl�che
        DisableColliders();

        // --------------  Fige la fl�che  -------------- //

        rbArrow.velocity = Vector3.zero;
        rbArrow.isKinematic = true;


        // --------------  Placement de la fl�che -------------- //

        // Donne � la fl�che sa derni�re position avant impact
        transform.position = lastPosition;
        transform.rotation = lastRotation;

        // R�cup�re la distance entre la fl�che et le point d'impact
        float distanceToTarget = (transform.position - colli.GetContact(0).point).magnitude;
        Debug.Log(distanceToTarget);

        // D�place la fl�che au point d'impact, tout en gardant son angle d'attaque initial
        transform.position += transform.TransformDirection(Vector3.forward) * (distanceToTarget - impactOffset);


        //--------------  Colle la fl�che � l'objet touch� -------------- //

        // Cr�� un GameObject tampon pour garder le scale de la fl�che
        GameObject sharedParent = new GameObject("Arrow Impact");
        sharedParent.transform.position = colli.GetContact(0).point;
        sharedParent.transform.rotation = colli.transform.rotation;
        sharedParent.transform.parent = colli.gameObject.transform;

        // Assigne le "Tampon" comme parent de la fl�che
        transform.parent = sharedParent.transform;
    }


    // D�sactive et repousse l'ennemi
    public void EnnemiTouched(Collision colli)
    {
        GameObject ennemi = colli.gameObject;
        Debug.Log("Touch�");

        // -----------------  Repousse l'ennemi ----------------- //

        // Applique une force a l'ennemi
        Rigidbody rbEnnemi = colli.rigidbody;
        rbEnnemi.constraints = RigidbodyConstraints.None;
        rbEnnemi.useGravity = true;

        rbEnnemi.AddForceAtPosition((-colli.relativeVelocity).normalized * impactForce, colli.gameObject.transform.position, ForceMode.Impulse);


        // -----------------  D�sactive l'ennemi ----------------- //
        
        // Supression de champ de vision 
        ennemi.transform.GetChild(0).gameObject.SetActive(false);
        ennemi.GetComponent<FieldOfView>().enabled = false;

        // D�sactiver l'IA...
        // ...
        // ...
    }


    // D�truit la fl�che apr�s un delais
    IEnumerator Die()
    {
        
        yield return new WaitForSeconds(5); 
        Destroy(transform.parent.gameObject);
    }

    public void EnableColliders()
    {
        // Active les colliders
        colldier.enabled = true;
    }

    public void DisableColliders()
    {
        // D�sactive les colliders
        colldier.enabled = false;
    }

}
