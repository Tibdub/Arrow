using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MovePlayer : MonoBehaviour
{

    public Transform bowCenter; // Debugging


    [SerializeField]
    private Camera mainCamera;
    public TextMeshProUGUI playerSpeedText;
    public GameObject dashImage;


    [Header("Deplacement")]
    public Rigidbody rb;
    public Transform playerTransform;
    public float inputForce;
    public float maxSpeed;
    private float actualSpeed;

    private float vertInput;
    private float horzInput;

    [Header("Rotation")]
    public Transform cursorTransform;
    private Vector3 mousePosition;


    [Header("Dash")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashCooldown = 1f;
    public float momentumFactor;
    bool keepMomentum;

    private bool canDash = true;
    private bool isDashing;


    private void Update()
    {
        MyInput();

        SpeedControl();
        
        PlayerRotation();

        ShowSpeed();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
            PlayerMovment();
    }




    // -_-_-_-_-_-_-_-_-_-_-  Déplacement -_-_-_-_-_-_-_-_-_- //

    //
    private void PlayerMovment()
    {
        // Déplacement du joueur (ZQSD)
        rb.velocity = (Vector3.forward * vertInput) + (Vector3.right * horzInput);
    }

    // Récuperation inputs
    private void MyInput()
    {
        vertInput = Input.GetAxis("Vertical") * inputForce;
        horzInput = Input.GetAxis("Horizontal") * inputForce;
    }

    // Cap la vitesse maximale
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limite la vitesse maximale
        if(actualSpeed > maxSpeed && !isDashing)
        {
            Vector3 limitdVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitdVel.x, rb.velocity.y, limitdVel.z);
        }
    }



    // -_-_-_-_-_-_-_-_-_-_-  Rotation  -_-_-_-_-_-_-_-_-_-_-_- //
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


    // -_-_-_-_-_-_-_-_-_-_-   Dash   -_-_-_-_-_-_-_-_-_-_-_- //

    private IEnumerator Dash()
    {
        dashImage.GetComponent<Image>().color = new Color32(255, 67, 67, 255);
        transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), dashingTime).SetEase(Ease.OutCirc);

        canDash = false;
        isDashing = true;

        // Applique le dash dans la direction du déplacement, si = 0, direction du regard
        if(rb.velocity == Vector3.zero)
        {
            rb.velocity = dashingPower * playerTransform.forward;
        }
        else
        {
            rb.velocity = dashingPower * rb.velocity.normalized;
        }
        

        // Alternative =>  rb.AddForce(dashingPower * playerTransform.forward);

        yield return new WaitForSeconds(dashingTime);
        transform.DOScale(new Vector3(1,1,1), dashingTime*2).SetEase(Ease.OutElastic);
        isDashing = false;
        keepMomentum = true;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        dashImage.GetComponent<Image>().color = new Color32(125, 255, 66, 255);
    }


    // A Finir ...
    //...
    //...
    // Smoothifier la fin du dash
    /*private IEnumerator SmoothLerpMoveSpeed()
    {
        // Transition smooth entre la vitesse de Dash et la vitesse normale
        float time = 0;
        float difference = Mathf.Abs(maxSpeed - actualSpeed);
        float startValue = actualSpeed;

        float boostFactor = momentumFactor;

        while (time < difference)
        {
            //actualSpeed = Mathf.Lerp(startValue, maxSpeed, time / difference);
            Debug.Log(Mathf.Lerp(startValue, maxSpeed, time / difference));

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        actualSpeed = maxSpeed;
        keepMomentum = false;
    }*/


    // -_-_-_-_-_-_-_-_-_-_-   Affichage   -_-_-_-_-_-_-_-_-_-_-_- //

    public void ShowSpeed()
    {
        // Vitesse du joueur
        actualSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        playerSpeedText.text = actualSpeed.ToString("F1");
    }

}
