using AdverGame.PathFinding;
using AdverGame.Player;
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
        [SerializeField] int TotCustomersWalking = 0;
        GameObject m_taskHUD;
        Transform m_customerSpawnPostStart;
        Transform m_customerSpawnPostEnd;

        NavMeshGenerator m_generator;
        PathFinder m_pathFinder;


        [SerializeField] List<GameObject> m_customerVariants;
        [SerializeField] GameObject m_taskHUDPrefab;
        [SerializeField] GameObject m_orderTaskPrefab;
        [SerializeField] int m_maxCustomerRunning;
        [SerializeField] int m_maxCustomerQueued;

        [Header("PathFinding Setting")]
        [SerializeField] Rect m_rectSize;
        [SerializeField] LayerMask m_notWalkableLayer;
        [SerializeField] float m_pointDistributionSize;
        [SerializeField] bool m_isDrawPath;
        public List<Order> CustomerOrders { get; private set; }
        List<Task> m_taskOrders;
        public Queue<CustomerController> CustomersQueue;

        public Vector2[] m_path;
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
            m_generator = new(m_rectSize, m_pointDistributionSize, m_notWalkableLayer);
            m_pathFinder = new(m_generator);

            m_playerInput = PlayerManager.s_Instance.Player.InputBehaviour;

            SetupCustomers();

            m_taskHUD = Instantiate(m_taskHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_taskHUD.transform.SetAsFirstSibling();
        }

        private void OnDrawGizmos()
        {
            if (!m_isDrawPath) return;
            m_generator.OnDrawGizmos();
            Gizmos.color = Color.blue;

            if (m_path != null && m_path.Length > 0)
            {

                for (int i = 0; i < m_path.Length; i++)
                {
                    if (i + 1 < m_path.Length)
                        Gizmos.DrawLine(m_path[i], m_path[i + 1]);
                }
            }

        }

        void SetupCustomers()
        {
            m_customerSpawnPostStart = GameObject.Find("CustomerMinOffset").transform;
            m_customerSpawnPostEnd = GameObject.Find("CustomerMaxOffset").transform;
            SpawnCustomer();

            for (int i = 0; i < m_maxCustomerRunning; i++)
            {
                CommandCustomer();
            }
        }
        void SpawnCustomer()
        {
            CustomersQueue ??= new();

            var variantQuota = m_maxCustomerQueued / m_customerVariants.Count;
            var currentVariant = 1;
            for (int i = 1; i <= m_maxCustomerQueued; i++)
            {
                var variant = m_customerVariants[currentVariant - 1].GetComponent<CustomerController>();
                var widhtOffset = variant.DummyCharacter.GetComponent<SpriteRenderer>().bounds.size.x;
                var heightOffset = variant.DummyCharacter.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                var pos = SetRandomPos(widhtOffset, heightOffset);
                var newCust = GameObject.Instantiate(m_customerVariants[currentVariant - 1], pos.start, Quaternion.identity, GameObject.Find("Customer").transform).GetComponent<CustomerController>();

                CustomersQueue.Enqueue(newCust);
                newCust.TargetPos = pos.end;
                m_playerInput.OnLeftClick += newCust.OnTouch;
                newCust.OnCreateOrder += AddOrder;
                newCust.OnCancelOrder += RemoveOrder;
                newCust.OnResetPos += OnResetCustomer;
                newCust.OnToChair += DecreaseCustomerWalking;
                newCust.OnRequestPath += FindPath;
                newCust.SpawnDelay += i;

                currentVariant = currentVariant - (i / variantQuota) == 0 ? currentVariant + 1 : currentVariant;

            }

        }


        void FindPath(Vector2 position, Vector2 destination, CustomerController cust, CustomerState state)
        {

            if (state == CustomerState.TOCHAIR)
            {
                var path = m_pathFinder.FindPath(position, destination);
                m_path = path;
                cust.CurrentCoro = cust.StartCoroutine(cust.ToChair(path));
            }

            else if (state == CustomerState.LEAVE)
                cust.CurrentCoro = cust.StartCoroutine(cust.Leaving());

        }
        void DecreaseCustomerWalking()
        {



            TotCustomersWalking--;

            CommandCustomer();
        }
        void OnResetCustomer(CustomerController cust)
        {

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

            if (TotCustomersWalking >= m_maxCustomerRunning) return;

            TotCustomersWalking++;

            if (CustomersQueue == null || CustomersQueue.Count == 0) SpawnCustomer();
            var cus = CustomersQueue.Dequeue();

            cus.CurrentState = CustomerState.WALK;
            cus.CurrentCoro = cus.StartCoroutine(cus.walking());


        }
        (Vector2 start, Vector2 end) SetRandomPos(float widhtOffset, float heightOffset)
        {
            var rand = Random.Range(0, 2);
            var stageDimension = Camera.main.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
            var posStart = new Vector2(m_customerSpawnPostStart.position.x - widhtOffset, UnityEngine.Random.Range(m_customerSpawnPostStart.position.y, -(stageDimension.y - heightOffset * 2)));
            var posEnd = new Vector2(m_customerSpawnPostEnd.position.x + widhtOffset, posStart.y);
            return (rand == 1 ? (posStart, posEnd) : (posEnd, posStart));
        }


        #region Order
        void AddOrder(CustomerController obj, ItemSerializable order)
        {
            CustomerOrders ??= new();
            var cusOrder = new Order(order, obj);
            CustomerOrders.Add(cusOrder);

            var panel = m_taskHUD.transform.GetChild(0).GetComponent<RectTransform>();
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, panel.sizeDelta.y + 120);

            var task = Instantiate(m_orderTaskPrefab, m_taskHUD.transform.GetChild(0)).GetComponent<Task>();
            task.transform.GetChild(0).GetComponent<Image>().sprite = order.Content.Image;
            task.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;

            task.GetComponent<Task>().CustomerOrder = cusOrder;
            m_taskOrders ??= new();
            m_taskOrders.Add(task);

        }
        void RemoveOrder(ItemSerializable menu)
        {
            if (CheckOrder(menu, out Order order))
            {
                foreach (var task in m_taskOrders)
                {
                    if (task.CustomerOrder.Customer == order.Customer)
                    {

                        var panel = m_taskHUD.transform.GetChild(0).GetComponent<RectTransform>();
                        panel.sizeDelta = new Vector2(panel.sizeDelta.x, panel.sizeDelta.y - 130);
                        m_taskOrders.Remove(task);
                        Destroy(task.gameObject);
                        break;
                    }
                }
                CustomerOrders.Remove(order);
            }

        }
        public void GetOrder(Order menu)
        {

            foreach (var task in m_taskOrders)
            {
                if (task.CustomerOrder.Customer == menu.Customer)
                {
                    m_taskOrders.Remove(task);
                    Destroy(task.gameObject);
                    break;
                }
            }

            menu.Customer.ResetOrder();
            menu.Customer.CurrentState = CustomerState.EAT;

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

