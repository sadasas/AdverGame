using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace AdverGame.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        bool m_isMute = false;

        [SerializeField] GameObject m_loadHUD;
        [SerializeField] GameObject m_buttonBGM;
        [SerializeField] Slider m_loadSlider;
        [SerializeField] Sprite m_bgmOff;
        [SerializeField] Sprite m_bgmOn;


        public void LoadScene()
        {
            StartCoroutine(ProcessLoadScene());
        }


        IEnumerator ProcessLoadScene()
        {
            AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            while (!asyncLoadScene.isDone)
            {
                m_loadHUD.SetActive(true);
                m_loadSlider.value = asyncLoadScene.progress;

                yield return null;

            }
            yield return new WaitForEndOfFrame();
            m_loadHUD.SetActive(false);
        }


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
