using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemContainer
    {
        public List<Item> Items { get; private set; }
        MonoBehaviour m_player;
        public ItemContainer(MonoBehaviour player)
        {
            m_player = player;

            m_player.StartCoroutine(LoadItemsData());
        }

        IEnumerator LoadItemsData()
        {
            var data = PlayerManager.s_Instance.Data.Items;
            if (data != null && data.Count > 0) yield return Items = LoadItemsFromPlayerData();
            else yield return Items = LoadDefaultItemsSet();
        }
        List<Item> LoadItemsFromPlayerData()
        {
            return PlayerManager.s_Instance.Data.Items;
        }

        List<Item> LoadDefaultItemsSet()
        {
            var items = Resources.LoadAll<Item>("Prefab/Items").Where(a => a.m_content != null).ToList();


            return items;
        }

    }

    public class ItemPlayerBehaviour
    {
        public ItemPlayerBehaviour(InputBehaviour inputBehaviour, Transform player, GameObject findItemHUD, GameObject itemListHUD, GameObject itemListButton)
        {
            m_itemContainer = new(player.GetComponent<MonoBehaviour>());
            m_input = inputBehaviour;
            m_player = player;
            m_itemListHUDPrefab = itemListHUD;
            m_itemListButtonPrefab = itemListButton;


            m_input.OnLeftClick += OnLeftClickCallback;
            m_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            m_findItemHandler = new(player.GetComponent<MonoBehaviour>(), m_itemContainer.Items, findItemHUD, m_itemListHUD);
            InitItemListHUD();
            InitItemListButton();


        }

        InputBehaviour m_input;
        Transform m_player;
        ItemContainer m_itemContainer;
        GameObject m_itemListHUDPrefab;

        GameObject m_itemListButtonPrefab;

        ItemListHUDHandler m_itemListHUD;
        FindItemHUDHandler m_findItemManager;
        Transform m_mainCanvas;
        ItemListButton m_itemListButton;
        FindItemHandler m_findItemHandler;


        public void OnDisable()
        {
            m_input.OnLeftClick -= OnLeftClickCallback;
        }

        void OnLeftClickCallback(GameObject obj)
        {
            if (obj == m_player.gameObject)
            {
                m_findItemHandler.InitFindItemHUD();
            }
        }



        void InitItemListButton()
        {
            m_itemListButton = GameObject.Instantiate(m_itemListButtonPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<ItemListButton>();
            m_itemListButton.m_itemListHUD = m_itemListHUD;
        }
        void InitItemListHUD()
        {
            m_itemListHUD = GameObject.Instantiate(m_itemListHUDPrefab, m_mainCanvas).GetComponent<ItemListHUDHandler>();

            foreach (var item in m_itemContainer.Items)
            {
                m_itemListHUD.DisplayItem(item);

            }

            m_findItemHandler.m_itemListHUDHandler = m_itemListHUD;
            m_itemListHUD.gameObject.SetActive(false);

        }

    }


}
