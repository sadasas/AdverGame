using AdverGame.Customer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class UncompletedTask : MonoBehaviour, IPointerClickHandler
    {
       

        public GameObject m_HUDOrderDetail;
        public Order CustomerOrder { get; set; }

        public Action<UncompletedTask> OnShowDetail;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!PlayerManager.s_Instance.Player.InputBehaviour.IsDrag)
            {
                OnShowDetail?.Invoke(this);
                m_HUDOrderDetail.SetActive(true);
            }
        }

        private void Start()
        {
            m_HUDOrderDetail.transform.GetChild(0).GetComponent<Image>().sprite = CustomerOrder.ItemOrder.Content.Image;
            m_HUDOrderDetail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = CustomerOrder.ItemOrder.Content.Name;
            m_HUDOrderDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = CustomerOrder.Customer.Variant.Name;
        }
    }
}
