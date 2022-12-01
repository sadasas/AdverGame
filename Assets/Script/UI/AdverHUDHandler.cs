using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdverGame.Player
{


    public class AdverHUDHandler : MonoBehaviour, IPointerClickHandler
    {
        Image m_contentPlace;

        [SerializeField] float m_countDown;
        [SerializeField] TextMeshProUGUI m_countDownText;
        [SerializeField] Sprite[] m_contents;

        private void OnEnable()
        {
            m_contentPlace = GetComponent<Image>();
            var rand = Random.Range(0, m_contents.Length);
            Time.timeScale = 0;
            m_contentPlace.sprite = m_contents[rand];

        }
        private void Update()
        {
            m_countDown -= Time.fixedDeltaTime * 1F;
            m_countDownText.text = ((int)m_countDown).ToString();

            if (m_countDown <= 0) Destroy(gameObject);

        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Application.OpenURL("https://gofood.co.id/semarang/restaurant/oishi-fried-chicken-jl-raya-muntal-gunungpati-7b93d121-1be2-44bc-bf94-fc549bfd5fa7");
        }
    }


}
