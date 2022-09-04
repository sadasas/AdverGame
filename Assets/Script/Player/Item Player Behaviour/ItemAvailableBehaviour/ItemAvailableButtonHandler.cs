using AdverGame.UI;
using System;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemAvailableButtonHandler : MonoBehaviour
    {


        public ItemAvailableHUDHandler m_itemAvailableHUD;

        public Action OnDisplayItemAvailableHUD;
        public void DisplayItemListHUD()
        {
            OnDisplayItemAvailableHUD?.Invoke();

            m_itemAvailableHUD.gameObject.SetActive(true);
            UIManager.s_Instance.SelectHUD(m_itemAvailableHUD.gameObject);
        }


    }
}
