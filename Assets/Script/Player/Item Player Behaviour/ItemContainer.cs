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
            bool isSame = false;
            if (Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    if (item.Content.Name.Equals(newItem.Content.Name))
                    {
                        IncreaseItem(item, newItem.Stack);
                        isSame = true;
                        break;
                    }
                }
            }
            if (!isSame) Items.Add(newItem);

            PlayerManager.s_Instance.SaveItem(Items);
        }

        public void IncreaseItem(ItemSerializable item, int stack)
        {
            item.IncreaseStack(stack);
        }
        public void DecreaseItem(ItemSerializable currentItem)
        {
            var isAvailable = true;
            var tempItem = new ItemSerializable(null);
            foreach (var item in Items)
            {
                if (item.Content.Name.Equals(currentItem.Content.Name))
                {
                    if (item.Stack > 0) item.IncreaseStack(-1);
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
