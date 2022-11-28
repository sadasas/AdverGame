using AdverGame.Customer;
using AdverGame.Sound;
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
        UncompletedTask m_currentTaskShowDetail;
        List<ItemPlate> m_plates;
        int m_itemCooked;
        int m_itemCooking;
        List<UncompletedTask> m_tasksDisplayed;
        [SerializeField] GameObject m_platePrefab;
        [SerializeField] GameObject m_itemSelectionPrefab;
        [SerializeField] Transform m_platePlace;
        [SerializeField] Transform m_itemSelectionsPlace;
        [SerializeField] TextMeshProUGUI m_proggres;
        [SerializeField] GameObject m_chef;
        [SerializeField] DrinksPlate m_drinkPlate;
        [SerializeField] GameObject m_HUDTaskUncompleted;
        [SerializeField] GameObject m_orderTaskPrefab;
        [SerializeField] GameObject m_HUDOrderDetail;

        public Action<ItemPlate, ItemSerializable> OnItemChoosed;
        public bool isProhibited = false;
        private void Start()
        {

            UpdateItemCooked(0);
        }

        private void OnEnable()
        {
            SetupAmbience();
        }

        private void OnDisable()
        {
            var sm = SoundManager.s_Instance;
            if (sm != null) SoundManager.s_Instance.StopAmbience();
        }
        void SetupAmbience()
        {
            var sm = SoundManager.s_Instance;
            if (m_itemCooking > 0)
            {
                if (sm != null) sm.PlayAmbience(AmbienceType.KITCHEN);
            }

        }
        public void SetupChef(int itemcooking)
        {
            m_itemCooking = itemcooking;
            var sm = SoundManager.s_Instance;
            if (itemcooking <= 0)
            {
                m_chef.SetActive(false);
                if (sm != null) sm.StopAmbience();
            }
            else
            {
                if (sm != null) sm.PlayAmbience(AmbienceType.KITCHEN);
                m_chef.SetActive(value: true);
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
            if (isProhibited) return;
            if (item.Content.Type == MenuType.FOOD)
            {
                if (m_plates == null || m_plates.Count == 0) return;
                foreach (var plate in m_plates)
                {

                    if (plate.IsEmpty)
                    {
                        OnItemChoosed?.Invoke(plate, item);
                        return;

                    }
                }

                UIManager.s_Instance.ShowNotification("Kompor penuh");
            }
            else
            {
                if (m_drinkPlate.IsEmpty)
                {
                    OnItemChoosed?.Invoke(m_drinkPlate, item);
                    return;
                }
                UIManager.s_Instance.ShowNotification(message: "Tempat minuman penuh");
            }


        }
        public void DisplayTaskUncompleted(Task order)
        {
            m_tasksDisplayed ??= new();
            var panel = m_HUDTaskUncompleted.transform.GetChild(0).GetComponent<RectTransform>();
            panel.sizeDelta = new Vector2(panel.sizeDelta.x + m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.x, panel.sizeDelta.y);

            var cusOrder = new Order(order.CustomerOrder.ItemOrder, order.CustomerOrder.Customer);
            var task = Instantiate(m_orderTaskPrefab, m_HUDTaskUncompleted.transform.GetChild(0)).GetComponent<UncompletedTask>();
            task.transform.GetChild(0).GetComponent<Image>().sprite = order.CustomerOrder.ItemOrder.Content.Image;
            task.CustomerOrder = cusOrder;
            task.m_HUDOrderDetail = m_HUDOrderDetail;
            task.OnShowDetail += ShowDetailOrder;
            m_tasksDisplayed.Add(task);
        }

        void ShowDetailOrder(UncompletedTask task)
        {
            m_currentTaskShowDetail = task;
        }
        public void RemoveTaskUncompleted(Task order)
        {
            var panel = m_HUDTaskUncompleted.transform.GetChild(0).GetComponent<RectTransform>();
            panel.sizeDelta = new Vector2(panel.sizeDelta.x - m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.x, panel.sizeDelta.y);

            foreach (var item in m_tasksDisplayed.ToArray())
            {
                if (item.CustomerOrder.Customer == order.CustomerOrder.Customer)
                {
                    item.OnShowDetail -= ShowDetailOrder;
                    if (item == m_currentTaskShowDetail) m_HUDOrderDetail.SetActive(false);
                    m_tasksDisplayed.Remove(item);
                    Destroy(item.gameObject);
                    break;
                }
            }
        }
        public void UpdateItemCooked(int itemCooked)
        {
            m_itemCooked += itemCooked;
            m_proggres.text = $"{m_itemCooked}/{m_plates.Count + 1} ";

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
