using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.CharacterCollection
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] Image m_image;
        [SerializeField] Image m_BG;
      
        public TextMeshProUGUI Name;



        public void Setup(Sprite image, string name, Sprite bg)
        {
            m_BG.sprite = bg;


            m_image.sprite = image;
            Name.text = name;
        }
    }
}
