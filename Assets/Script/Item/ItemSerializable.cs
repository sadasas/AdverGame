using System;
using UnityEngine;

namespace AdverGame.Player
{
    [Serializable]
    public class ItemSerializable
    {
        public ItemContent Content;

        [field: SerializeField]
        public int Stack { get; private set; } = 1;

        public ItemSerializable(ItemContent content)
        {
            Content = content;
        }

        public void UpdateStack(int stack)
        {
            Stack += stack;
        }

    }


}
