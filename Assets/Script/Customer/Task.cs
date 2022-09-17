using AdverGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdverGame.Customer
{
    public class Task : MonoBehaviour, IPointerDownHandler
    {
        public Order CustomerOrder{get;set;}

        public void OnPointerDown(PointerEventData eventData)
        {
            var camera = Camera.main;
            camera.transform.position = new Vector3(CustomerOrder.Customer.transform.position.x, camera.transform.position.y, camera.transform.position.z);
            UIManager.s_Instance.ForceHUD(HUDName.ITEM_AVAILABLE);

        }
    }
}

