using System.Collections;
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
            
            m_contentPlace.sprite = m_contents[rand];

            StartCoroutine(Spawning());

        }


        IEnumerator Spawning()
        {
            var countDown = m_countDown;
            while (countDown > 0.0f)
            {
                countDown -= Time.deltaTime;
                m_countDownText.text = ((int)countDown).ToString();
                yield return null;
            }
            Destroy(gameObject);
        }
/*
        private void OnDisable()
        {
            Time.timeScale = 1;
        }
*/
        public void OnPointerClick(PointerEventData eventData)
        {
            Application.OpenURL("https://gofood.co.id/semarang/restaurant/oishi-fried-chicken-jl-raya-muntal-gunungpati-7b93d121-1be2-44bc-bf94-fc549bfd5fa7");
        }
    }


}
