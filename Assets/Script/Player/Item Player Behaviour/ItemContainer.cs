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

            LoadData();
        }

        public void LoadData()
        {
            m_player.StartCoroutine(LoadItemsData());
        }
        IEnumerator LoadItemsData()
        {

            yield return Items = PlayerManager.s_Instance.Data.Items;


        }


        public void AddItem(ItemSerializable newItem)
        {
            Items ??= new();
            Items.Add(newItem);
            PlayerManager.s_Instance.SaveItem(Items);
        }

        public void IncreaseItem(ItemSerializable item, int stack)
        {
            item.UpdateStack(stack);
            PlayerManager.s_Instance.SaveItem(Items);
        }
        public void DecreaseItem(ItemSerializable currentItem)
        {
            var isAvailable = true;
            var tempItem = new ItemSerializable(null);
            foreach (var item in Items)
            {
                if (item == currentItem)
                {
                    if (item.Stack > 1) item.UpdateStack(-1);
                    else
                    {
                        isAvailable = false;
                        tempItem = item;
                    }

                }
            }

            if (!isAvailable) Items.Remove(tempItem);

            PlayerManager.s_Instance.SaveItem(Items);
        }




    }


}
