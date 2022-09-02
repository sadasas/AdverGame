using AdverGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdverGame.Player
{

    public class FindItemHandler
    {

        List<Item> m_allItems;
        int m_searchItemTime = 5;
        bool m_isSearching = false;
        FindItemHUDHandler m_HUDHandler;
        GameObject m_findItemHUDPrefab;
        public List<Item> ItemFounded { get; private set; }
        public ItemListHUDHandler m_itemListHUDHandler;
        List<Item> m_ItemFoundedND;

        MonoBehaviour m_player;
        public FindItemHandler(MonoBehaviour player, List<Item> allItems, GameObject findItemHUDPrefab, ItemListHUDHandler itemListHUDHandler)
        {
            m_findItemHUDPrefab = findItemHUDPrefab;
            m_player = player;
            m_allItems = allItems;

            m_itemListHUDHandler = itemListHUDHandler;
        }
        void StartFindItem()
        {
            if (!m_isSearching) m_player.StartCoroutine(FindItem());
        }
        IEnumerator FindItem()
        {
            m_isSearching = true;
            var indexs = new List<int>();

            for (int i = 0; i < m_allItems.Count(); i++)
            {
                var index = UnityEngine.Random.Range(1, m_allItems.Count());
                indexs.Add(index);
            }

            ItemFounded ??= new();
            foreach (var index in indexs)
            {
                ItemFounded.Add(m_allItems[index]);

                Debug.Log("searching");
                if (m_HUDHandler != null)
                {
                    if (m_HUDHandler.gameObject.activeInHierarchy) m_HUDHandler.DisplayItemFounded(m_allItems[index]);
                    else
                    {
                        m_ItemFoundedND ??= new();
                        m_ItemFoundedND.Add(m_allItems[index]);
                    }
                }
                else if (m_HUDHandler == null)
                {
                    m_ItemFoundedND ??= new();
                    m_ItemFoundedND.Add(m_allItems[index]);
                }
                yield return new WaitForSeconds(m_searchItemTime);

            }
            m_isSearching = false;
        }

        public void destroyItemTemp()
        {
            if (m_ItemFoundedND != null)
            {
                foreach (var item in m_ItemFoundedND)
                {
                    GameObject.Destroy(item);
                }
                m_ItemFoundedND = null;
            }



            if (ItemFounded != null)
            {
                foreach (var item in ItemFounded)
                {
                    GameObject.Destroy(item);
                }
                ItemFounded = null;
            }

        }
        public void InitFindItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_findItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<FindItemHUDHandler>();
                m_HUDHandler.OnSearchTriggered += StartFindItem;
                m_HUDHandler.OnGetTriggered += destroyItemTemp;
            }
            m_HUDHandler.gameObject.SetActive(true);
            if (ItemFounded != null && ItemFounded.Count > 0)
            {
                if (m_HUDHandler.m_itemDisplayed != null && m_HUDHandler.m_itemDisplayed.Count > 0)
                {
                    if (ItemFounded.Count > m_HUDHandler.m_itemDisplayed.Count)
                    {

                        foreach (var item in m_ItemFoundedND)
                        {
                            m_HUDHandler.DisplayItemFounded(item);
                        }

                        destroyItemTemp();
                    }
                }
                else
                {
                    foreach (var item in ItemFounded)
                    {
                        m_HUDHandler.DisplayItemFounded(item);
                    }

                }


            }


            UIManager.s_Instance.SelectHUD(m_HUDHandler.gameObject);
        }
    }
    public class FindItemHUDHandler : MonoBehaviour
    {

        [SerializeField] Transform m_itemPlace;
        public List<Item> m_itemDisplayed { get; private set; }

        public Action OnSearchTriggered;
        public Action OnGetTriggered;

        public void DisplayItemFounded(Item itemfounded)
        {

            m_itemDisplayed ??= new();
            var obj = Instantiate(itemfounded.gameObject, m_itemPlace.transform);
            obj.transform.localScale = obj.transform.localScale / 2;
            m_itemDisplayed.Add(itemfounded);

        }

        public void SearchItem()
        {
            OnSearchTriggered?.Invoke();
        }
        public void Exit()
        {
            this.gameObject.SetActive(false);
        }

        public void PutItem()
        {
            PlayerManager.s_Instance.Data.Items ??= new();
            foreach (var item in m_itemDisplayed)
            {
                if (PlayerManager.s_Instance.Data.Items != null && PlayerManager.s_Instance.Data.Items.Count > 0)
                {
                    foreach (var itempl in PlayerManager.s_Instance.Data.Items)
                    {
                        if (itempl == item) itempl.IncreaseItem();
                        break;
                    }
                    PlayerManager.s_Instance.Data.Items.Add(item);

                }
                else PlayerManager.s_Instance.Data.Items.Add(item);

            }

            OnGetTriggered?.Invoke();
        }


    }


}
