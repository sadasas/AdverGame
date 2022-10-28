using AdverGame.Customer;
using AdverGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{
    /// <summary>
    /// TODO: Refactor display update item method
    /// </summary>
    public class ItemAvailableHandler
    {
        ItemAvailableHUDHandler m_menuAvailableHUDHandler;
        Transform m_mainCanvas;
        ItemAvailableButtonHandler m_menuAvailableButtonHandler;
        GameObject m_menuAvailableHUDPrefab;
        GameObject m_buttonmenuAvailablePrefab;

        ItemContainer m_itemContainer;



        public ItemAvailableHandler(GameObject HUDmenuAvailablePrefab, GameObject buttonmenuAvailablePrefab, ItemContainer itemContainer)
        {

            m_menuAvailableHUDPrefab = HUDmenuAvailablePrefab;
            m_buttonmenuAvailablePrefab = buttonmenuAvailablePrefab;

            m_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            m_itemContainer = itemContainer;

            InitItemAvailableHUD();
            InitItemAvailableButton();

        }


        void InitItemAvailableButton()
        {

            m_menuAvailableButtonHandler = GameObject.Instantiate(m_buttonmenuAvailablePrefab, m_mainCanvas).GetComponent<ItemAvailableButtonHandler>();
            m_menuAvailableButtonHandler.m_itemAvailableHUD = m_menuAvailableHUDHandler;
            m_menuAvailableButtonHandler.transform.SetAsFirstSibling();


        }
        void InitItemAvailableHUD()
        {
            m_menuAvailableHUDHandler = GameObject.Instantiate(m_menuAvailableHUDPrefab, m_mainCanvas).GetComponent<ItemAvailableHUDHandler>();
            m_menuAvailableHUDHandler.OnItemTouched += ItemTouched;
            m_menuAvailableHUDHandler.OnActive += DisplayUpdateItem;
            m_menuAvailableHUDHandler.gameObject.SetActive(false);
            UIManager.s_Instance.HUDRegistered.Add(HUDName.ITEM_AVAILABLE, m_menuAvailableHUDHandler.gameObject);

        

        }


        void DisplayUpdateItem()
        {
            if (m_itemContainer.Items == null || m_itemContainer.Items.Count == 0)
            {
                if (m_menuAvailableHUDHandler.ItemsDisplayed != null && m_menuAvailableHUDHandler.ItemsDisplayed.Count > 0)
                {
                    foreach (var item in m_menuAvailableHUDHandler.ItemsDisplayed)
                    {

                        m_menuAvailableHUDHandler.DestroyItem(item.Key);
                    }
                    m_menuAvailableHUDHandler.RemoveItem();
                }

                return;
            }
            if (m_menuAvailableHUDHandler.ItemsDisplayed == null || m_menuAvailableHUDHandler.ItemsDisplayed.Count == 0)
            {

                foreach (var item in m_itemContainer.Items)
                {
                    m_menuAvailableHUDHandler.DisplayItem(item);
                }

            }
            else
            {
                if (m_menuAvailableHUDHandler.ItemsDisplayed.Count > m_itemContainer.Items.Count)
                {
                    var itemTrash = new List<Item>();
                    foreach (var itemDisplayed in m_menuAvailableHUDHandler.ItemsDisplayed)
                    {
                        var isSame = false;
                        foreach (var item in m_itemContainer.Items)
                        {
                            if (item == itemDisplayed.Value) isSame = true;
                        }
                        if (!isSame) itemTrash.Add(itemDisplayed.Key);
                    }
                    if (itemTrash.Count > 0)
                    {
                        foreach (var item in itemTrash)
                        {

                            m_menuAvailableHUDHandler.DestroyItem(item);
                            m_menuAvailableHUDHandler.RemoveItem(item);
                        }
                    }

                }
                foreach (var itemInventory in m_itemContainer.Items)
                {
                    var isSame = false;
                    foreach (var itemDisplayed in m_menuAvailableHUDHandler.ItemsDisplayed)
                    {
                        if (itemDisplayed.Value == itemInventory)
                        {
                            /// VIOLETE SINGLE RESPONABILITY PRINCIPLE
                            itemDisplayed.Key.UpdateItem(itemInventory.Content, itemInventory.Stack);
                            isSame = true;
                        }
                    }

                    if (!isSame) m_menuAvailableHUDHandler.DisplayItem(itemInventory);
                }

            }
        }

        void ItemTouched(ItemSerializable item)
        {

            if (CustomerManager.s_Instance.CheckOrder(item, out var order))
            {
                CustomerManager.s_Instance.GetOrder(order);
                m_itemContainer.DecreaseItem(item);
                DisplayUpdateItem();
            }

        }

    }


}
