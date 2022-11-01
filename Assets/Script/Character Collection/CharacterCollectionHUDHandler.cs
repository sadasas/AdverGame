
using AdverGame.Customer;
using UnityEngine;

namespace AdverGame.CharacterCollection
{
    public class CharacterCollectionHUDHandler : MonoBehaviour
    {
        int m_rareItems;
        int m_CommonItems;


        [SerializeField] Transform m_commonItemPlace;
        [SerializeField] Transform m_RareItemPlace;
        [SerializeField] GameObject m_itemCollectionPrefab;
        RectTransform m_RareItemPlaceRect;
        RectTransform m_commonItemPlaceRect;


        public ItemCollection DisplayItem(CustomerVariant cust, Sprite bg)
        {

            m_RareItemPlaceRect ??= m_RareItemPlace.GetComponent<RectTransform>();
            m_commonItemPlaceRect ??= m_commonItemPlace.GetComponent<RectTransform>();
            if (cust.Type == CustomerType.RARE)
            {
                m_rareItems++;
                CheckNewRow(ref m_rareItems, ref m_RareItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_RareItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                return newItem;
            }
            else
            {
                m_CommonItems++;
                CheckNewRow(ref m_CommonItems, ref m_commonItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_commonItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                return newItem;
            }

        }

        public ItemCollection DisplayItem(CustomerVariant cust, Sprite bg, Color32 color)
        {

            m_RareItemPlaceRect ??= m_RareItemPlace.GetComponent<RectTransform>();
            m_commonItemPlaceRect ??= m_commonItemPlace.GetComponent<RectTransform>();
            if (cust.Type == CustomerType.RARE)
            {
                m_rareItems++;
                CheckNewRow(ref m_rareItems, ref m_RareItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_RareItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                newItem.SetColor(color);
                return newItem;
            }
            else
            {
                m_CommonItems++;
                CheckNewRow(ref m_CommonItems, ref m_commonItemPlaceRect);
                var newItem = Instantiate(m_itemCollectionPrefab, m_commonItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg, cust.Description);
                newItem.SetColor(color);
                return newItem;
            }

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
