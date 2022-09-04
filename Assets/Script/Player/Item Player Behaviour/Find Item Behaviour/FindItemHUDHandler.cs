using System;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class FindItemHUDHandler : MonoBehaviour
    {

        [SerializeField] Slider m_slider;
        [SerializeField] Transform m_itemPlace;
        [SerializeField] TextMeshProUGUI m_itemFounded;
        [field: SerializeField]
        public List<GameObject> m_itemDisplayed { get; private set; }


        public Action OnGetTriggered;

        public void DisplayItemFounded(ItemSerializable itemfounded)
        {
            m_itemDisplayed ??= new();
            var obj = Instantiate(itemfounded.Content.ItemPrefab, m_itemPlace.transform);
            obj.transform.localScale = obj.transform.localScale / 2;
            m_itemDisplayed.Add(obj);
            obj.GetComponent<Item>().UpdateItem(itemfounded.Content, 1);

        }

        public void TrackItemFinded(int itemfindedTot , float itemMaxFounded)
        {

            m_slider.value = 1f / (itemMaxFounded / itemfindedTot);
            m_itemFounded.text = itemfindedTot.ToString() + " / " + itemMaxFounded;

        }

        public void Exit()
        {
            this.gameObject.SetActive(false);
        }



        public void CheckItemFinded()
        {
            m_itemPlace.parent.gameObject.SetActive(true);
        }

        public void ResetItemFinded()
        {
            OnGetTriggered?.Invoke();

            foreach (var item in m_itemDisplayed)
            {
                Destroy(item);
            }
            m_itemDisplayed = null;
            m_itemPlace.parent.gameObject.SetActive(false);
        }
    }


}
