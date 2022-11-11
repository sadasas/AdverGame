

using AdverGame.Sound;
using AdverGame.UI;
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

        public void Open()
        {

            transform.SetAsLastSibling();
            if (!UIManager.s_Instance.HUDRegistered.ContainsKey(HUDName.SETTING)) UIManager.s_Instance.HUDRegistered.Add(HUDName.SETTING, this.gameObject);
            UIManager.s_Instance.SelectHUD(gameObject);
        }
        private void Start()
        {
            m_isMute = PlayerPrefs.GetInt("BGM") == 1 ? true : false;

            SetupBGM(m_isMute);
        }
        public void SetupBGM()
        {
            if (m_isMute)
            {
                PlayerPrefs.SetInt("BGM", 0);
                m_buttonBGM.GetComponent<Image>().sprite = m_bgmOn;
                SoundManager.s_Instance.PlayBGM();
            }
            else
            {
                PlayerPrefs.SetInt("BGM", 1);
                m_buttonBGM.GetComponent<Image>().sprite = m_bgmOff;
                SoundManager.s_Instance.StopBGM();
            }

            m_isMute = !m_isMute;
        }
        public void SetupBGM(bool ismute)
        {
            if (!m_isMute)
            {
                PlayerPrefs.SetInt("BGM", 0);
                m_buttonBGM.GetComponent<Image>().sprite = m_bgmOn;
                SoundManager.s_Instance.PlayBGM();
            }
            else
            {
                PlayerPrefs.SetInt("BGM", 1);
                m_buttonBGM.GetComponent<Image>().sprite = m_bgmOff;
                SoundManager.s_Instance.StopBGM();
            }


        }

        public void Close()
        {


            UIManager.s_Instance.CloseHUD(gameObject);
        }

    }

}
