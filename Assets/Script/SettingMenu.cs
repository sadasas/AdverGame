

using AdverGame.Sound;
using AdverGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame
{
    public class SettingMenu : MonoBehaviour
    {


        [SerializeField] GameObject m_buttonBGM;
        [SerializeField] GameObject m_buttonSFX;
        [SerializeField] Sprite m_soundOff;
        [SerializeField] Sprite m_soundOn;

        public void Open()
        {

            transform.SetAsLastSibling();
            if (!UIManager.s_Instance.HUDRegistered.ContainsKey(HUDName.SETTING)) UIManager.s_Instance.HUDRegistered.Add(HUDName.SETTING, this.gameObject);
            UIManager.s_Instance.SelectHUD(gameObject);
        }
        private void Start()
        {
            LoadPlayerSetting();

        }

        void LoadPlayerSetting()
        {
            var isBGMMute = PlayerPrefs.GetInt("BGM") == 1 ? true : false;
            var isSFXMute = PlayerPrefs.GetInt("SFX") == 1 ? true : false;
            m_buttonBGM.GetComponent<Image>().sprite = isBGMMute ? m_soundOff : m_soundOn;
            m_buttonSFX.GetComponent<Image>().sprite = isSFXMute ? m_soundOff : m_soundOn;
        }

        public void SetupBGM()
        {
            var isBGMMute = SoundManager.s_Instance.IsBGMMute;
            if (isBGMMute)
            {
                PlayerPrefs.SetInt("BGM", 0);
                m_buttonBGM.GetComponent<Image>().sprite = m_soundOn;
                SoundManager.s_Instance.PlayBGM(BGMType.INGAME);
            }
            else
            {
                PlayerPrefs.SetInt("BGM", 1);
                m_buttonBGM.GetComponent<Image>().sprite = m_soundOff;
                SoundManager.s_Instance.StopBGM();
            }

            SoundManager.s_Instance.IsBGMMute = !isBGMMute;
        }

        public void SetupSFX()
        {
            var isSFXMute = SoundManager.s_Instance.IsSFXMute;
            if (isSFXMute)
            {
                PlayerPrefs.SetInt("SFX", 0);
                m_buttonSFX.GetComponent<Image>().sprite = m_soundOn;

            }
            else
            {
                PlayerPrefs.SetInt("SFX", 1);
                m_buttonSFX.GetComponent<Image>().sprite = m_soundOff;

            }

            SoundManager.s_Instance.IsSFXMute = !isSFXMute;
        }
        public void Close()
        {


            UIManager.s_Instance.CloseHUD(gameObject);
        }

        public void ExitGame()
        {

            Application.Quit();
        }

    }

}
