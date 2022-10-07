using UnityEngine;

namespace AdverGame.Player
{

    public class ItemPlayerBehaviour
    {


        InputBehaviour m_input;
        GameObject m_cookTandomItemClickable;
        GameObject m_cookItemClickable;
        ItemContainer m_itemContainer;

        CookRandomItemHandler m_cookRandomItemHandler;
        ItemAvailableHandler m_itemAvailableHandler;
        CookItemHandler m_cookItemHandler;
        public ItemPlayerBehaviour(InputBehaviour inputBehaviour, GameObject cookRandomItemClickable, GameObject findItemHUD, GameObject itemAvailableHUDPrefab, GameObject itemAvailableButtonPrefab, int searchItemTime, int maxItemGetted, MonoBehaviour player, GameObject cookItemHUDPrefab, GameObject cookItemClickable, float timeCooking)
        {
            m_itemContainer = new(player);
            m_input = inputBehaviour;
            m_cookTandomItemClickable = cookRandomItemClickable;
            m_cookItemClickable = cookItemClickable;

            m_input.OnLeftClick += OnLeftClickCallback;
            m_cookRandomItemHandler = new(player, findItemHUD, searchItemTime, maxItemGetted, m_itemContainer);
            m_cookItemHandler = new(cookItemHUDPrefab, timeCooking, m_itemContainer, player);
            m_itemAvailableHandler = new(itemAvailableHUDPrefab, itemAvailableButtonPrefab, m_itemContainer);


        }

        public void OnDisable()
        {
            m_input.OnLeftClick -= OnLeftClickCallback;
        }

        public void OnDestroy()
        {

        }
        void OnLeftClickCallback(GameObject obj)
        {
            if (obj == m_cookTandomItemClickable)
            {
                m_cookRandomItemHandler.InitFindItemHUD();
            }
            else if (obj == m_cookItemClickable)
            {
                m_cookItemHandler.InitCookItemHUD();
            }
        }




    }


}
