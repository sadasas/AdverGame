

using UnityEngine;
using UnityEngine.UI;

namespace AdverGame
{
    public class SettingMenu : MonoBehaviour
    {
        bool m_isMute = false;

        [SerializeField] GameObject m_buttonBGM;
        [SerializeField] Sprite m_bgmOff;
        [SerializeField] Sprite m_bgmOn;
        public void SetupBGM()
        {
            if (m_isMute)
            {
                m_buttonBGM.GetComponent<Image>().sprite = m_bgmOn;
            }
            else m_buttonBGM.GetComponent<Image>().sprite = m_bgmOff;

            m_isMute = !m_isMute;
        }
    }

}
