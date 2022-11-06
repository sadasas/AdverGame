using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.CharacterCollection
{
    public class ItemCollection : MonoBehaviour
    {
        string m_description = null;

        public Image m_image;
        [SerializeField] Image m_BG;
        [SerializeField] GameObject m_HUDItemCollectionDetailPrefab;
        public static GameObject m_HUDItemCollectionDetail;

        public TextMeshProUGUI Name;

        public bool IsLocked;

        public void Setup(Sprite image, string name, Sprite bg, string desk)
        {
            m_BG.sprite = bg;

            m_description = desk;
            m_image.sprite = image;
            Name.text = name;
        }

        public void SetColor(Color32 color)
        {
            m_image.color = color;
            m_BG.color = color;
        }
        public void Onclick()
        {
            if (IsLocked) return;
            m_HUDItemCollectionDetail ??= Instantiate(m_HUDItemCollectionDetailPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_HUDItemCollectionDetail.transform.GetChild(0).GetComponent<Image>().sprite = m_image.sprite;
            m_HUDItemCollectionDetail.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;
            m_HUDItemCollectionDetail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Name.text;
            m_HUDItemCollectionDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = m_description;
            m_HUDItemCollectionDetail.SetActive(true);
        }
    }
}
