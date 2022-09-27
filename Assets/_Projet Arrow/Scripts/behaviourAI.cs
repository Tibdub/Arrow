using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class behaviourAI : MonoBehaviour
{

    public FieldOfView fowScript;
    
    [SerializeField]
    private bool playerIsSpoted;


    // Update is called once per frame
    void FixedUpdate()
    {
        // Cherche si le joueur est en vue
        playerIsSpoted = fowScript.FindVisibleTargets();

        if (playerIsSpoted)
        {
            // Stare at player
            fowScript.LookAtTarget();
        }

        
    }
}
