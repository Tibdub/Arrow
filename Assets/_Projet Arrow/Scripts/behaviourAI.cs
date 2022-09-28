using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class behaviourAI : MonoBehaviour
{

    public FieldOfView fowScript;

    public Transform restPosition;

    private GameObject target;

    [SerializeField]
    private bool playerIsSpoted;
    private bool lastPlayerIsSpotted;

    private bool patrollOver;

    private void Start()
    {
        patrollOver = true;
        //StartCoroutine(RotateAround());
    }


    void FixedUpdate()
    {
        // -------------------- PATROUILLE NORMALE -------------------//

        // Patrouille tant que le joueur n'est pas vu
        if (patrollOver && !playerIsSpoted)
        {
            StartCoroutine(RotateAround());
        }

        lastPlayerIsSpotted = playerIsSpoted; // Garde en mémoire le dernier état de détection

        // Cherche si le joueur est en vue
        target = fowScript.FindVisibleTargets();
        playerIsSpoted = target != null; // target(null) => FASLE  |  target(!= null)  => TRUE


        // -------------------- JOUEUR DETECTE -------------------//

        if (playerIsSpoted)
        {
            // 1ère frame ou le joueur entre dans le champ de vision
            if (playerIsSpoted != lastPlayerIsSpotted && playerIsSpoted)
            {
                // Arrète de patrouiller
                StopCoroutine(RotateAround());
                patrollOver = true;

                // Animation sursaute ....
                StartCoroutine(SpottedAnimation());
            }
            else 
                LookAtTarget(); // Regarde le joueur en continue
        }

        // -------------------- JOUEUR DISPARAIT -------------------//

        // Frame ou le joueur sors du champ de vision
        if (playerIsSpoted != lastPlayerIsSpotted && !playerIsSpoted)
        {
            target = null;
        }

    }

    
    // ---------------------------  PATROLL  ------------------------------ // 

    public IEnumerator RotateAround()
    {
        patrollOver = false;

        Debug.Log("Debut patrouille");
        transform.DORotate(new Vector3(0f, transform.eulerAngles.y + 90, 0f), 2f);
        yield return new WaitForSeconds(3);
        transform.DORotate(new Vector3(0f, transform.eulerAngles.y - 90, 0f), 2f);
        yield return new WaitForSeconds(3);

        patrollOver = true;
    }

    public IEnumerator SpottedAnimation()
    {
        Debug.Log("Spotted animation");
        transform.DOMoveY(transform.position.y + 0.8f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        transform.DOMoveY(transform.position.y - 0.8f, 0.3f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.3f);
    }

    public void LookAtTarget()
    {
        /*
        Vector3 dirToTarget = (actualTarget.transform.position - transform.position).normalized;
        Debug.Log("Look at  " + dirToTarget);
        float angle = Mathf.Atan2(dirToTarget.z, dirToTarget.x) * Mathf.Rad2Deg - 90f;
        Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0f, -angle, 0f);*/

        transform.DODynamicLookAt(target.transform.position, 0.2f);
    }

    private IEnumerator PlayerLeaveFow(Transform targetTransform)
    {
        Debug.Log("Attente...");
        transform.DOLookAt(targetTransform.position, 3f);
        yield return new WaitForSeconds(3f);
    }
}
