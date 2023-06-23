using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory : MonoBehaviour
    {
        private static List<InventoryItem> myList = new List<InventoryItem>();
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
                myList.Add(item);
                //call event
            }
        }

        public  void RemoveItem(InventoryItem item)
        {
            lock (lockObject)
            {
                myList.Remove(item);
            }
        }

        public  bool ContainsItem(InventoryItem item)
        {
            lock (lockObject)
            {
                return myList.Contains(item);
            }
        }

        // Other methods or properties

        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return myList.Count;
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
