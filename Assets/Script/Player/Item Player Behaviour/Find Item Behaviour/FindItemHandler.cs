using AdverGame.UI;
using AdverGame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{
    public class FindItemHandler
    {

        List<ItemSerializable> m_allItems;
        int m_searchItemTime = 5;
        FindItemHUDHandler m_HUDHandler;
        GameObject m_findItemHUDPrefab;
        List<ItemSerializable> m_ItemFoundedND;
        MonoBehaviour m_player;
        int m_maxItemGetted = 8;
        ItemContainer m_itemContainer;
        bool isSearching = false;
        Action<int> OnFindItem;
        public List<ItemSerializable> ItemFounded { get; private set; }

        public FindItemHandler(MonoBehaviour player, GameObject findItemHUDPrefab, int searchItemTime, int maxItemGetted, ItemContainer itemContainer)
        {
            m_findItemHUDPrefab = findItemHUDPrefab;
            m_player = player;
            m_searchItemTime = searchItemTime;
            m_maxItemGetted = maxItemGetted;

            m_allItems = AssetHelpers.GetAllItemRegistered();

            InitFindItemHUD();
            StartFindItem();
            m_itemContainer = itemContainer;
        }
        void StartFindItem()
        {
            if (!isSearching) m_player.StartCoroutine(FindItem());
        }
        IEnumerator FindItem()
        {
            isSearching = true;

            ItemFounded ??= new();
            while (ItemFounded.Count < m_maxItemGetted)
            {

                //Find item
                var index = UnityEngine.Random.Range(0, m_allItems.Count - 1);
                ItemFounded.Add(new ItemSerializable(m_allItems[index].Content));

                OnFindItem?.Invoke(ItemFounded.Count);

                //display item
                Debug.Log("searching");
                if (m_HUDHandler != null)
                {
                    if (m_HUDHandler.gameObject.activeInHierarchy) m_HUDHandler.DisplayItemFounded(m_allItems[index].Content.ItemPrefab);
                    else
                    {
                        m_ItemFoundedND ??= new();
                        m_ItemFoundedND.Add(m_allItems[index]);

                    }
                }
                else
                {
                    m_ItemFoundedND ??= new();
                    m_ItemFoundedND.Add(m_allItems[index]);
                }
                yield return new WaitForSeconds(m_searchItemTime);


            }

            isSearching = false;

        }

        void PutItemFinded()
        {


            foreach (var item in ItemFounded)
            {
                if (m_itemContainer.Items != null && m_itemContainer.Items.Count > 0)
                {
                    var isSameItem = false;
                    foreach (var itempl in m_itemContainer.Items)
                    {
                        if (itempl.Content.Name.Equals(item.Content.Name))
                        {
                            itempl.IncreaseItem(item.Stack);
                            isSameItem = true;
                            break;
                        }
                        if (!isSameItem) m_itemContainer.AddItem(item);
                    }
                }
                else m_itemContainer.AddItem(newItem: item);

            }

            ResetItemFounded();




        }
        void ResetItemFounded()
        {
            destroyItemTemp();
            StartFindItem();
        }
        void destroyItemTemp()
        {
            m_ItemFoundedND = new();

            ItemFounded = new();

        }
        public void InitFindItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_findItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<FindItemHUDHandler>();
                m_HUDHandler.OnGetTriggered += PutItemFinded;
                OnFindItem += m_HUDHandler.TrackItemFinded;
                m_HUDHandler.gameObject.SetActive(false);

            }
            else
            {
                m_HUDHandler.gameObject.SetActive(true);
                if (ItemFounded.Count > 0)
                {
                    if (m_HUDHandler.m_itemDisplayed != null && m_HUDHandler.m_itemDisplayed.Count > 0)
                    {
                        if (m_HUDHandler.m_itemDisplayed.Count < m_maxItemGetted)
                        {

                            if (m_ItemFoundedND != null)
                            {
                                foreach (var item in m_ItemFoundedND)
                                {
                                    m_HUDHandler.DisplayItemFounded(item.Content.ItemPrefab);
                                }
                            }


                            m_ItemFoundedND = null;
                        }
                    }
                    else
                    {
                        foreach (var item in ItemFounded)
                        {
                            m_HUDHandler.DisplayItemFounded(item.Content.ItemPrefab);
                        }

                    }
                }


            }

            UIManager.s_Instance.SelectHUD(m_HUDHandler.gameObject);
        }
    }


}
