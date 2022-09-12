using AdverGame.UI;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemAvailableButtonHandler : MonoBehaviour
    {


        public ItemAvailableHUDHandler m_itemAvailableHUD;


        public void DisplayItemListHUD()
        {


            m_itemAvailableHUD.gameObject.SetActive(true);
            UIManager.s_Instance.SelectHUD(m_itemAvailableHUD.gameObject);
        }


    }
}
