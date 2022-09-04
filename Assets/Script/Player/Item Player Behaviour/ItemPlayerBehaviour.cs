using UnityEngine;

namespace AdverGame.Player
{

    public class ItemPlayerBehaviour
    {


        InputBehaviour m_input;
        Transform m_player;
        ItemContainer m_itemContainer;


        FindItemHandler m_findItemHandler;
        ItemAvailableHandler m_itemAvailableHandler;
        public ItemPlayerBehaviour(InputBehaviour inputBehaviour, Transform player, GameObject findItemHUD, GameObject itemAvailableHUDPrefab, GameObject itemAvailableButtonPrefab, int searchItemTime, int maxItemGetted)
        {
            m_itemContainer = new(player.GetComponent<MonoBehaviour>());
            m_input = inputBehaviour;
            m_player = player;

            m_input.OnLeftClick += OnLeftClickCallback;
            m_findItemHandler = new(player.GetComponent<MonoBehaviour>(), findItemHUD, searchItemTime, maxItemGetted, m_itemContainer);
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
            if (obj == m_player.gameObject)
            {
                m_findItemHandler.InitFindItemHUD();
            }
        }




    }


}
