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
        public int platesCount;
        
        public Action<ItemPlate,ItemSerializable> OnItemChoosed;

        private void Start()
        {
            SpawnPlate(platesCount);
            UpdateItemCooked(0);
        }

      

        public void SpawnPlate(int tot)
        {
            m_plates ??= new();

            for (int i = 0; i < platesCount; i++)
            {
                var newPlate = Instantiate(m_platePrefab, m_platePlace).GetComponent<ItemPlate>();
                m_plates.Add(newPlate);
            }
        }

        public void SpawnItem(ItemSerializable item)
        {
            var currentSizeDeltaPar = m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta;
            var newSizeDeltaPar = new Vector2(m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta.x + 250, m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta.y);
            m_itemSelectionsPlace.GetComponent<RectTransform>().sizeDelta = newSizeDeltaPar;

            var newSelection = Instantiate(m_itemSelectionPrefab, m_itemSelectionsPlace).GetComponent<ItemCookSelection>();
            newSelection.Content = item;
            newSelection.OnChoosed += StartCooking;
            newSelection.GetComponent<Image>().sprite = item.Content.Image;
            newSelection.GetComponent<Image>().preserveAspect = true;

        }
        void StartCooking(ItemSerializable item)
        {
            if (m_plates == null || m_plates.Count == 0) return;
            foreach (var plate in m_plates)
            {

                if (plate.IsEmpty)
                {
                    OnItemChoosed?.Invoke(plate,item);
                    
                }
            }

        }
        public void UpdateItemCooked(int itemCooked)
        {
            m_itemCooked += itemCooked;
            m_proggres.text = $"{m_itemCooked} / {m_plates.Count}";
        }
        public void SpawnPlate()
        {
            m_plates ??= new();

            var newPlate = Instantiate(m_platePrefab, m_platePlace).GetComponent<ItemPlate>();
            m_plates.Add(newPlate);
        }


    }
}
