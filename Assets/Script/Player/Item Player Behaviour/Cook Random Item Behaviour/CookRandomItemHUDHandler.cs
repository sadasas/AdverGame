using AdverGame.Sound;
using AdverGame.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    /// <summary>
    /// TODO: Refactor
    /// </summary>
    public class CookRandomItemHUDHandler : MonoBehaviour
    {
        float m_timeSlerp = 0;
        int m_itemFound = 0;
        private float m_itemFoundMax;
        private float m_maxSearchTime = 0;
        SoundManager sm;
        [SerializeField] Slider m_slider;
        [SerializeField] Transform m_itemPlace;
        [SerializeField] TextMeshProUGUI m_itemFounded;
        [SerializeField] GameObject m_adverHUDPrefab;
        [SerializeField] GameObject m_getInstantItemButton;
        [SerializeField] GameObject m_getItemButton;
        [SerializeField] GameObject m_itemFoundedPrefab;
        [SerializeField] GameObject m_chef;

        [field: SerializeField]
        public List<GameObject> m_itemDisplayed { get; private set; }

        public Action OnGetTriggered;
        public Action OnResetTriggered;
        public Action OnInstantSearchItemTriggered;

        private void OnEnable()
        {
            SetupAmbience();
        }
        private void OnDisable()
        {
            sm = SoundManager.s_Instance;
            if (sm != null) SoundManager.s_Instance.StopAmbience();
        }
        void SetupAmbience()
        {
            sm = SoundManager.s_Instance;
            if (m_itemFound < m_itemFoundMax)
            {
                if (sm != null) sm.PlayAmbience(AmbienceType.KITCHEN);
            }

        }
        private void Update()
        {

            if (m_timeSlerp < m_maxSearchTime)
            {
                var a = 1f / (m_itemFoundMax / m_itemFound);
                var b = (1f / m_itemFoundMax) * (m_timeSlerp / m_maxSearchTime);

                m_slider.value = a + b;
                m_timeSlerp += Time.deltaTime;

            }

            if (m_itemFound == m_itemFoundMax && m_chef.activeInHierarchy)
            {
                if (sm != null) sm.StopAmbience();
                m_chef.SetActive(false);
            }
            else if (m_itemFound != m_itemFoundMax)
            {

                if (!m_chef.activeInHierarchy) m_chef.SetActive(true);
            }
        }

        public void DisplayItemFounded(ItemSerializable itemfounded)
        {
            m_itemDisplayed ??= new();
            var obj = Instantiate(m_itemFoundedPrefab, m_itemPlace.transform);
            obj.transform.GetChild(0).GetComponent<Image>().sprite = itemfounded.Content.Image;
            obj.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemfounded.Stack.ToString();
            obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = itemfounded.Content.Name;

            m_itemDisplayed.Add(obj);


        }

        public void TrackItemFinded(int itemfindedTot, float itemMaxFounded, float maxSearchTime)
        {
            m_timeSlerp = 0f;
            m_itemFound = itemfindedTot;
            m_itemFoundMax = itemMaxFounded;
            m_maxSearchTime = maxSearchTime;
            m_itemFounded.text = itemfindedTot.ToString() + " / " + itemMaxFounded;


            if (itemfindedTot == itemMaxFounded) m_getInstantItemButton.SetActive(false);
            else m_getInstantItemButton.SetActive(true);

            if (itemfindedTot == 0) m_getItemButton.SetActive(false);
            else m_getItemButton.SetActive(true);

        }

        public void GetItemInstant()
        {
            Instantiate(m_adverHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);

            OnInstantSearchItemTriggered?.Invoke();

            m_itemPlace.parent.gameObject.SetActive(true);

        }
        public void Close()
        {
            UIManager.s_Instance.CloseHUD(gameObject);
        }



        public void GetItemFinded()
        {
            m_getInstantItemButton.SetActive(false);
            m_itemPlace.parent.gameObject.SetActive(true);
            OnGetTriggered?.Invoke();
        }

        public void ResetItemFinded()
        {

            OnResetTriggered?.Invoke();

            if (m_itemDisplayed == null || m_itemDisplayed.Count == 0) return;
            foreach (var item in m_itemDisplayed)
            {
                Destroy(item);
            }
            m_itemDisplayed = null;
            m_slider.value = 0;
            m_itemFounded.text = "0 / 8";
            m_itemPlace.parent.gameObject.SetActive(false);
            m_getItemButton.SetActive(false);
            m_getInstantItemButton.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }


}
