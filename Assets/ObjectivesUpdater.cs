using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesUpdater : MonoBehaviour
{

    private Text objectivesText = null;
    private string template = "";
    // Start is called before the first frame update
    void Start()
    {
        objectivesText = GetComponent<Text>();
        template = objectivesText.text;
        Inventory.OnInventoryChange += UpdateInventory;
        UpdateInventory(new List<InventoryItem>());
    }

    private void UpdateInventory(List<InventoryItem> items)
    {
        /*  
            Collect Cobblestone [stone] / [stoneMax]
            Collect Wood [wood] / [woodMax]
            Collect Water [water] / [watermax]
            Build farm!
         */
        objectivesText.text = template.Replace("[stone]", items.Where(x => x == InventoryItem.Cobble).Count() + "")
        .Replace("[wood]", items.Where(x => x == InventoryItem.Wood).Count() + "")
        .Replace("[water]", items.Where(x => x == InventoryItem.Water).Count() + "")
        .Replace("[seeds]", items.Where(x => x == InventoryItem.Seeds).Count() + "")
        .Replace("[stoneMax]", ObjectiveLevel1.MIN_COBBLE+ "")
        .Replace("[woodMax]", ObjectiveLevel1.MIN_WOOD + "")
        .Replace("[seedsmax]", ObjectiveLevel1.MIN_SEEDS + "")
        .Replace("[watermax]", ObjectiveLevel1.MIN_WATER + "");
    }
}

public class ObjectiveLevel1
{
    public static int MIN_WATER = 1;
    public static int MIN_WOOD = 4;
    public static int MIN_COBBLE = 24;
    public static int MIN_SEEDS = 1;
}
