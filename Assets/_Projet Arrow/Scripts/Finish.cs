using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{

    public GameObject panel;
        

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entrer");

        if (other.CompareTag("Player"))
        {
            if (!panel.activeSelf)
                panel.SetActive(true);
        }
    }
}
