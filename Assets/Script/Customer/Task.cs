using AdverGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdverGame.Customer
{
    public class Task : MonoBehaviour, IPointerDownHandler
    {
        public Order CustomerOrder;
        Camera camera;
        private void Awake()
        {
            camera = Camera.main;
            Debug.Log("camera " + camera.transform.position.x);
        }
        public void OnPointerDown(PointerEventData eventData)
        {

            var camXPos = camera.transform.position.x;
            var targetXPos = CustomerOrder.Customer.transform.position.x;
            var distance = camXPos > targetXPos ? -(camXPos - targetXPos) : targetXPos - camXPos;

            camera.transform.position = new Vector3(CustomerOrder.Customer.transform.position.x, camera.transform.position.y, camera.transform.position.z);
            UIManager.s_Instance.ForceHUD(HUDName.ITEM_AVAILABLE);
          
        }
    }
}

