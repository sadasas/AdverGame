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
        [SerializeField] Slider m_loadSlider;


        public void LoadScene()
        {
            StartCoroutine(ProcessLoadScene());
        }
        public void SetupMusic()
        {

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
    }
}
