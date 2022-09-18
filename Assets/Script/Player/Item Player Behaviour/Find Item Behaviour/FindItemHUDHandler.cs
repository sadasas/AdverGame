using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class FindItemHUDHandler : MonoBehaviour
    {

        [SerializeField] Slider m_slider;
        [SerializeField] Transform m_itemPlace;
        [SerializeField] TextMeshProUGUI m_itemFounded;
        [SerializeField] GameObject m_adverHUDPrefab;
        [SerializeField] GameObject m_getInstantItemButton;
        [SerializeField] GameObject m_getItemButton;
        [field: SerializeField]
        public List<GameObject> m_itemDisplayed { get; private set; }

        public Action OnGetTriggered;
        public Action OnResetTriggered;
        public Action OnInstantSearchItemTriggered;


        public void DisplayItemFounded(ItemSerializable itemfounded)
        {
            m_itemDisplayed ??= new();
            var obj = Instantiate(itemfounded.Content.ItemPrefab, m_itemPlace.transform);
            obj.transform.localScale = obj.transform.localScale / 2;
            m_itemDisplayed.Add(obj);
            obj.GetComponent<Item>().UpdateItem(itemfounded.Content, 1);

        }

        public void TrackItemFinded(int itemfindedTot, float itemMaxFounded)
        {

            m_slider.value = 1f / (itemMaxFounded / itemfindedTot);
            m_itemFounded.text = itemfindedTot.ToString() + " / " + itemMaxFounded;

            if (itemfindedTot == itemMaxFounded) m_getInstantItemButton.SetActive(false);
            else m_getInstantItemButton.SetActive(true);

            if (itemfindedTot == 0) m_getItemButton.SetActive(false);
            else m_getItemButton.SetActive(true);

        }

        public void GetItemInstant()
        {
            Instantiate(m_adverHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);

            OnInstantSearchItemTriggered?.Invoke();
            OnGetTriggered?.Invoke();
            m_itemPlace.parent.gameObject.SetActive(true);

        }
        public void Exit()
        {
            this.gameObject.SetActive(false);
        }



        public void GetItemFinded()
        {

            m_itemPlace.parent.gameObject.SetActive(true);
            OnGetTriggered?.Invoke();
        }

        public void ResetItemFinded()
        {

            OnResetTriggered?.Invoke();

            if (m_itemDisplayed == null || m_itemDisplayed.Count == 0) return;
            foreach (var item in m_itemDisplayed)
            {
                Destroy(item);
            }
            m_itemDisplayed = null;
            m_slider.value = 0;
            m_itemFounded.text = "0 / 8";
            m_itemPlace.parent.gameObject.SetActive(false);
            m_getItemButton.SetActive(false);
            m_getInstantItemButton.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }


}
