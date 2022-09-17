using TMPro;
using UnityEngine;
namespace AdverGame.Player
{


    public class AdverHUDController : MonoBehaviour
    {
        [SerializeField] float m_countDown;
        [SerializeField] TextMeshProUGUI m_countDownText;

        private void Update()
        {
            m_countDown -= Time.deltaTime;
            m_countDownText.text = ((int)m_countDown).ToString();

            if (m_countDown <= 0) Destroy(gameObject);

        }

       
    }


}
