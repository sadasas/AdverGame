

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

        GameObject s_CurrentHUDSelected = null;

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
            if (s_CurrentHUDSelected != null)
            {
                if (s_CurrentHUDSelected == hud) return;
                s_CurrentHUDSelected.SetActive(false);
            }

            s_CurrentHUDSelected = hud;
        }

        public void FindHUD(HUDName name)
        {

            HUDRegistered[name].SetActive(true);
            if (HUDRegistered[name] != s_CurrentHUDSelected) SelectHUD(HUDRegistered[name]);
        }
    }
}
