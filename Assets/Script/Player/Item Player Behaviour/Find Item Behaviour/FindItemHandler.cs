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
        readonly object m_locker = new object();
        List<ItemSerializable> m_allItems;
        int m_searchItemTime = 5;
        FindItemHUDHandler m_HUDHandler;
        GameObject m_findItemHUDPrefab;
        Coroutine m_findItemCoro;
        MonoBehaviour m_player;
        int m_maxItemGetted = 8;
        ItemContainer m_itemContainer;
        bool isSearching = false;
        bool isInstantSearch = false;
        Action<int, float> OnFindItem;
        int[] index;

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

        IEnumerator FindItem()
        {

            index = new int[m_maxItemGetted];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = UnityEngine.Random.Range(0, m_allItems.Count);
            }

            var count = 0;
            ItemFounded ??= new();
            while (isSearching && ItemFounded.Count < m_maxItemGetted)
            {

                //Find item

                ItemFounded.Add(new ItemSerializable(m_allItems[index[count]].Content));

               
                OnFindItem?.Invoke(ItemFounded.Count, m_maxItemGetted);

                count++;
                yield return new WaitForSeconds(m_searchItemTime);


            }
        }
        void StartFindItem()
        {

            isSearching = true;
            m_findItemCoro = m_player.StartCoroutine(FindItem());
        }

        void DisplayItemFinded()
        {


            foreach (var item in ItemFounded)
            {
                m_HUDHandler.DisplayItemFounded(item);
            }
            ItemFounded = new();
        }

        void PutItemFinded()
        {
            isSearching = false;
            m_player.StopCoroutine(m_findItemCoro);
            lock (m_locker)
            {

                if (isInstantSearch)
                {
                    for (int i = ItemFounded.Count; i < m_maxItemGetted; i++)
                    {
                        //Find item
                        ItemFounded.Add(new ItemSerializable(m_allItems[index[i]].Content));


                        OnFindItem?.Invoke(ItemFounded.Count, m_maxItemGetted);
                    }


                    isInstantSearch = false;

                }

                foreach (var item in ItemFounded)
                {
                    if (m_itemContainer.Items != null && m_itemContainer.Items.Count > 0)
                    {
                        var isSameItem = false;
                        foreach (var itempl in m_itemContainer.Items)
                        {
                            if (itempl.Content.Name.Equals(item.Content.Name))
                            {
                                m_itemContainer.IncreaseItem(itempl, item.Stack);
                                isSameItem = true;
                                break;
                            }
                        }
                        if (!isSameItem) m_itemContainer.AddItem(item);
                    }
                    else m_itemContainer.AddItem(newItem: item);

                }

            }
            DisplayItemFinded();

        }
        void ResetItemFounded()
        {
            ItemFounded = new();
            StartFindItem();
        }

        void InstantSearch()
        {
            isInstantSearch = true;

        }

        public void InitFindItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_findItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<FindItemHUDHandler>();
                m_HUDHandler.OnGetTriggered += PutItemFinded;
                m_HUDHandler.OnResetTriggered += ResetItemFounded;
                m_HUDHandler.OnInstantSearchItemTriggered += InstantSearch;
                OnFindItem += m_HUDHandler.TrackItemFinded;

                m_HUDHandler.gameObject.SetActive(false);
                UIManager.s_Instance.HUDRegistered.Add(HUDName.FIND_ITEM, m_HUDHandler.gameObject);

            }
            else
            {
                m_HUDHandler.gameObject.SetActive(true);

                UIManager.s_Instance.SelectHUD(m_HUDHandler.gameObject);

            }

        }
    }


}
