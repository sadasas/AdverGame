

using AdverGame.Sound;
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
        CHARACTERCOLLECTIONDETAIL,
        NEWLEVEL


    }
    public class UIManager : MonoBehaviour
    {
        public static UIManager s_Instance;

        [SerializeField] GameObject m_currentHUDSelected = null;

        public AnimationCurve AnimCurve;
        public float AnimTime;

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
            SoundManager.s_Instance.PlaySFX(SFXType.BTNCLICK);
            if (m_currentHUDSelected != null)
            {

                if (m_currentHUDSelected != hud)
                {
                    CloseHUD(m_currentHUDSelected);
                };
            }
            hud.SetActive(true);

            LeanTween.scale(hud, Vector3.one, AnimTime).setEase(AnimCurve)
            .setOnComplete(() =>
            {
                m_currentHUDSelected = hud;
                m_currentHUDSelected.transform.SetAsLastSibling();
            });

        }

        public void OverlapHUD(GameObject hud)
        {
            SoundManager.s_Instance.PlaySFX(SFXType.BTNCLICK);
            hud.SetActive(true);

            LeanTween.scale(hud, Vector3.one, AnimTime).setEase(AnimCurve)
            .setOnComplete(() =>
            {
                m_currentHUDSelected = hud;
                m_currentHUDSelected.transform.SetAsLastSibling();
            });
        }
        public void CloseHUD(GameObject hud)
        {
            LeanTween.scale(hud, Vector3.zero, AnimTime).setEase(AnimCurve).setOnComplete(() => { m_currentHUDSelected.SetActive(false); });

        }
        public void ForceHUD(HUDName name)
        {


            if (HUDRegistered[name] != m_currentHUDSelected || !HUDRegistered[name].activeInHierarchy) SelectHUD(HUDRegistered[name]);
        }
    }
}
