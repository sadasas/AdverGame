
using AdverGame.Customer;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.CharacterCollection
{
    public class CharacterCollectionHUDHandler : MonoBehaviour
    {
        int m_rareItems;
        int m_CommonItems;
        RectTransform m_RareItemPlaceRect;
        RectTransform m_commonItemPlaceRect;
        List<ItemCollection> itemsDisplayed;

        [SerializeField] Transform m_commonItemPlace;
        [SerializeField] Transform m_RareItemPlace;
        [SerializeField] GameObject m_itemCollectionPrefab;


        public ItemCollection DisplayItem(CustomerVariant cust, Sprite bg)
        {
            itemsDisplayed ??= new();
            m_RareItemPlaceRect ??= m_RareItemPlace.GetComponent<RectTransform>();
            m_commonItemPlaceRect ??= m_commonItemPlace.GetComponent<RectTransform>();
            if (cust.Type == CustomerType.RARE)
            {
                m_rareItems++;
                CheckNewRow(ref m_rareItems, ref m_RareItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_RareItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                itemsDisplayed.Add(newItem);
                return newItem;
            }
            else
            {
                m_CommonItems++;
                CheckNewRow(ref m_CommonItems, ref m_commonItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_commonItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                itemsDisplayed.Add(newItem);
                return newItem;
            }

        }

        public ItemCollection DisplayItem(CustomerVariant cust, Sprite bg, Color32 color)
        {
            itemsDisplayed ??= new();
            m_RareItemPlaceRect ??= m_RareItemPlace.GetComponent<RectTransform>();
            m_commonItemPlaceRect ??= m_commonItemPlace.GetComponent<RectTransform>();
            if (cust.Type == CustomerType.RARE)
            {
                m_rareItems++;
                CheckNewRow(ref m_rareItems, ref m_RareItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_RareItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                newItem.SetColor(color);
                itemsDisplayed.Add(newItem);
                return newItem;
            }
            else
            {
                m_CommonItems++;
                CheckNewRow(ref m_CommonItems, ref m_commonItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_commonItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                newItem.SetColor(color);
                itemsDisplayed.Add(newItem);
                return newItem;
            }

        }

        public ItemCollection UnlockItem(CustomerVariant cust)
        {
            foreach (var itemDisplayed in itemsDisplayed)
            {
                if (itemDisplayed.Name.text.Equals(cust.Name))
                {
                    var whiteColor = new Color32(255, 255, 255, 255);
                    itemDisplayed.SetColor(whiteColor);
                    return itemDisplayed;

                }
            }
            return null;
        }
        void CheckNewRow(ref int item, ref RectTransform place)
        {
            if (item > 3)
            {
                item = 1;
                var rectItem = m_itemCollectionPrefab.GetComponent<RectTransform>();
                var currentRect = place.sizeDelta;
                var newRect = new Vector2(currentRect.x, currentRect.y + rectItem.sizeDelta.y);
                place.sizeDelta = newRect;
            }
        }
    }
}
