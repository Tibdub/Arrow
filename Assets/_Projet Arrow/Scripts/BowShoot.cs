using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShoot : MonoBehaviour
{
    public Transform playerTransform;
    public Transform bowTransform;
    public Transform bowCenter;
    public GameObject arrowPrefab;
    public float bowPower;

    private GameObject currentArrow;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Chargement fleche");
            PrepareShoot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Flèche tirée !");
            ShootArrow();
        }

        // Permet à la flèche de garder sa position d'attente (sur l'arc)
        if(currentArrow != null)
        {
            currentArrow.GetComponent<Transform>().position = bowCenter.position;
            currentArrow.GetComponent<Transform>().rotation = bowCenter.rotation;
        }
    }

    public void PrepareShoot()
    {
        // Creation du prefab (position au milieu de l'arc / rotation de l'arc)
        currentArrow = Instantiate(arrowPrefab, bowCenter.position, bowTransform.rotation);
        Debug.Log(bowCenter.position);
        Debug.Log(currentArrow.GetComponent<Transform>().position);

        // (Optionnel) Après un delai, la fleche est completement chargée
        // ...
        // ...
    }

    public void ShootArrow()
    {
        // Force et direction de la flèche
        Vector3 arrowForce = playerTransform.forward * bowPower;


        Debug.Log(arrowForce);
        // Tire la fleche
        currentArrow.GetComponent<Rigidbody>().velocity = arrowForce;
        currentArrow = null;


        // (Optionnel) La force de la flèche dépend si le tire est chargé complètement ou non 
        // ...
        // ...
    }
}
