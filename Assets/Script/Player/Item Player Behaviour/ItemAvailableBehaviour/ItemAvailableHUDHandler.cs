﻿using System;
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

        public Action<ItemSerializable> OnItemTouched;
        public Action OnActive;
        private void OnEnable()
        {
            OnActive?.Invoke();
        }

        void ItemTouched(Item item)
        {
            OnItemTouched?.Invoke(ItemsDisplayed[item]);
        }
        public void DisplayItem(ItemSerializable item)
        {
            ItemsDisplayed ??= new();
            var obj = Instantiate(item.Content.ItemPrefab, m_itemPlace).GetComponent<Item>();
            obj.UpdateItem(item.Content, item.Stack);

            ItemsDisplayed.Add(obj, item);
            obj.OnTouch += ItemTouched;

        }
        public void DestroyItem(Item item)
        {
           
            Destroy(item.gameObject);
        }
        public void RemoveItem(Item item)
        {
            ItemsDisplayed.Remove(item);
         
        }
        public void RemoveItem()
        {
            ItemsDisplayed.Clear();

        }
    }
}
