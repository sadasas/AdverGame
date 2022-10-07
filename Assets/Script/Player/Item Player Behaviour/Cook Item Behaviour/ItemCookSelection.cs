using System;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemCookSelection : MonoBehaviour
    {
        public ItemSerializable Content;
        public Action<ItemSerializable> OnChoosed;
        public void Choose()
        {
            OnChoosed?.Invoke(Content);
        }
    }
}
