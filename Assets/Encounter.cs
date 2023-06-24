using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Encounter : MonoBehaviour
{
    [SerializeField]
    BossAttacks[] enemies;
    [SerializeField]
    GameObject walls;

    private GameObject intercationUi = null;
    private Text interactionTextUi = null;

    bool encounterStarted = false;
    bool encounterFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && !encounterFinished)
        {
            walls.SetActive(true);
            interactionTextUi.text = "Defeat the bandits!";
            intercationUi.SetActive(true);
            foreach(var enemy in enemies)
            {
                enemy.AI = true;
            }

            encounterStarted = true;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (intercationUi == null)
        {
            intercationUi = InteractionUISingleton.Instance.gameObject;
            interactionTextUi = intercationUi.GetComponentInChildren<Text>();
        }

        if (encounterStarted && !encounterFinished)
        {
            if(enemies.Where(x => x != null).Count() < 1){
                encounterFinished = true;
                walls.SetActive(false);
            }
        }

    }
}
