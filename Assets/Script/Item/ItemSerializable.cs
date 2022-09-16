using AdverGame.Utility;
using System;
using UnityEngine;

namespace AdverGame.Player
{
    [Serializable]
    public class ItemSerializable
    {
       [HideInInspector] public string m_content = null;

        public ItemContent Content = null;

        [field: SerializeField]
        public int Stack { get; private set; } = 1;

        public ItemSerializable(ItemContent content)
        {
            Content = content;
        }
        public ItemSerializable()
        {
           
        }

        public void UpdateStack(int stack)
        {
            Stack += stack;
        }

    

        
    }


}
