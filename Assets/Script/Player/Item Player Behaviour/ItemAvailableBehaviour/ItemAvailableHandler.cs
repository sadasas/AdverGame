using AdverGame.Customer;
using AdverGame.UI;
using AdverGame.Utility;
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
        List<ItemSerializable> m_allItems;
        ItemContainer m_itemContainer;



        public ItemAvailableHandler(GameObject HUDmenuAvailablePrefab, GameObject buttonmenuAvailablePrefab, ItemContainer itemContainer)
        {

            m_menuAvailableHUDPrefab = HUDmenuAvailablePrefab;
            m_buttonmenuAvailablePrefab = buttonmenuAvailablePrefab;

            m_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            m_itemContainer = itemContainer;
            m_allItems = AssetHelpers.GetAllItemRegistered();
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
            //ResolutionHelper.ScaleToFitScreen(m_menuAvailableHUDHandler.gameObject);
            foreach (var item in m_allItems)
            {
                item.IncreaseStack(-1);
                
                m_menuAvailableHUDHandler.DisplayItem(item);
            }

            m_menuAvailableHUDHandler.gameObject.SetActive(false);
            UIManager.s_Instance.HUDRegistered.Add(HUDName.ITEM_AVAILABLE, m_menuAvailableHUDHandler.gameObject);



        }


        void DisplayUpdateItem()
        {
            if (m_itemContainer.Items == null || m_itemContainer.Items.Count == 0) return;

            if (m_menuAvailableHUDHandler.ItemsDisplayed == null || m_menuAvailableHUDHandler.ItemsDisplayed.Count == 0)
            {

                foreach (var item in m_itemContainer.Items)
                {
                    m_menuAvailableHUDHandler.DisplayItem(item);
                }

            }
            else
            {

                foreach (var itemInventory in m_itemContainer.Items)
                {
                    var isSame = false;
                    foreach (var itemDisplayed in m_menuAvailableHUDHandler.ItemsDisplayed)
                    {
                        if (itemDisplayed.Value.Content.Name.Equals(itemInventory.Content.Name))
                        {
                            /// VIOLANCE SINGLE RESPONABILITY PRINCIPLE
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



        public void SelectItemInHUD(ItemSerializable item)
        {
            m_menuAvailableHUDHandler.SelectItem(item);
        }

        public void UnselectItemInHUD()
        {
            m_menuAvailableHUDHandler.UnselectItem();
        }
    }


}
