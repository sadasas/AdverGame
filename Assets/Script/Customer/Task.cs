using AdverGame.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdverGame.Customer
{
    public class Task : MonoBehaviour, IPointerClickHandler
    {
        public Order CustomerOrder { get; set; }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!PlayerManager.s_Instance.Player.InputBehaviour.IsDrag)
            {
                var camera = Camera.main;
                camera.transform.position = new Vector3(CustomerOrder.Customer.transform.position.x, camera.transform.position.y, camera.transform.position.z);
                CustomerManager.s_Instance.SeeOrder(CustomerOrder.ItemOrder, CustomerOrder.Customer);
            }
        }
    }
}

