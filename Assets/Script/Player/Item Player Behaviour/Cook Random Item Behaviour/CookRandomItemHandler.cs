﻿using AdverGame.UI;
using AdverGame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{
    public class CookRandomItemHandler
    {
        readonly object m_locker = new object();
        List<ItemSerializable> m_allItems;
        int m_searchItemTime = 5;
        CookRandomItemHUDHandler m_HUDHandler;
        GameObject m_cookRandomItemHUDPrefab;
        Coroutine m_cookRandomItemCoro;
        MonoBehaviour m_player;
        int m_maxItemCooked = 8;
        ItemContainer m_itemContainer;
        bool isCooking = false;
        bool isInstantCooking = false;
        Action<int, float, float> OnFindItem;
        int[] index;

        public List<ItemSerializable> ItemFounded { get; private set; }

        public CookRandomItemHandler(MonoBehaviour player, GameObject cookRandomItemHUDPrefab, int searchItemTime, int maxItemCooked, ItemContainer itemContainer)
        {
            m_cookRandomItemHUDPrefab = cookRandomItemHUDPrefab;
            m_player = player;
            m_searchItemTime = searchItemTime;
            m_maxItemCooked = maxItemCooked;

            m_allItems = AssetHelpers.GetAllItemRegistered();

            InitFindItemHUD();
            StartCookRandomItem();
            m_itemContainer = itemContainer;

        }

        IEnumerator CookFindItem()
        {

            index = new int[m_maxItemCooked];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = UnityEngine.Random.Range(0, m_allItems.Count);
            }

            var count = 0;
            ItemFounded ??= new();
            while (isCooking && ItemFounded.Count < m_maxItemCooked)
            {


                OnFindItem?.Invoke(count, m_maxItemCooked, m_searchItemTime);

                yield return new WaitForSeconds(m_searchItemTime);

                //Find item
                ItemFounded.Add(new ItemSerializable(m_allItems[index[count]].Content));


                count++;
                if (ItemFounded.Count == m_maxItemCooked) OnFindItem?.Invoke(m_maxItemCooked, m_maxItemCooked, m_searchItemTime);


            }
        }
        void StartCookRandomItem()
        {

            isCooking = true;
            m_cookRandomItemCoro = m_player.StartCoroutine(CookFindItem());
        }

        void DisplayItemCooked()
        {


            foreach (var item in ItemFounded)
            {
                m_HUDHandler.DisplayItemFounded(item);
            }
            ItemFounded = new();
        }

        void PutItemFinded()
        {
            isCooking = false;
            m_player.StopCoroutine(m_cookRandomItemCoro);
            lock (m_locker)
            {

                if (isInstantCooking)
                {
                    for (int i = ItemFounded.Count; i < m_maxItemCooked; i++)
                    {
                        //Find item
                        ItemFounded.Add(new ItemSerializable(m_allItems[index[i]].Content));


                        OnFindItem?.Invoke(ItemFounded.Count, m_maxItemCooked, 0);
                    }


                    isInstantCooking = false;

                }

                foreach (var item in ItemFounded)
                {
                    m_itemContainer.AddItem(item);

                }

            }
            DisplayItemCooked();

        }
        void ResetItemCooked()
        {

            ItemFounded = new();
            StartCookRandomItem();
        }

        void InstantCook()
        {
            isInstantCooking = true;

        }

        public void InitFindItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_cookRandomItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<CookRandomItemHUDHandler>();
                m_HUDHandler.OnGetTriggered += PutItemFinded;
                m_HUDHandler.OnResetTriggered += ResetItemCooked;
                m_HUDHandler.OnInstantSearchItemTriggered += InstantCook;
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