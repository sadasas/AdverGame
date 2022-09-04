using System.Collections;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemAvailableHandler
    {
        ItemAvailableHUDHandler m_menuAvailableHUDHandler;
        private Transform m_mainCanvas;
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

            PlayerManager.s_Instance.StartCoroutine(Setup());

        }

        IEnumerator Setup()
        {
            InitItemAvailableHUD();
            yield return null;
            InitItemAvailableButton();
        }
        void InitItemAvailableButton()
        {

            m_menuAvailableButtonHandler = GameObject.Instantiate(m_buttonmenuAvailablePrefab, m_mainCanvas).GetComponent<ItemAvailableButtonHandler>();
            m_menuAvailableButtonHandler.m_itemAvailableHUD = m_menuAvailableHUDHandler;

            m_menuAvailableButtonHandler.OnDisplayItemAvailableHUD += UpdateItem;

        }
        void InitItemAvailableHUD()
        {
            m_menuAvailableHUDHandler = GameObject.Instantiate(m_menuAvailableHUDPrefab, m_mainCanvas).GetComponent<ItemAvailableHUDHandler>();

            m_menuAvailableHUDHandler.gameObject.SetActive(false);

        }

        void UpdateItem()
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
                        if (itemDisplayed.Name.text.Equals(itemInventory.Content.Name))
                        {
                            itemDisplayed.UpdateItem(itemInventory.Content, itemInventory.Stack);
                            isSame = true;
                        }
                    }

                    if (!isSame) m_menuAvailableHUDHandler.DisplayItem(itemInventory);
                }

            }
        }

    }


}
