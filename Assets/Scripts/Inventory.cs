using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory  
    {

        public static event Action<List<InventoryItem>> OnInventoryChange = delegate { };

        private static List<InventoryItem> inventoryList = new List<InventoryItem>();
        private static object lockObject = new object();

        private static List<int> emissions = new List<int>();
        private static List<int> susActions = new List<int>();

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

        public void Clear()
        {
            instance = null;
        }


        public  void AddItem(InventoryItem item)
        {
            lock (lockObject)
            {
                inventoryList.Add(item);
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

        public void AddSusActions(int deduction) => susActions.Add(deduction);
        public List<int> GetSusActions() => susActions;
        public void AddEmissions(int deduction) => emissions.Add(deduction);
        public List<int> GetEmissions() => emissions;
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
