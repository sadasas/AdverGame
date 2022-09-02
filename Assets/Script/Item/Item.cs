using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class Item : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_name;
        [SerializeField] TextMeshProUGUI m_stack;
        [SerializeField] Image m_image;

        [field: SerializeField]
        public ItemContent m_content { get; private set; }
        public int Stack { get; private set; } = 1;

        private void Start()
        {
            UpdateItem();
        }
        public void UpdateItem()
        {
            m_name.text = m_content.Name;
            m_image.sprite = m_content.Image;

        }

        public void IncreaseItem()
        {
            Stack++;
            m_stack.text = Stack.ToString();
        }

    }


}
