﻿using AdverGame.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Customer
{

    public struct Order
    {
        public ItemSerializable ItemOrder { get; private set; }
        public CustomerController Customer { get; private set; }

        public Order(ItemSerializable itemOrder, CustomerController customer)
        {
            ItemOrder = itemOrder;
            Customer = customer;
        }

    }
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager s_Instance;

        InputBehaviour m_playerInput;
        int TotCustomersWalking = 0;
        GameObject m_taskHUD;
        Transform m_customerSpawnPostStart;
        Transform m_customerSpawnPostEnd;

        [SerializeField] List<GameObject> m_customerPrefab;
        [SerializeField] GameObject m_taskHUDPrefab;
        [SerializeField] GameObject m_orderTaskPrefab;
        [SerializeField] int m_maxCustomerRunning;
        [SerializeField] int m_maxCustomerQueued;

        public List<Order> CustomerOrders { get; private set; }
        public List<CustomerController> m_customerRunning;
        public Queue<CustomerController> CustomersQueue;

        private void OnValidate()
        {

            if (m_maxCustomerQueued < m_maxCustomerRunning) m_maxCustomerQueued = m_maxCustomerRunning;


        }
        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            m_playerInput = PlayerManager.s_Instance.Player.InputBehaviour;

            SetupCustomers();

            m_taskHUD = Instantiate(m_taskHUDPrefab, GameObject.Find("Menu Available Button Pos").transform.position + Vector3.down * 600, Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);

        }


        public void RetargetCustomersPos(float targetXPos)
        {
            foreach (var cus in CustomersQueue)
            {
                cus.DefaultPos = new Vector2(cus.transform.position.x + targetXPos, cus.transform.position.y);
                cus.TargetPos = new Vector2(cus.TargetPos.x + targetXPos, cus.TargetPos.y);
                cus.transform.position = cus.DefaultPos;

            }

        }
        void SetupCustomers()
        {
            m_customerSpawnPostStart = GameObject.Find("CustomerPosStart").transform;
            m_customerSpawnPostEnd = GameObject.Find("CustomerPosEnd").transform;
            SpawnCustomer();
            
            for (int i = 0; i < m_maxCustomerRunning; i++)
            {

                CommandCustomer();
            }
        }
        void SpawnCustomer()
        {
            CustomersQueue ??= new();

            var variantQuota = m_maxCustomerQueued / m_customerPrefab.Count;
            var currentVariant = 1;
            for (int i = 1; i <= m_maxCustomerQueued; i++)
            {
                var widhtOffset = m_customerPrefab[currentVariant - 1].GetComponent<SpriteRenderer>().bounds.size.x;
                var heightOffset = m_customerPrefab[currentVariant - 1].GetComponent<SpriteRenderer>().bounds.size.y / 2;
                var pos = SetRandomPos(widhtOffset, heightOffset);
                var newCust = GameObject.Instantiate(m_customerPrefab[currentVariant - 1], pos.start, Quaternion.identity).GetComponent<CustomerController>();
                newCust.TargetPos = pos.end;
                CustomersQueue.Enqueue(newCust);
                m_playerInput.OnLeftClick += newCust.OnTouch;
                newCust.OnCreateOrder += AddOrder;
                newCust.OnCancelOrder += RemoveOrder;
                newCust.OnResetPos += OnResetCustomer;
                newCust.OnToChair += DecreaseCustomerWalking;
                newCust.SpawnDelay += i;

                currentVariant = currentVariant - (i / variantQuota) == 0 ? currentVariant + 1 : currentVariant;

            }

        }

        void DecreaseCustomerWalking()
        {
            TotCustomersWalking--;

            CommandCustomer();
        }
        void OnResetCustomer(CustomerController cust)
        {
            m_customerRunning.Remove(cust);

            if (cust.CurrentState == CustomerState.WALK) TotCustomersWalking--;

            var pos = SetRandomPos(cust.widhtOffset, cust.heightOffset);
            cust.DefaultPos = pos.start;
            cust.TargetPos = pos.end;
            cust.ResetPos();

            CustomersQueue.Enqueue(cust);
            CommandCustomer();
        }
        void CommandCustomer()
        {

            if (TotCustomersWalking >= m_maxCustomerRunning)
            {

                return;
            }

            TotCustomersWalking++;

            if (CustomersQueue == null || CustomersQueue.Count == 0) SpawnCustomer();
            var cus = CustomersQueue.Dequeue();
            cus.CurrentState = CustomerState.WALK;

            m_customerRunning ??= new();
            m_customerRunning.Add(cus);



        }

        (Vector2 start,Vector2 end) SetRandomPos(float widhtOffset, float heightOffset)
        {
            var rand = Random.Range(0, 2);
            var stageDimension = Camera.main.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
            var posStart = new Vector2(m_customerSpawnPostStart.position.x - widhtOffset, UnityEngine.Random.Range(m_customerSpawnPostStart.position.y, -(stageDimension.y - heightOffset)));
            var posEnd = new Vector2(m_customerSpawnPostEnd.position.x + widhtOffset, posStart.y);
            return (rand == 1 ? (posStart, posEnd) : (posEnd, posStart));
        }

        (Vector2 start, Vector2 end) SetRandomPosByCamera(float widhtOffset, float heightOffset)
        {

            var rand = Random.Range(0, 2);
            var stageDimensionStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.currentResolution.height, 0));
            var stageDimensionEnd = Camera.main.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));

            var posStart = new Vector2(stageDimensionStart.x, UnityEngine.Random.Range(-3, -(stageDimensionStart.y - heightOffset)));
            var posEnd = new Vector2(stageDimensionEnd.x, posStart.y);
            return (rand == 1 ? (posStart, posEnd) : (posEnd, posStart));
        }


        #region Order
        void AddOrder(CustomerController obj, ItemSerializable order)
        {
            CustomerOrders ??= new();
            var cusOrder = new Order(order, obj);
            CustomerOrders.Add(cusOrder);
            var task = Instantiate(m_orderTaskPrefab, m_taskHUD.transform);
            task.GetComponent<Image>().sprite = order.Content.Image;
            task.GetComponent<Task>().CustomerOrder = cusOrder;

        }

        void RemoveOrder(ItemSerializable menu)
        {
            if (CheckOrder(menu, out Order order))
            {
                Destroy(m_taskHUD.transform.GetChild(CustomerOrders.IndexOf(order)).gameObject);
                CustomerOrders.Remove(order);
            }

        }

        public void GetOrder(Order menu)
        {
            Destroy(m_taskHUD.transform.GetChild(CustomerOrders.IndexOf(menu)).gameObject);
            menu.Customer.ResetOrder();
            OnResetCustomer(menu.Customer);
            menu.Customer.ResetPos();
            menu.Customer.Pay();
            CustomerOrders.Remove(menu);


        }

        public bool CheckOrder(ItemSerializable menu, out Order order)
        {
            order = new();
            if (CustomerOrders == null) return false;
            foreach (var item in CustomerOrders)
            {
                if (item.ItemOrder.Content.name.Equals(menu.Content.name))
                {
                    order = item;
                    return true;
                }

            }

            return false;
        }

        #endregion
    }
}

