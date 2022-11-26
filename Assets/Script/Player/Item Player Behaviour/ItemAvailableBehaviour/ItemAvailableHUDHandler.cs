using AdverGame.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{

    /// <summary>
    /// TODO: Refactor update item method
    /// </summary>
    public class ItemAvailableHUDHandler : MonoBehaviour
    {
        int m_count = 0;
        int m_row = 1;
        int m_currentIndexItemSelected;
        Item m_selectedItem;
        public bool m_isMustReposition;
        [SerializeField] Transform m_itemPlace;
        [SerializeField] Scrollbar m_scrollbar;
        public Dictionary<Item, ItemSerializable> ItemsDisplayed { get; private set; }

        public Action<ItemSerializable> OnItemTouched;
        public Action OnActive;


        private void OnEnable()
        {
            OnActive?.Invoke();

           if(m_isMustReposition) RepositionScrollbarValue();

        }

        void RepositionScrollbarValue()
        {
            m_isMustReposition = false;
            var valueIndex = 1f / m_row + 0.04f;
            var aa = ((float)m_currentIndexItemSelected / 3) - Math.Round((float)m_currentIndexItemSelected / 3, MidpointRounding.AwayFromZero) == 0 ? Math.Round((float)m_currentIndexItemSelected / 3, MidpointRounding.AwayFromZero) :
                ((float)m_currentIndexItemSelected / 3) - Math.Round((float)m_currentIndexItemSelected / 3, MidpointRounding.AwayFromZero) < 0f ? Math.Round((float)m_currentIndexItemSelected / 3, MidpointRounding.AwayFromZero) : Math.Round((float)m_currentIndexItemSelected / 3, MidpointRounding.AwayFromZero) + 1;
            var itemRow = m_row - aa;

            Debug.Log(aa);
            Debug.Log(Math.Round((float)m_currentIndexItemSelected / 3, MidpointRounding.AwayFromZero));
            Debug.Log(m_row + " - " + m_currentIndexItemSelected + " / " + 3 + " = " + itemRow);
            var value = itemRow * valueIndex;

            Debug.Log(value);

            m_scrollbar.value = (float)value;
        }
        public void SelectItem(ItemSerializable item)
        {
            if (m_selectedItem != null) m_selectedItem.UnSelected();

            m_currentIndexItemSelected = 0;
            foreach (var itemDisplayed in ItemsDisplayed)
            {
                m_currentIndexItemSelected++;
                if (itemDisplayed.Value.Content.Name.Equals(item.Content.Name))
                {
                    m_selectedItem = itemDisplayed.Key;
                    itemDisplayed.Key.Selected();
                    break;
                }
            }

            RepositionScrollbarValue();

        }
        void ItemTouched(Item item)
        {

            OnItemTouched?.Invoke(ItemsDisplayed[item]);
        }

        public void DisplayItem(ItemSerializable item)
        {
            m_count++;
            if (m_count > 3)
            {
                m_row++;
                m_count = 1;
                var tempsize = new Vector2(m_itemPlace.GetComponent<RectTransform>().sizeDelta.x, m_itemPlace.GetComponent<RectTransform>().sizeDelta.y + item.Content.ItemPrefab.GetComponent<RectTransform>().sizeDelta.y);
                m_itemPlace.GetComponent<RectTransform>().sizeDelta = tempsize;
            }
            ItemsDisplayed ??= new();
            var obj = Instantiate(item.Content.ItemPrefab, m_itemPlace).GetComponent<Item>();
            obj.UpdateItem(item.Content, item.Stack);

            ItemsDisplayed.Add(obj, item);
            obj.OnTouch += ItemTouched;

        }
        public void DestroyItem(Item item)
        {

            Destroy(item.gameObject);
        }
        public void RemoveItem(Item item)
        {
            ItemsDisplayed.Remove(item);

        }
        public void RemoveItem()
        {
            ItemsDisplayed.Clear();

        }


        public void Close()
        {
            UIManager.s_Instance.CloseHUD(gameObject);

        }
    }
}
