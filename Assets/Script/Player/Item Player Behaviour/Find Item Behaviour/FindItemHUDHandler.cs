using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class FindItemHUDHandler : MonoBehaviour
    {

        [SerializeField] Slider m_slider;
        [SerializeField] Transform m_itemPlace;
        [field: SerializeField]
        public List<GameObject> m_itemDisplayed { get; private set; }


        public Action OnGetTriggered;

        public void DisplayItemFounded(GameObject itemfounded)
        {
            m_itemDisplayed ??= new();
            var obj = Instantiate(itemfounded, m_itemPlace.transform);
            obj.transform.localScale = obj.transform.localScale / 2;
            m_itemDisplayed.Add(obj);

        }

        public void TrackItemFinded(int itemfindedTot)
        {

            m_slider.value = 1f / (8f / itemfindedTot);

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
