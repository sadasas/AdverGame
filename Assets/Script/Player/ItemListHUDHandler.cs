using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemListHUDHandler : MonoBehaviour
    {
        [SerializeField] Transform m_itemPlace;
        List<Item> m_items;


        public void UpdateItem()
        {
            foreach (var item in m_items)
            {
                item.UpdateItem();
            }
        }
        public void DisplayItem(Item item)
        {
            m_items ??= new();
            m_items.Add(item);
            Instantiate(item.gameObject, m_itemPlace);
        }
    }
}
