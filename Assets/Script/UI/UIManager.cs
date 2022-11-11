

using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.UI
{

    public enum HUDName
    {
        FIND_ITEM,
        ITEM_AVAILABLE,
        COOK_ITEM,
        HYPERLINK,
        NEWCHARACTERNOTIF,
        SETTING,
        CHARACTERCOLLECTION,
        CHARACTERCOLLECTIONDETAIL


    }
    public class UIManager : MonoBehaviour
    {
        public static UIManager s_Instance;

        [SerializeField] GameObject m_currentHUDSelected = null;
        [SerializeField] AnimationCurve m_animationCurve;
        [SerializeField] float m_animTime;

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

                if (m_currentHUDSelected != hud)
                {
                    CloseHUD(m_currentHUDSelected);
                };
            }
            hud.SetActive(true);

            LeanTween.scale(hud, Vector3.one, m_animTime).setEase(m_animationCurve)
            .setOnComplete(() =>
            {
                m_currentHUDSelected = hud;
                m_currentHUDSelected.transform.SetAsLastSibling();
            });

        }

        public void CloseHUD(GameObject hud)
        {
            LeanTween.scale(hud, Vector3.zero, m_animTime).setEase(m_animationCurve).setOnComplete(() => { m_currentHUDSelected.SetActive(false); });

        }
        public void ForceHUD(HUDName name)
        {

            HUDRegistered[name].SetActive(true);
            if (HUDRegistered[name] != m_currentHUDSelected) SelectHUD(HUDRegistered[name]);
        }
    }
}
