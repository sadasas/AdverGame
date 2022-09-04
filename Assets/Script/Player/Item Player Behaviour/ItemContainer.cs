using AdverGame.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Player
{


    public class ItemContainer
    {
        public List<ItemSerializable> Items { get; private set; }
        MonoBehaviour m_player;
        public ItemContainer(MonoBehaviour player)
        {
            m_player = player;


        }

        public void LoadData()
        {
            m_player.StartCoroutine(LoadItemsData());
        }
        IEnumerator LoadItemsData()
        {
            var data = PlayerManager.s_Instance.Data.Items;
            if (data != null && data.Count > 0) yield return Items = LoadItemsFromPlayerData();
            else yield return Items = LoadDefaultItemsSet();
            SaveItems();
        }
        List<ItemSerializable> LoadItemsFromPlayerData()
        {
            return PlayerManager.s_Instance.Data.Items;
        }

        List<ItemSerializable> LoadDefaultItemsSet()
        {


            return AssetHelpers.GetAllItemRegistered();
        }


        public void AddItem(ItemSerializable newItem)
        {
            Items ??= new();
            Items.Add(newItem);
            SaveItems();
        }


        void SaveItems()
        {
            PlayerManager.s_Instance.Data.Items = Items;
        }


    }


}
