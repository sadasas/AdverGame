using AdverGame.UI;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemListButton : MonoBehaviour
    {


        public ItemListHUDHandler m_itemListHUD;

    
        public void SpawnItemListHUD()
        {
            m_itemListHUD.UpdateItem();
            m_itemListHUD.gameObject.SetActive(true);
            UIManager.s_Instance.SelectHUD(m_itemListHUD.gameObject);
        }


    }
}
