using Assets;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarInteractable : MonoBehaviour, PlayerInteractable
{

    private BoxCollider interactionBox = null;
    private CarController carController = null;
    private CarUserControl carUserControl = null;
    private BoxCollider seatTarget = null;
    private CapsuleCollider exitTarget = null;

    [SerializeField]
    private GameObject intercationUi = null;
    private Text interactionTextUi = null;

    [SerializeField]
    string startText = "Enter car";
    [SerializeField]
    string exitText = "Exit car";

    [SerializeField]
    GameObject exitLocation;


    private bool playerInRange = false;
    private bool isInInteraction = false;
    private GameObject playerCollider;
    private GameObject playerControllerObject;
    private HumanAnimationController humanAnim;


    // Start is called before the first frame update
    void Start()
    {
        startText = $"<{InputManager.interactKeyboard.ToString()}> {startText}";
        exitText = $"<{InputManager.interactKeyboard.ToString()}> {exitText}";
        interactionBox = GetComponent<BoxCollider>();
        carController = GetComponentInParent<CarController>();
        seatTarget = GetComponentInChildren<BoxCollider>();
        exitTarget = GetComponentInChildren<CapsuleCollider>();
        carUserControl = GetComponentInParent<CarUserControl>();
        interactionTextUi = intercationUi.GetComponentInChildren<Text>();
        playerControllerObject = GameObject.FindGameObjectWithTag("PlayerController");
        humanAnim = playerControllerObject.GetComponent<HumanAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetInteract() && (playerInRange || isInInteraction ))
        {

            if (isInInteraction)
            {
                FinishInteraction();
                isInInteraction = false;
            }
            else
            {
                isInInteraction = true;
                StartIntercation();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;
            playerCollider = other.gameObject;
            interactionTextUi.text = startText;
            intercationUi.SetActive(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isInInteraction)
            {
                interactionTextUi.text = exitText;
                intercationUi.SetActive(true);
            }
            else
            {
                playerInRange = false;
                interactionTextUi.text = startText;
                intercationUi.SetActive(false);
            }

        }
    }

    public void FinishInteraction()
    {
        ShowCharacter();
        Invoke(nameof(DelayedAnimDriveActivated), 1f);
        carController.StopAll();
        carController.enabled = false;
        carUserControl.enabled = false;
        intercationUi.SetActive(false);


        playerControllerObject.GetComponent<PlayerController>().enabled = true;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = false;

    }

    public void StartIntercation()
    {
        humanAnim.SetDriving(true);
        playerControllerObject.GetComponent<PlayerController>().enabled = false;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = true;
        carController.enabled = true;
        carUserControl.enabled = true;
        // carController.Move(-10, 0, 0, 1);
        carController.StartAll();
        interactionTextUi.text = exitText;
        intercationUi.SetActive(true);
        Invoke(nameof(HideCharacter), 1);
    }

    private void HideCharacter()
    {
        if (!humanAnim.IsDriving()) return;
        var pos = seatTarget.transform.parent.position;
        pos.y = -100;
        playerControllerObject.transform.position = pos;
    }

    private void DelayedAnimDriveActivated()
    {
        humanAnim.SetDriving(false);
        playerControllerObject.GetComponent<PlayerController>().enabled = true;
    }

    private void ShowCharacter()
    {
        var pos = exitTarget.transform.position;

        playerControllerObject.GetComponentInChildren<CapsuleCollider>().gameObject.transform.position = new Vector3(pos.x, 0, pos.z);
    }
}
