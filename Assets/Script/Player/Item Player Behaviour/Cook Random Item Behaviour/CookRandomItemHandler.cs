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
        int m_maxItemCooked = 8;
        ItemContainer m_itemContainer;
        bool isCooking = false;
        bool isInstantCooking = false;
        Action<int, float, float> OnFindItem;
        int[] index;
        SpriteRenderer m_etalase;
        Sprite m_etalaseFull, m_etalaseHalf, m_etalaseEmpty;
        public List<ItemSerializable> ItemFounded { get; private set; }

        public CookRandomItemHandler(MonoBehaviour player, GameObject cookRandomItemHUDPrefab, int searchItemTime, int maxItemCooked, ItemContainer itemContainer, SpriteRenderer etalase, Sprite etalaseFull, Sprite etalaseHalf, Sprite etalaseEmpty)
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

            InitFindItemHUD();
            StartCookRandomItem();
            m_itemContainer = itemContainer;

        }

        IEnumerator CookRandomItem()
        {

            index = new int[m_maxItemCooked];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = UnityEngine.Random.Range(0, m_allItems.Count);
            }

            var count = 0;
            ItemFounded ??= new();
            while (isCooking && count < m_maxItemCooked)
            {


                OnFindItem?.Invoke(count, m_maxItemCooked, m_searchItemTime);

                yield return new WaitForSeconds(m_searchItemTime);

                var newItem = new ItemSerializable(m_allItems[index[count]].Content);
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
                if (count == m_maxItemCooked) OnFindItem?.Invoke(m_maxItemCooked, m_maxItemCooked, m_searchItemTime);

                SetupEtalase(count);
            }
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
            m_player.StopCoroutine(m_cookRandomItemCoro);
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


    }


}
