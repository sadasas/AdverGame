using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemAvailableHUDHandler : MonoBehaviour
    {
        public List<Item> ItemsDisplayed = null;
        [SerializeField] Transform m_itemPlace;

        public void DisplayItem(ItemSerializable item)
        {
            ItemsDisplayed ??= new();
            var obj = Instantiate(item.Content.ItemPrefab, m_itemPlace).GetComponent<Item>();
            obj.UpdateItem(item.Content, item.Stack);

            ItemsDisplayed.Add(obj);

        }
    }
}
