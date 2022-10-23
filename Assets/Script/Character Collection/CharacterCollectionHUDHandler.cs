
using AdverGame.Customer;
using UnityEngine;

namespace AdverGame.CharacterCollection
{
    public class CharacterCollectionHUDHandler : MonoBehaviour
    {
        [SerializeField] Transform m_commonItemPlace;
        [SerializeField] Transform m_RareItemPlace;
        [SerializeField] GameObject m_itemCollectionPrefab;

        public ItemCollection DisplayItem(CustomerVariant cust,Sprite bg)
        {
            if(cust.Type == CustomerType.RARE)
            {
                var newItem = Instantiate(m_itemCollectionPrefab, m_RareItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name,bg );
                return newItem;
            }
            else
            {
                var newItem = Instantiate(m_itemCollectionPrefab, m_commonItemPlace).GetComponent<ItemCollection>();
                newItem.Setup(cust.Image, cust.Name, bg);
                return newItem;
            }
        }
    }
}
