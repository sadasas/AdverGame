using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class ItemSerializable
    {
        public ItemContent Content;
        public int Stack { get; private set; } = 1;

        public ItemSerializable(ItemContent content)
        {
            Content = content;
        }

        public void IncreaseItem(int stack)
        {
            Stack += stack;
        }

    }
    public class Item : MonoBehaviour
    {
        [field: SerializeField]
        public TextMeshProUGUI Name { get; private set; }
        [SerializeField] TextMeshProUGUI m_stack;
        [SerializeField] Image m_image;


        public void UpdateItem(ItemContent content, int stack)
        {
            Name.text = content.Name;
            m_image.sprite = content.Image;
            m_stack.text = stack.ToString();

        }

    }


}
