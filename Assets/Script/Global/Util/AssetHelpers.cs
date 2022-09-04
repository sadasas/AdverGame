

using AdverGame.Player;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Utility
{
    public class AssetHelpers
    {
        public static List<ItemSerializable> GetAllItemRegistered()
        {
            var so = Resources.LoadAll<ItemContent>(path: "ScriptableObject/Items");
            var items = new List<ItemSerializable>();
            foreach (var s in so)
            {
                var obj = new ItemSerializable(s);
                items.Add(obj);
            }

            return items;
        }

    }
}
