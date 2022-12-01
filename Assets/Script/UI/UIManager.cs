

using AdverGame.Sound;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        GameObject m_notificationHUD;
        Coroutine m_notifState;

        [SerializeField] GameObject m_currentHUDSelected = null;
        [SerializeField] GameObject m_notificationHUDPrefab;
        [SerializeField] float m_notifTime;

        public AnimationCurve AnimCurve;
        public float AnimTime;

        public Dictionary<HUDName, GameObject> HUDRegistered;
        public bool isProhibited = false;


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
            if (isProhibited) return;
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
            if (isProhibited) return;
            LeanTween.scale(hud, Vector3.zero, AnimTime).setOnComplete(() => { hud.SetActive(false); });

        }
        public void ForceHUD(HUDName name)
        {


            if (HUDRegistered[name] != m_currentHUDSelected || !HUDRegistered[name].activeInHierarchy) SelectHUD(HUDRegistered[name]);
        }

        public void ShowNotification(string message)
        {
            m_notificationHUD ??= Instantiate(m_notificationHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            if(m_notifState!=null) StopCoroutine(m_notifState);
            m_notifState = StartCoroutine(ShowingNotif(message));

        }

        IEnumerator ShowingNotif(string message)
        {
            m_notificationHUD.SetActive(value: true);
            m_notificationHUD.transform.SetAsLastSibling();
            m_notificationHUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
            yield return new WaitForSeconds(m_notifTime);
            m_notificationHUD.SetActive(false);
        }
    }
}
