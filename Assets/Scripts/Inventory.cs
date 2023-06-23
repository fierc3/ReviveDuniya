using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory : MonoBehaviour
    {

        public static event Action<List<InventoryItem>> OnInventoryChange = delegate { };

        private static List<InventoryItem> inventoryList = new List<InventoryItem>();
        private static object lockObject = new object();

        private static Inventory instance;

        private Inventory()
        {
            // Private constructor to prevent instantiation from outside the class.
        }

        public static Inventory Instance
        {
            get
            {
                // Double-checked locking for thread safety.
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new Inventory();
                        }
                    }
                }

                return instance;
            }
        }


        public  void AddItem(InventoryItem item)
        {
            lock (lockObject)
            {
                inventoryList.Add(item);
                Debug.Log("pum2 ab " + inventoryList.Count());
                OnInventoryChange?.Invoke(inventoryList);
            }
        }

        public  void RemoveItem(InventoryItem item)
        {
            lock (lockObject)
            {
                inventoryList.Remove(item);
                OnInventoryChange?.Invoke(inventoryList);
            }
        }

        public int CountOfItem(InventoryItem item)
        {
            return inventoryList.Where(x => x == item).Count();
        }

        public  bool ContainsItem(InventoryItem item)
        {
            lock (lockObject)
            {
                return inventoryList.Contains(item);
            }
        }

        // Other methods or properties

        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return inventoryList.Count;
                }
            }
        }
        public static string GetInventoryDescripton(InventoryItem value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

            return attribute != null ? attribute.Description : value.ToString();
        }
    }

    public enum InventoryItem
    {
        [Description("Wood")]
        Wood,
        [Description("Cobblestone")]
        Cobble,
        [Description("Water")]
        Water,
        [Description("Seeds")]
        Seeds
    }


}
