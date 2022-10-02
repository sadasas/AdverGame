using AdverGame.Player;
using AdverGame.UI;
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
                UIManager.s_Instance.ForceHUD(HUDName.ITEM_AVAILABLE);
            }
        }
    }
}

