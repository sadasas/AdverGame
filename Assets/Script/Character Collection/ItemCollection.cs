using AdverGame.UI;
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
        [SerializeField] Image m_textBG;
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
            m_textBG.gameObject.SetActive(true);
            Name.gameObject.SetActive(true);
        }

        public void Unlock(Color32 color)
        {
            m_textBG.gameObject.SetActive(true);
            Name.gameObject.SetActive(true);
            SetColor(color);
        }

        public void Lock(Color32 color)
        {
            m_textBG.gameObject.SetActive(false);
            Name.gameObject.SetActive(false);
            SetColor(color);
        }
        void SetColor(Color32 color)
        {
            m_image.color = color;
            m_BG.color = color;

        }
        public void Onclick()
        {
            if (IsLocked) return;
            if (m_HUDItemCollectionDetail == null)
            {
                m_HUDItemCollectionDetail = Instantiate(m_HUDItemCollectionDetailPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);

            }
            UIManager.s_Instance.OverlapHUD(m_HUDItemCollectionDetail);
            m_HUDItemCollectionDetail.transform.GetChild(0).GetComponent<Image>().sprite = m_image.sprite;
            m_HUDItemCollectionDetail.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;
            m_HUDItemCollectionDetail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Name.text;
            m_HUDItemCollectionDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = m_description;
            m_HUDItemCollectionDetail.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.s_Instance.CloseHUD(m_HUDItemCollectionDetail);
            });



        }
    }
}
