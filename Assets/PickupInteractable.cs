using Assets;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PickupInteractable : MonoBehaviour, PlayerInteractable
{
    private BoxCollider interactionBox = null;

    [SerializeField]
    private GameObject intercationUi = null;
    private Text interactionTextUi = null;

    [SerializeField]
    string startText = "Pick up";
    [SerializeField]
    string actionDescription = "Picking up {x}";

    [SerializeField]
    float interactionLock = 2f;

    [SerializeField]
    InventoryItem pickUpItem = InventoryItem.Wood;

    [SerializeField]
    bool destroyOnPickup = true;

    private bool playerInRange = false;
    private bool isInInteraction = false;
    private GameObject playerCollider;
    private GameObject playerControllerObject;
    private HumanAnimationController humanAnim;
    private GameObject holdPos;
    private bool isHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        holdPos = GameObject.FindGameObjectWithTag("HoldPos");
        startText = $"<{InputManager.interactKeyboard.ToString()}> {startText}";
        actionDescription = actionDescription.Replace("{x}", Inventory.GetInventoryDescripton(pickUpItem));
        interactionBox = GetComponent<BoxCollider>();
        if(intercationUi == null)
        {
            intercationUi = InteractionUISingleton.Instance.gameObject;
        }

        interactionTextUi = intercationUi.GetComponentInChildren<Text>();
        playerControllerObject = GameObject.FindGameObjectWithTag("PlayerController");
        humanAnim = playerControllerObject.GetComponent<HumanAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            this.gameObject.transform.position = holdPos.transform.position;
        }

        if (humanAnim.IsDriving()) return;
        if (InputManager.GetInteract() && playerInRange && !isInInteraction)
        {
            isInInteraction = true;
            StartIntercation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (humanAnim.IsDriving() || isInInteraction) return;

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
        if (humanAnim.IsDriving()) return;

        if (other.gameObject.tag == "Player" && !isInInteraction)
        {
            Debug.Log("pum2 EXIT TRIGGERED");
            playerInRange = false;
            intercationUi.SetActive(false);
        }
    }

    public void FinishInteraction()
    {
        Debug.Log("pum2 FINISH TRIGGERED");

        Inventory.Instance.AddItem(pickUpItem);
        intercationUi.SetActive(false);
        playerControllerObject.GetComponent<PlayerController>().enabled = true;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = false;
        isInInteraction = false;
        if (destroyOnPickup)
        {
            this.gameObject.SetActive(false);
        }
        isHeld = false;

    }

    public void StartIntercation()
    {
        playerControllerObject.GetComponent<PlayerController>().enabled = false;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = true;
        PickupAnim();
        interactionTextUi.text = actionDescription;
        intercationUi.SetActive(true);
        Invoke(nameof(FinishInteraction), interactionLock);
    }

    private void PickupAnim()
    {
        humanAnim.StopMovement();
        humanAnim.PickUp();
        Invoke(nameof(AttachToHand), 0.7f);
    }

    private void AttachToHand()
    {
        if (pickUpItem == InventoryItem.Cobble) return;
        isHeld = true;
    }



}
