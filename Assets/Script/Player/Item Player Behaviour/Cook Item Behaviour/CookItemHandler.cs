using AdverGame.UI;
using AdverGame.Utility;
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
        int m_plates = 1;

        public int ItemBeingCook = 0;
        public int ItemCooked = 0;
        public CookItemHandler(GameObject cookItemHUDPrefab, float timeCooking, ItemContainer itemContainer, MonoBehaviour playerMono)
        {
            m_cookItemHUDPrefab = cookItemHUDPrefab;
            m_timeCooking = timeCooking;
            m_itemContainer = itemContainer;
            m_playerMono = playerMono;

            m_allItems = AssetHelpers.GetAllItemRegistered();
            InitCookItemHUD();
            m_HUDHandler.gameObject.SetActive(false);
            PlayerManager.s_Instance.OnDataLoaded += LoadStovePlayer;
            PlayerManager.s_Instance.OnIncreaseLevel += UpdatePlate;

        }

        public void LoadStovePlayer(PlayerData Data)
        {
            UpdatePlate(Data.Level.CurrentLevel);

        }

        public void UpdatePlate(Level level)
        {

            var newPlate = level.MaxStove - m_plates;
            m_plates = level.MaxStove;
            m_HUDHandler.SpawnPlate(newPlate);
        }
        public void InitCookItemHUD()
        {
            if (m_HUDHandler == null)
            {
                m_HUDHandler = GameObject.Instantiate(m_cookItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<CookItemHUDHandler>();
                m_HUDHandler.SpawnPlate(m_plates);
                m_HUDHandler.OnItemChoosed += (itemPlate, item) => m_playerMono.StartCoroutine(Cooking(itemPlate, item));

                foreach (var item in m_allItems)
                {
                    m_HUDHandler.SpawnItem(item);
                }

                UIManager.s_Instance.HUDRegistered.Add(HUDName.COOK_ITEM, m_HUDHandler.gameObject);
            }
            else
            {
                UIManager.s_Instance.SelectHUD(m_HUDHandler.gameObject);
                m_HUDHandler.gameObject.SetActive(true);
            }


        }

        void PutItemCooked(ItemSerializable item, ItemPlate itemPlate)
        {
            ItemCooked--;
            var newItem = new ItemSerializable(item.Content);

            m_itemContainer.AddItem(newItem);
            itemPlate.OnPutItem -= PutItemCooked;
            m_HUDHandler.UpdateItemCooked(-1);
            itemPlate.UpdateProggressBar(m_timeCooking, m_timeCooking);
        }

        IEnumerator Cooking(ItemPlate itemPlate, ItemSerializable item)
        {
            ItemBeingCook++;
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
            ItemBeingCook--;
            ItemCooked++;

        }
    }
}
