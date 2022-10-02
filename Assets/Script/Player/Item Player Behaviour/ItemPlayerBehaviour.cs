using UnityEngine;

namespace AdverGame.Player
{

    public class ItemPlayerBehaviour
    {


        InputBehaviour m_input;
        GameObject m_randomItemClickable;
        ItemContainer m_itemContainer;


        FindItemHandler m_findItemHandler;
        ItemAvailableHandler m_itemAvailableHandler;
        public ItemPlayerBehaviour(InputBehaviour inputBehaviour, GameObject randomItemClickable, GameObject findItemHUD, GameObject itemAvailableHUDPrefab, GameObject itemAvailableButtonPrefab, int searchItemTime, int maxItemGetted, MonoBehaviour player)
        {
            m_itemContainer = new(player);
            m_input = inputBehaviour;
            m_randomItemClickable = randomItemClickable;

            m_input.OnLeftClick += OnLeftClickCallback;
            m_findItemHandler = new(player, findItemHUD, searchItemTime, maxItemGetted, m_itemContainer);
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
            if (obj == m_randomItemClickable)
            {
                m_findItemHandler.InitFindItemHUD();
            }
        }




    }


}
