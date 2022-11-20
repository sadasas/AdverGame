

using AdverGame.Customer;
using AdverGame.Player;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Utility
{
    /// <summary>
    /// TODO: make generic function for similiar function get item from asset
    /// </summary>
    public class AssetHelpers
    {
        public static List<ItemSerializable> GetAllItemRegistered()
        {
            var so = Resources.LoadAll<ItemContent>(path: "ScriptableObject/Foods");
            var items = new List<ItemSerializable>();
            foreach (var s in so)
            {
                var obj = new ItemSerializable(s);
                
                items.Add(obj);
            }

            return items;
        }
        public static ItemContent GetScriptableItemRegistered(string name)
        {

            var so = Resources.Load<ItemContent>($"ScriptableObject/Foods/{name}");
            return so;
        }

        public static CustomerVariant[] GetAllCustomerVariantsRegistered()
        {
            var so = Resources.LoadAll<CustomerVariant>("ScriptableObject/Customers");

            return so;
        }

        public static Level[] GetAllLevelVariantRegistered()
        {
            var so = Resources.LoadAll<Level>("ScriptableObject/Levels");

            return so;
        }
    }
}
