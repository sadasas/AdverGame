using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdverGame.MainMenu
{
	class MainMenu : MonoBehaviour
	{
		bool m_isMute = false;
		public void LoadScene()
		{
			SceneManager.LoadScene("Prototype", LoadSceneMode.Single);
		}
		public void SetupMusic()
		{
			
		}
	}
}
