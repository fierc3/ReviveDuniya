using Assets;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class BuildInteractable : MonoBehaviour, PlayerInteractable
{
    private BoxCollider interactionBox = null;

    [SerializeField]
    private GameObject intercationUi = null;
    private Text interactionTextUi = null;

    [SerializeField]
    string notReadyText = "Missing components to build";
    [SerializeField]
    string readyText = "Start Building!";
    string currrentText = "";
    [SerializeField]
    string actionDescription = "Building...";
    bool allItemsReady = false;

 
    private bool playerInRange = false;
    private bool isInInteraction = false;
    private GameObject playerCollider;
    private GameObject playerControllerObject;
    private HumanAnimationController humanAnim;

    // Start is called before the first frame update
    void Start()
    {
        interactionBox = GetComponent<BoxCollider>();

        playerControllerObject = GameObject.FindGameObjectWithTag("PlayerController");
        humanAnim = playerControllerObject.GetComponent<HumanAnimationController>();
    }

    private void UpdateLabels()
    {
        if(Inventory.Instance.CountOfItem(InventoryItem.Water) >= ObjectiveLevel1.MIN_WATER &&
            Inventory.Instance.CountOfItem(InventoryItem.Cobble) >= ObjectiveLevel1.MIN_COBBLE &&
            Inventory.Instance.CountOfItem(InventoryItem.Wood) >= ObjectiveLevel1.MIN_WOOD &&
            Inventory.Instance.CountOfItem(InventoryItem.Seeds) >= ObjectiveLevel1.MIN_SEEDS)
        {
            allItemsReady = true;
            currrentText = $"<{InputManager.interactKeyboard.ToString()}> {readyText}";
        }
        else
        {
            allItemsReady = false;
            currrentText = notReadyText;
        }
        interactionTextUi.text = currrentText;
    }

    // Update is called once per frame
    void Update()
    {
        if (intercationUi == null)
        {
            intercationUi = InteractionUISingleton.Instance.gameObject;
            interactionTextUi = intercationUi.GetComponentInChildren<Text>();
        }

        if (humanAnim.IsDriving() || !allItemsReady) return;
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
            UpdateLabels();
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
        intercationUi.SetActive(false);
        playerControllerObject.GetComponent<PlayerController>().enabled = true;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = false;
        isInInteraction = false;

    }

    public void StartIntercation()
    {
        playerControllerObject.GetComponent<PlayerController>().enabled = false;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = true;

        interactionTextUi.text = actionDescription;
        intercationUi.SetActive(true);
        Invoke(nameof(FinishInteraction), 4);
    }

    private void BuildAnim()
    {

    }


}
