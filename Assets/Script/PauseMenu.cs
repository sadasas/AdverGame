
using UnityEngine;

namespace AdverGame
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject m_pauseMenuHUD;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
               
                m_pauseMenuHUD.SetActive(true);
                Time.timeScale = 0f;
            }
          
        }

        public void Cancel()
        {
            m_pauseMenuHUD.SetActive(false);
            Time.timeScale = 1f;
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
