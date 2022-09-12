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

        MonoBehaviour m_player;
        int m_maxItemGetted = 8;
        ItemContainer m_itemContainer;
        bool isSearching = false;
        Action<int, float> OnFindItem;
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
            isSearching = true;
            m_player.StartCoroutine(FindItem());
        }
        IEnumerator FindItem()
        {


            ItemFounded ??= new();
            while (isSearching && ItemFounded.Count < m_maxItemGetted)
            {

                //Find item
                var index = UnityEngine.Random.Range(0, m_allItems.Count);

                ItemFounded.Add(new ItemSerializable(m_allItems[index].Content));


                OnFindItem?.Invoke(ItemFounded.Count, m_maxItemGetted);


                yield return new WaitForSeconds(m_searchItemTime);


            }



        }

        void DisplayItemFinded()
        {
            isSearching = false;
            foreach (var item in ItemFounded)
            {
                m_HUDHandler.DisplayItemFounded(item);
            }
            ItemFounded = new();
        }

        void PutItemFinded()
        {
            lock (m_locker)
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
                                m_itemContainer.IncreaseItem(itempl, item.Stack);
                                isSameItem = true;
                                break;
                            }
                        }
                        if (!isSameItem) m_itemContainer.AddItem(item);
                    }
                    else m_itemContainer.AddItem(newItem: item);

                }
                DisplayItemFinded();


            }

        }
        void ResetItemFounded()
        {
            ItemFounded = new();
            StartFindItem();
        }

        public void InitFindItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_findItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<FindItemHUDHandler>();
                m_HUDHandler.OnGetTriggered += PutItemFinded;
                m_HUDHandler.OnResetTriggered += ResetItemFounded;
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
