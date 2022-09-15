using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private Vector3 mousePosition;

    public Transform playerTransform;
    public Transform cursorTransform;


    private void Update()
    {
        PlayerRotation();
    }

    // Le joueur suit la souris
    public void PlayerRotation()
    {
        // Récupération de la souris % à la scène
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePosition = raycastHit.point;
        }

        // Rotation du joueur vers le curseur
        Vector3 lookDir = playerTransform.position - mousePosition;
        float angle = Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg + 90f;
        playerTransform.rotation = Quaternion.Euler(0f, -angle, 0f);

        // Affichage du curseur
        cursorTransform.position = mousePosition;
    }

}
