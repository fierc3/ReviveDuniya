using Assets.Scripts;
using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintInteractable : MonoBehaviour, PlayerInteractable
{
    [SerializeField]
    private GameObject intercationUi = null;
    private Text interactionTextUi = null;

    [SerializeField]
    string hintText = "I shouldn't go this way...";

    // Start is called before the first frame update
    void Start()
    {
        if (intercationUi != null)
        {
            interactionTextUi = intercationUi.GetComponentInChildren<Text>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (intercationUi == null)
        {
            intercationUi = InteractionUISingleton.Instance.gameObject;
            interactionTextUi = intercationUi.GetComponentInChildren<Text>();
        }

        if (other.gameObject.tag == "Player")
        {
            StartIntercation();
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            FinishInteraction();
        }
    }

    public void FinishInteraction()
    {
        intercationUi.SetActive(false);
    }

    public void StartIntercation()
    {
        interactionTextUi.text = hintText;
        intercationUi.SetActive(true);
    }

}
