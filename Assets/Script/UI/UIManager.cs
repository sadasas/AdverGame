

using UnityEngine;

namespace AdverGame.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager s_Instance;

        GameObject s_CurrentHUDSelected = null;

        private void Awake()
        {
            if (s_Instance) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(this);
        }

        public void SelectHUD(GameObject hud)
        {
            if (s_CurrentHUDSelected != null)
            {
                if (s_CurrentHUDSelected == hud) return;
                s_CurrentHUDSelected.SetActive(false);
            }

            s_CurrentHUDSelected = hud;
        }
    }
}
