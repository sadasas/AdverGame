using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{


    public class AdverHUDController : MonoBehaviour
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



    }


}
