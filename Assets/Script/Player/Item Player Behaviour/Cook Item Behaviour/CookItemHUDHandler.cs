using AdverGame.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class CookItemHUDHandler : MonoBehaviour
    {

        List<ItemPlate> m_plates;
        int m_itemCooked;
        [SerializeField] GameObject m_platePrefab;
        [SerializeField] GameObject m_itemSelectionPrefab;
        [SerializeField] Transform m_platePlace;
        [SerializeField] Transform m_itemSelectionsPlace;
        [SerializeField] TextMeshProUGUI m_proggres;
        [SerializeField] GameObject m_chef;

        public float m_workTime;
        public Action<ItemPlate, ItemSerializable> OnItemChoosed;

        private void Start()
        {

            UpdateItemCooked(0);
        }

        private void Update()
        {
            if(m_workTime>0)
            {
                m_workTime -= Time.deltaTime;
                if (!m_chef.activeInHierarchy) m_chef.SetActive(true);
               
            }
            else
            {
                m_workTime = 0;
                if (m_chef.activeInHierarchy) m_chef.SetActive(false);
            }
        }

        public void SpawnPlate(int tot)
        {
            m_plates ??= new();

            for (int i = 0; i < tot; i++)
            {
                var newPlate = Instantiate(m_platePrefab, m_platePlace).GetComponent<ItemPlate>();
                m_plates.Add(newPlate);
            }
        }

        public void SpawnItem(ItemSerializable item)
        {
            var currentSizeDeltaPar = m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta;
            var itemSelRect = m_itemSelectionPrefab.GetComponent<RectTransform>();

            var newSizeDeltaPar = new Vector2(m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta.x + itemSelRect.sizeDelta.x + 50, m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta.y);
            m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta = newSizeDeltaPar;

            var newSelection = Instantiate(m_itemSelectionPrefab, m_itemSelectionsPlace).GetComponent<ItemCookSelection>();
            newSelection.Content = item;
            newSelection.OnChoosed += StartCooking;
            newSelection.transform.GetChild(0).GetComponent<Image>().sprite = item.Content.Image;
            newSelection.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;
            newSelection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Content.Name;


        }
        void StartCooking(ItemSerializable item)
        {
            if (m_plates == null || m_plates.Count == 0) return;
            foreach (var plate in m_plates)
            {

                if (plate.IsEmpty)
                {
                    OnItemChoosed?.Invoke(plate, item);
                    break;

                }
            }

        }
        public void UpdateItemCooked(int itemCooked)
        {
            m_itemCooked += itemCooked;
            m_proggres.text = $"{m_itemCooked}/{m_plates.Count}";
            if (m_itemCooked == m_plates.Count) m_chef.SetActive(false);
            else
            {
                m_chef.SetActive(true);
            }
        }
        public void SpawnPlate()
        {
            m_plates ??= new();

            var newPlate = Instantiate(m_platePrefab, m_platePlace).GetComponent<ItemPlate>();
            m_plates.Add(newPlate);
        }

        public void Close()
        {
            UIManager.s_Instance.CloseHUD(gameObject);
        }
    }
}
