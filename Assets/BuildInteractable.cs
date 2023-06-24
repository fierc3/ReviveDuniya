using Assets;
using Assets.Scripts;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    LevelStopwatch stopwatch;

    [SerializeField]
    GameObject winnerScreen;


    [SerializeField]
    GameObject resultsScreen;

    [SerializeField]
    GameObject goalScreen;

    [SerializeField]
    Text finalResultsText;


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
        if (Inventory.Instance.CountOfItem(InventoryItem.Water) >= ObjectiveLevel1.MIN_WATER &&
            Inventory.Instance.CountOfItem(InventoryItem.Cobble) >= ObjectiveLevel1.MIN_COBBLE &&
            Inventory.Instance.CountOfItem(InventoryItem.Wood) >= ObjectiveLevel1.MIN_WOOD &&
            Inventory.Instance.CountOfItem(InventoryItem.Seeds) >= ObjectiveLevel1.MIN_SEEDS
            )
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

        if (InputManager.GetInteract() && playerInRange && isInInteraction)
        {
            Debug.Log("GO TO MAIN MENU");
            SceneManager.LoadScene("Menu");
        }


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
            playerInRange = false;
            intercationUi.SetActive(false);
        }
    }

    public void FinishInteraction()
    {
        intercationUi.SetActive(false);
        playerControllerObject.GetComponent<PlayerController>().enabled = true;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = false;
        isInInteraction = false;

    }

    public void StartIntercation()
    {
        playerControllerObject.GetComponent<PlayerController>().enabled = false;
        playerControllerObject.GetComponent<Rigidbody>().isKinematic = true;

        AfterWin();

        /*
        interactionTextUi.text = actionDescription;
        intercationUi.SetActive(true);
        Invoke(nameof(FinishInteraction), 4);
        */
    }

    void AfterWin()
    {
        stopwatch.StopStopwatch();
        winnerScreen.SetActive(true);
        goalScreen.SetActive(false);
        intercationUi.SetActive(false);
        Invoke(nameof(DisplayResults), 4f);
        playerInRange = false;

        var playerControllerObject = GameObject.FindGameObjectWithTag("PlayerController");
        var humanAnim = playerControllerObject.GetComponent<HumanAnimationController>();
        humanAnim.DoFinish();
    }

    private void DisplayResults()
    {
        playerInRange = true;
        CalcFinalScore();
        resultsScreen.SetActive(true);
        intercationUi.SetActive(true);
        interactionTextUi.text = $"<{InputManager.interactKeyboard.ToString()}> Go to main menu";        
    }

    private void CalcFinalScore()
    {
        /*
         * Time: {time}
            Time in points: <color=lime>{bruttoPoints}</color>
             <size=25>                        <color=red>- {emission}</color> Emissions
                                    <color=red> - {overprod}</color> Overproduction
                                    <color=red> - {susTrade}</color> Unsustainable Trade
            </size>
            Final SCORE: <b><size=36><color=lime>{nettoPoints}</color></size></b>



         */

        var time = stopwatch.GetElapsedTime();
        var temp = Mathf.Pow(0.6f, 0.003f * time);
        var bruttoScore = 1000 * temp;
        var susActions = Inventory.Instance.GetSusActions().Sum();
        var emissions = Inventory.Instance.GetEmissions().Sum();
        var overWood = Inventory.Instance.CountOfItem(InventoryItem.Wood) - ObjectiveLevel1.MIN_WOOD;
        var overSeeds = Inventory.Instance.CountOfItem(InventoryItem.Seeds) - ObjectiveLevel1.MIN_SEEDS;
        var overCobble = Inventory.Instance.CountOfItem(InventoryItem.Cobble) - ObjectiveLevel1.MIN_COBBLE;
        var overProd = (overWood + overSeeds + overCobble);

        var nettoScore = bruttoScore - (susActions + emissions+ overProd);

        finalResultsText.text = finalResultsText.text.Replace("{time}", time.ToString())
            .Replace("{bruttoPoints}", bruttoScore.ToString())
            .Replace("{emission}", emissions.ToString())
            .Replace("{overprod}", overProd.ToString())
            .Replace("{susTrade}", susActions.ToString())
            .Replace("{nettoPoints}", nettoScore.ToString());

    }


}
