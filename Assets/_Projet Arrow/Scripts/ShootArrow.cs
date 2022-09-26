using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    public Transform bowTransform; // Pour la rotation de la fl�che
    public Transform bowCenter; // Pour la position de la fl�che
    public GameObject arrowPrefab;
    public float bowPower; // Puissance de tir

    public AudioSource loadingBowSound;
    public AudioSource shootBowSound;

    private GameObject currentArrow; // Fl�che actuellement charg�e
    private Vector3 shootDir;


    void Update()
    {
        // Chargement du tir
        if (Input.GetMouseButtonDown(0))
        {
            PrepareShoot();
            loadingBowSound.Play();
        }

        // Tir de la fl�che 
        if (Input.GetMouseButtonUp(0))
        {
            Shoot();

            loadingBowSound.Stop();
            shootBowSound.Play();
        }

        // Permet � la fl�che de garder sa position d'attente (sur l'arc)
        if (currentArrow != null)
        {
            currentArrow.GetComponent<Transform>().position = bowCenter.position;
            currentArrow.GetComponent<Transform>().rotation = bowCenter.rotation;
        }
    }


    private void FixedUpdate()
    {
        // R�cup�re & affiche la direction du tir
        shootDir = transform.forward;
        Debug.DrawRay(transform.position, transform.forward * 40, Color.green);
    }



    public void PrepareShoot()
    {
        // Creation du prefab (position au milieu de l'arc / rotation de l'arc)
        currentArrow = Instantiate(arrowPrefab, bowCenter.position, bowTransform.rotation, transform);

        // (Optionnel) Apr�s un delai, la fleche est completement charg�e
        // ...
        // ...
    }


    public void Shoot()
    {

        // D�solidarise la fl�che de son parent
        currentArrow.transform.parent = null;

        // Active les colliders de la fl�che
        currentArrow.GetComponent<Arrow>().EnableColliders();

        // Force et direction de la fl�che
        Vector3 arrowForce = shootDir * bowPower;

        // Tire la fleche
        currentArrow.GetComponent<Rigidbody>().velocity = arrowForce;
        currentArrow = null;


        // (Optionnel) La force de la fl�che d�pend si le tire est charg� compl�tement ou non 
        // ...
        // ...
    }
}
