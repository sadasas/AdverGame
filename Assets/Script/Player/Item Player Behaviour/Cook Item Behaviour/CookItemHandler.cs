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
        float m_timeCooking;
        ItemContainer m_itemContainer;
        MonoBehaviour m_playerMono;
        int m_plates = 1;

        public CookItemHUDHandler HUDHandler;

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
            HUDHandler.gameObject.SetActive(false);
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
            HUDHandler.SpawnPlate(newPlate);
        }
        public void InitCookItemHUD()
        {
            if (HUDHandler == null)
            {
                HUDHandler = GameObject.Instantiate(m_cookItemHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform).GetComponent<CookItemHUDHandler>();
                HUDHandler.SpawnPlate(m_plates);
                HUDHandler.OnItemChoosed += (itemPlate, item) => m_playerMono.StartCoroutine(Cooking(itemPlate, item));

                //ResolutionHelper.ScaleToFitScreen(HUDHandler.gameObject);
                foreach (var item in m_allItems)
                {
                    HUDHandler.SpawnItem(item);
                }

                UIManager.s_Instance.HUDRegistered.Add(HUDName.COOK_ITEM, HUDHandler.gameObject);
            }
            else
            {

                UIManager.s_Instance.SelectHUD(HUDHandler.gameObject);

            }
            HUDHandler.SetupChef(ItemBeingCook);


        }

        void PutItemCooked(ItemSerializable item, ItemPlate itemPlate)
        {
            ItemCooked--;
            var newItem = new ItemSerializable(item.Content);

            m_itemContainer.AddItem(newItem);
            itemPlate.OnPutItem -= PutItemCooked;
            HUDHandler.UpdateItemCooked(-1);

        }

        IEnumerator Cooking(ItemPlate itemPlate, ItemSerializable item)
        {
            itemPlate.StartState(item.Content.Image);
            itemPlate.TimeCooking = m_timeCooking;
            ItemBeingCook++;
            HUDHandler.SetupChef(ItemBeingCook);

            itemPlate.OnPutItem += PutItemCooked;
            var amount = m_timeCooking;
            while (amount >= 0.0f)
            {
                itemPlate.UpdateState(amount);

                yield return new WaitForSeconds(1);
                amount -= 1;
            }


            itemPlate.Item = item;
            HUDHandler.UpdateItemCooked(1);
            ItemBeingCook--;
            ItemCooked++;
            HUDHandler.SetupChef(ItemBeingCook);
            itemPlate.FinishState(item.Content.Image);
        }


    }
}
