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
        // Garde toujours en mémoire la dernière position de la flèche
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


    // Snap manuellement la flèche à l'endroit de son impacte
    public void SnapArrow(Collision colli)
    {

        // Désactive le collider de la flèche
        DisableColliders();

        // --------------  Fige la flèche  -------------- //

        rbArrow.velocity = Vector3.zero;
        rbArrow.isKinematic = true;


        // --------------  Placement de la flèche -------------- //

        // Donne à la flèche sa dernière position avant impact
        transform.position = lastPosition;
        transform.rotation = lastRotation;

        // Récupère la distance entre la flèche et le point d'impact
        float distanceToTarget = (transform.position - colli.GetContact(0).point).magnitude;
        Debug.Log(distanceToTarget);

        // Déplace la flèche au point d'impact, tout en gardant son angle d'attaque initial
        transform.position += transform.TransformDirection(Vector3.forward) * (distanceToTarget - impactOffset);


        //--------------  Colle la flèche à l'objet touché -------------- //

        // Créé un GameObject tampon pour garder le scale de la flèche
        GameObject sharedParent = new GameObject("Arrow Impact");
        sharedParent.transform.position = colli.GetContact(0).point;
        sharedParent.transform.rotation = colli.transform.rotation;
        sharedParent.transform.parent = colli.gameObject.transform;

        // Assigne le "Tampon" comme parent de la flèche
        transform.parent = sharedParent.transform;
    }


    // Désactive et repousse l'ennemi
    public void EnnemiTouched(Collision colli)
    {
        GameObject ennemi = colli.gameObject;
        Debug.Log("Touché");

        // -----------------  Repousse l'ennemi ----------------- //

        // Applique une force a l'ennemi
        Rigidbody rbEnnemi = colli.rigidbody;
        rbEnnemi.constraints = RigidbodyConstraints.None;
        rbEnnemi.useGravity = true;

        rbEnnemi.AddForceAtPosition((-colli.relativeVelocity).normalized * impactForce, colli.gameObject.transform.position, ForceMode.Impulse);


        // -----------------  Désactive l'ennemi ----------------- //
        
        // Supression de champ de vision 
        ennemi.transform.GetChild(0).gameObject.SetActive(false);
        ennemi.GetComponent<FieldOfView>().enabled = false;

        // Désactiver l'IA...
        // ...
        // ...
    }


    // Détruit la flèche après un delais
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
        // Désactive les colliders
        colldier.enabled = false;
    }

}
