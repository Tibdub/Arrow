using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    public Transform bowTransform;
    public Transform bowCenter;
    public GameObject arrowPrefab;
    public float bowPower;

    public AudioSource loadingBowSound;
    public AudioSource shootBowSound;

    private GameObject currentArrow;

    private Vector3 shootDir;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            PrepareShoot();
            loadingBowSound.Play();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("playerTransform.forward 1 " + transform.forward);
            Shoot();

            loadingBowSound.Stop();
            shootBowSound.Play();
        }

        // Permet à la flèche de garder sa position d'attente (sur l'arc)
        if (currentArrow != null)
        {
            currentArrow.GetComponent<Transform>().position = bowCenter.position;
            currentArrow.GetComponent<Transform>().rotation = bowCenter.rotation;
        }
    }


    private void FixedUpdate()
    {
        shootDir = transform.forward;
        Debug.DrawRay(transform.position, transform.forward * 40, Color.green);
    }


    public void PrepareShoot()
    {
        // Creation du prefab (position au milieu de l'arc / rotation de l'arc)
        currentArrow = Instantiate(arrowPrefab, bowCenter.position, bowTransform.rotation, transform);

        // (Optionnel) Après un delai, la fleche est completement chargée
        // ...
        // ...
    }

    public void Shoot()
    {

        // Désolidarise la flèche de son parent
        currentArrow.transform.parent = null;

        // Active les colliders de la flèche
        currentArrow.GetComponent<Arrow>().EnableColliders();

        // Force et direction de la flèche
        Vector3 arrowForce = shootDir * bowPower;

        // Tire la fleche
        currentArrow.GetComponent<Rigidbody>().velocity = arrowForce;
        currentArrow = null;

        Debug.Log("playerTransform.forward : " + transform.forward);
        Debug.Log(arrowForce);


        // (Optionnel) La force de la flèche dépend si le tire est chargé complètement ou non 
        // ...
        // ...
    }
}
