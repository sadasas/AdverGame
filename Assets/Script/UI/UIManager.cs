

using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.UI
{

    public enum HUDName
    {
        FIND_ITEM,
        ITEM_AVAILABLE,

    }
    public class UIManager : MonoBehaviour
    {
        public static UIManager s_Instance;

        GameObject m_currentHUDSelected = null;

        public Dictionary<HUDName, GameObject> HUDRegistered;

        private void Awake()
        {
            if (s_Instance) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(this);
        }
        private void Start()
        {
            HUDRegistered = new();
        }

        public void SelectHUD(GameObject hud)
        {
            if (m_currentHUDSelected != null)
            {
                if (m_currentHUDSelected == hud) return;
                m_currentHUDSelected.SetActive(false);
            }

            m_currentHUDSelected = hud;
        }

        public void ForceHUD(HUDName name)
        {

            HUDRegistered[name].SetActive(true);
            if (HUDRegistered[name] != m_currentHUDSelected) SelectHUD(HUDRegistered[name]);
        }
    }
}
