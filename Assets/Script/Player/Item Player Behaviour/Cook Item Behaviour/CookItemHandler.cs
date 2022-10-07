﻿using AdverGame.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{
    public class CookItemHandler
    {

        List<ItemSerializable> m_allItems;
        GameObject m_cookItemHUDPrefab;
        CookItemHUDHandler m_HUDHandler;
        float m_timeCooking;
        ItemContainer m_itemContainer;
        MonoBehaviour m_playerMono;

        public CookItemHandler(GameObject cookItemHUDPrefab, float timeCooking, ItemContainer itemContainer, MonoBehaviour playerMono)
        {
            m_cookItemHUDPrefab = cookItemHUDPrefab;
            m_timeCooking = timeCooking;
            m_itemContainer = itemContainer;
            m_playerMono = playerMono;

            m_allItems = AssetHelpers.GetAllItemRegistered();

        }

        public void InitCookItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_cookItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<CookItemHUDHandler>();
                m_HUDHandler.platesCount = 1;
                m_HUDHandler.OnItemChoosed += (itemPlate, item) => m_playerMono.StartCoroutine(Cooking(itemPlate, item));

                foreach (var item in m_allItems)
                {
                    m_HUDHandler.SpawnItem(item);
                }
            }
            else
            {
                m_HUDHandler.gameObject.SetActive(true);
            }


        }

        void PutItemCooked(ItemSerializable item, ItemPlate itemPlate)
        {
            var newItem = new ItemSerializable(item.Content);

            m_itemContainer.AddItem(newItem);
            itemPlate.OnPutItem -= PutItemCooked;
            m_HUDHandler.UpdateItemCooked(-1);
            itemPlate.UpdateProggressBar(m_timeCooking, m_timeCooking);
        }

        IEnumerator Cooking(ItemPlate itemPlate, ItemSerializable item)
        {
            itemPlate.TimeCooking = m_timeCooking;
            itemPlate.OnPutItem += PutItemCooked;

            itemPlate.Cooking(item.Content.Image);
            var amount = m_timeCooking;
            while (amount >= 0.0f)
            {
                itemPlate.ProggressCooking = amount;
                yield return new WaitForSeconds(1);
                amount -= 1;
            }


            itemPlate.Item = item;
            m_HUDHandler.UpdateItemCooked(1);

        }
    }
}
