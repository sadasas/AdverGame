using AdverGame.UI;
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
        int m_maxItemCooked = 24;
        ItemContainer m_itemContainer;
        bool isCooking = false;
        bool isInstantCooking = false;
        Action<int, float, float> OnFindItem;
        int[] index;
        SpriteRenderer m_etalase;
        Sprite m_etalaseFull, m_etalaseHalf, m_etalaseEmpty;
        float m_cooldownBtnFaster;
        float m_currentCooldownBtnFaster;

        public List<ItemSerializable> ItemFounded { get; private set; }

        public CookRandomItemHandler(MonoBehaviour player, GameObject cookRandomItemHUDPrefab, int searchItemTime, int maxItemCooked, ItemContainer itemContainer, SpriteRenderer etalase, Sprite etalaseFull, Sprite etalaseHalf, Sprite etalaseEmpty, float cooldownBtnFaster)
        {
            m_cookRandomItemHUDPrefab = cookRandomItemHUDPrefab;
            m_player = player;
            m_searchItemTime = searchItemTime;
            m_maxItemCooked = maxItemCooked;
            m_etalase = etalase;
            m_allItems = AssetHelpers.GetAllItemRegistered();
            m_etalaseFull = etalaseFull;
            m_etalaseHalf = etalaseHalf;
            m_etalaseEmpty = etalaseEmpty;
            m_cooldownBtnFaster = cooldownBtnFaster;
            m_itemContainer = itemContainer;

            InitFindItemHUD();
            LoadDataPlayer();
            StartCookRandomItem();

        }

        void LoadDataPlayer()
        {
            var data = PlayerManager.s_Instance.GetDataRandomItem();
            ItemFounded = data.RandomItemGetted;
            m_currentCooldownBtnFaster = data.CooldownBtnFaster;

            if (m_currentCooldownBtnFaster > 0.0f) m_player.StartCoroutine(CooldownInstantCooking(m_currentCooldownBtnFaster));

        }
        IEnumerator CookRandomItem()
        {
            ItemFounded ??= new();
            var loadedData = 0;
            if (ItemFounded.Count > 0)
            {
                foreach (var item in ItemFounded)
                {
                    loadedData += item.Stack;
                }
            }
            if (loadedData >= m_maxItemCooked)
            {

                OnFindItem?.Invoke(m_maxItemCooked, m_maxItemCooked, m_searchItemTime);

                SetupEtalase(loadedData);
            }
            index = new int[m_maxItemCooked];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = UnityEngine.Random.Range(0, m_allItems.Count);
            }
            var count = 0;



            while (isCooking && count + loadedData < m_maxItemCooked)
            {


                OnFindItem?.Invoke(count + loadedData, m_maxItemCooked, m_searchItemTime);

                yield return new WaitForSeconds(m_searchItemTime);

                var newItem = new ItemSerializable(m_allItems[index[count + loadedData]].Content);
                //Find item
                bool isSame = false;
                if (ItemFounded.Count > 0)
                {
                    foreach (var item in ItemFounded)
                    {
                        if (item.Content.Name.Equals(newItem.Content.Name))
                        {
                            item.IncreaseStack(newItem.Stack);
                            isSame = true;
                            break;
                        }
                    }
                }
                if (!isSame) ItemFounded.Add(newItem);

                count++;
                if (count + loadedData == m_maxItemCooked) OnFindItem?.Invoke(m_maxItemCooked, m_maxItemCooked, m_searchItemTime);

                SetupEtalase(count);
                SaveData();
            }
        }
        void SaveData()
        {
            var data = new DataRandomItem();
            data.CooldownBtnFaster = m_currentCooldownBtnFaster;
            data.RandomItemGetted = ItemFounded;

            PlayerManager.s_Instance.SaveDataRandomItem(data);
        }
        void SetupEtalase(int itemFounded)
        {

            if (itemFounded == 0) m_etalase.sprite = m_etalaseEmpty;
            else if (itemFounded < m_maxItemCooked) m_etalase.sprite = m_etalaseHalf;
            else m_etalase.sprite = m_etalaseFull;
        }
        void StartCookRandomItem()
        {

            isCooking = true;
            m_cookRandomItemCoro = m_player.StartCoroutine(CookRandomItem());
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
            if (m_cookRandomItemCoro != null) m_player.StopCoroutine(m_cookRandomItemCoro);
            lock (m_locker)
            {

                if (isInstantCooking)
                {
                    index = new int[m_maxItemCooked - ItemFounded.Count];
                    for (int i = 0; i < index.Length; i++)
                    {
                        index[i] = UnityEngine.Random.Range(0, m_allItems.Count);
                    }

                    for (int i = 0; i < index.Length; i++)
                    {
                        //Find item

                        var newItem = new ItemSerializable(m_allItems[index[i]].Content);
                        //Find item
                        bool isSame = false;
                        if (ItemFounded.Count > 0)
                        {
                            foreach (var item in ItemFounded)
                            {
                                if (item.Content.Name.Equals(newItem.Content.Name))
                                {
                                    item.IncreaseStack(newItem.Stack);
                                    isSame = true;
                                    break;
                                }
                            }
                        }
                        if (!isSame) ItemFounded.Add(newItem);
                        OnFindItem?.Invoke(ItemFounded.Count, m_maxItemCooked, 0);
                    }


                    isInstantCooking = false;

                    m_player.StartCoroutine(CooldownInstantCooking(m_cooldownBtnFaster));

                }

                foreach (var item in ItemFounded)
                {
                    m_itemContainer.AddItem(item);

                }

            }
            DisplayItemCooked();

        }

        IEnumerator CooldownInstantCooking(float cooldownBtnFaster)
        {
            m_currentCooldownBtnFaster = cooldownBtnFaster;

            while (m_currentCooldownBtnFaster > 0.0f)
            {
                m_HUDHandler.DisableBtnFaster(m_currentCooldownBtnFaster);
                m_currentCooldownBtnFaster -= Time.deltaTime;
                SaveData();
                yield return null;

            }

            m_HUDHandler.EnableBtnFaster();
        }
        void ResetItemCooked()
        {

            ItemFounded = new();
            SetupEtalase(0);
            StartCookRandomItem();
        }

        void InstantCook()
        {
            isInstantCooking = true;
            PutItemFinded();
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
                m_HUDHandler.TrackItemFinded(0, m_maxItemCooked, m_searchItemTime);
                m_HUDHandler.gameObject.SetActive(false);
                UIManager.s_Instance.HUDRegistered.Add(HUDName.FIND_ITEM, m_HUDHandler.gameObject);

            }
            else
            {


                UIManager.s_Instance.SelectHUD(m_HUDHandler.gameObject);

            }

        }

        public void OnExit()
        {
            SaveData();
        }


    }


}
