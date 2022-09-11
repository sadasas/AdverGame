using AdverGame.Player;
using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField] List<GameObject> m_customerPrefab;

        public List<CustomerController> Customers;

        public List<Order> CustomerOrders { get; private set; }

        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            m_playerInput = PlayerManager.s_Instance.Player.InputBehaviour;

            SpawnCustomer();
        }

        void SpawnCustomer()
        {
            Customers ??= new();
            for (int i = 0; i < m_customerPrefab.Count; i++)
            {

                var newCust = GameObject.Instantiate(m_customerPrefab[i]).GetComponent<CustomerController>();
                Customers.Add(newCust);
                m_playerInput.OnLeftClick += newCust.OnTouch;
                newCust.OnCreateOrder += AddOrder;
                newCust.OnCancelOrder += RemoveOrder;

            }
        }
        void AddOrder(CustomerController obj, ItemSerializable order)
        {
            CustomerOrders ??= new();
            var cusOrder = new Order(order, obj);
            CustomerOrders.Add(cusOrder);

            Debug.Log(CustomerOrders.Count);
        }

        void RemoveOrder(ItemSerializable menu)
        {
            if (CheckOrder(menu, out Order order)) CustomerOrders.Remove(order);
        }

        public void GetOrder(Order menu)
        {
            menu.Customer.ResetOrder();
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

    }
}

