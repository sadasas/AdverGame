using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{

    /// <summary>
    /// TODO: Refactor updateitem method
    /// </summary>
    public class ItemAvailableHUDHandler : MonoBehaviour
    {
        [SerializeField] Transform m_itemPlace;


        public Dictionary<Item, ItemSerializable> ItemsDisplayed { get; private set; }

        public Action<ItemSerializable> OnUpdateItem;
        public Action OnActive;

        void OnEnable()
        {
            OnActive?.Invoke();
        }
        public void DisplayItem(ItemSerializable item)
        {
            ItemsDisplayed ??= new();
            var obj = Instantiate(item.Content.ItemPrefab, m_itemPlace).GetComponent<Item>();
            obj.UpdateItem(item.Content, item.Stack);

            ItemsDisplayed.Add(obj, item);
            obj.OnTouch += UpdateItem;

        }

        void UpdateItem(Item item)
        {
            OnUpdateItem?.Invoke(ItemsDisplayed[item]);
        }

        public void DestroyItem(Item item)
        {
            ItemsDisplayed.Remove(item);
            Destroy(item.gameObject);
        }
    }
}
