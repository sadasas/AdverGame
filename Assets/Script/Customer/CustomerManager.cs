using AdverGame.Chair;
using AdverGame.Player;
using System;
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

        ChairManager m_chairManager;
        InputBehaviour m_playerInput;
        [SerializeField] int TotDummyWalking = 0;
        GameObject m_taskHUD;
        Transform m_customerSpawnPostStart;
        Transform m_customerSpawnPostEnd;

        [SerializeField] List<CustomerController> m_customerVariantsPrefabs;
        [SerializeField] List<DummyController> m_dummyVariantsPrefabs;


        [SerializeField] GameObject m_taskHUDPrefab;
        [SerializeField] GameObject m_orderTaskPrefab;
        [SerializeField] int m_maxCustomerRunning;
        [SerializeField] int m_maxCustomerQueued;

        public List<Order> CustomerOrders { get; private set; }
        List<Task> m_taskOrders;
        public Queue<CustomerController> RealCustomersQueue;
        public Queue<DummyController> DummyCustomersQueue;
        public Action<CustomerVariant> OnCustomerChoosed;
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
            m_chairManager = ChairManager.s_Instance;
            SetupCustomers();

            m_taskHUD = Instantiate(m_taskHUDPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_taskHUD.transform.SetAsFirstSibling();
        }


        void SetupCustomers()
        {
            m_customerSpawnPostStart = GameObject.Find("CustomerMinOffset").transform;
            m_customerSpawnPostEnd = GameObject.Find("CustomerMaxOffset").transform;
            SpawnCustomers();

            for (int i = 0; i < m_maxCustomerRunning; i++)
            {
                CommandCustomer();
            }
        }
        void SpawnCustomers()
        {
            // dummy
            SpawnDummy();

            //real
            SpawnRealCustomers();
        }

        void SpawnDummy()
        {
            var temDummy = new List<DummyController>();

            // spawn cust by their variant presentage spawned 
            // quota =  maxCustQueued % custRate
            var posVariant = new int[m_dummyVariantsPrefabs.Count];
            var persMax = 0;

            for (int i = 0; i < m_dummyVariantsPrefabs.Count; i++)
            {

                persMax += m_dummyVariantsPrefabs[i].Variant.OccurrencePercentage;
            }

            for (int i = 0; i < m_dummyVariantsPrefabs.Count; i++)
            {

                posVariant[i] = (int)Math.Round(((float)m_dummyVariantsPrefabs[i].Variant.OccurrencePercentage / persMax) * 100, MidpointRounding.AwayFromZero);
            }

            for (int i = 0; i < posVariant.Length; i++)
            {
                var quota = (int)Math.Round((float)posVariant[i] / 100 * m_maxCustomerQueued, MidpointRounding.AwayFromZero);
                for (int j = 0; j < quota; j++)
                {
                    var dc = m_dummyVariantsPrefabs[i].GetComponent<DummyController>();
                    var widhtOffset = dc.GetComponent<SpriteRenderer>().bounds.size.x;
                    var heightOffset = dc.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                    var pos = SetRandomPos(widhtOffset, heightOffset);
                    var newDummy = GameObject.Instantiate(m_dummyVariantsPrefabs[i], pos.start, Quaternion.identity, GameObject.Find("Customer").transform).GetComponent<DummyController>();


                    newDummy.TargetPos = pos.end;
                    m_playerInput.OnLeftClick += newDummy.OnTouch;
                    newDummy.OnToChair += SelectRealCustomer;
                    newDummy.OnResetPos += OnResetDummy;
                    newDummy.SpawnDelay += j;
                    newDummy.Setup();
                    temDummy.Add(newDummy);
                }

            }

            DummyCustomersQueue ??= new();



            // randomize dummy queue
            var dummyCount = temDummy.Count;
            while (dummyCount > 0)
            {
                var rand = UnityEngine.Random.Range(0, dummyCount);
                var newDum = temDummy[rand];
                DummyCustomersQueue.Enqueue(newDum);
                dummyCount--;
                temDummy.Remove(newDum);

            }


        }


        void SelectRealCustomer(List<ChairController> chairs, DummyController dummy, Vector2 defaultPos)
        {

            if (RealCustomersQueue.Count == 0) SpawnRealCustomers();
            var cust = RealCustomersQueue.Peek();
            ChairController tempChair = null;

            while (cust.Variant.Type == CustomerType.OJOL && tempChair == null)
            {
                foreach (var chair in chairs)
                {
                    if (chair.Type == ChairType.DRIVERS)
                    {
                        RealCustomersQueue.Dequeue();
                        tempChair = chair;
                        chair.Customer = cust;
                        OnCustomerChoosed?.Invoke(cust.Variant);
                        break;
                    }
                }
                if (tempChair == null)
                {
                    RealCustomersQueue.Dequeue();

                    if (RealCustomersQueue.Count == 0)
                    {
                        SpawnRealCustomers();
                    }
                    cust = RealCustomersQueue.Peek();
                }

            }
            if (tempChair == null)
            {
                foreach (var chair in chairs.ToArray())
                {
                    if (chair.Type == ChairType.DRIVERS) chairs.Remove(chair);
                }
                if (chairs == null || chairs.Count == 0)
                {
                    dummy.CurrentCoro = dummy.StartCoroutine(dummy.WaitChairAvailable(cust.Variant.Type));
                    return;
                }
                int rand = UnityEngine.Random.Range(0, chairs.Count);
                tempChair = chairs[rand];
                chairs[rand].Customer = cust;
                RealCustomersQueue.Dequeue();
                OnCustomerChoosed?.Invoke(cust.Variant);
            }

            cust.m_currentChair = tempChair;
            cust.transform.position = dummy.transform.position;
            cust.TargetPos = tempChair.transform.position;
            cust.DefaultPos = defaultPos;
            cust.CurrentCoro = cust.StartCoroutine(cust.ToChair());


            OnResetDummy(dummy);



        }
        void SpawnRealCustomers()
        {

            // spawn cust by their variant presentage spawned 
            // quota =  maxCustQueued % custRate

            var posVariant = new int[m_customerVariantsPrefabs.Count];
            var persmax = 0;

            for (int i = 0; i < m_customerVariantsPrefabs.Count; i++)
            {

                persmax += m_customerVariantsPrefabs[i].Variant.OccurrencePercentage;
            }

            for (int i = 0; i < m_customerVariantsPrefabs.Count; i++)
            {

                posVariant[i] = (int)Math.Round(((float)m_customerVariantsPrefabs[i].Variant.OccurrencePercentage / persmax) * 100, MidpointRounding.AwayFromZero);
            }

            var tempCustomers = new List<CustomerController>();
            for (int i = 0; i < posVariant.Length; i++)
            {
                var quota = (int)Math.Round((float)posVariant[i] / 100 * m_maxCustomerQueued, MidpointRounding.AwayFromZero);
                for (int j = 0; j < quota; j++)
                {
                    var variant = m_customerVariantsPrefabs[i].GetComponent<CustomerController>();
                    var widhtOffset = variant.GetComponent<SpriteRenderer>().bounds.size.x;
                    var heightOffset = variant.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                    var pos = SetRandomPos(widhtOffset, heightOffset);
                    var newCust = GameObject.Instantiate(m_customerVariantsPrefabs[i], pos.start, Quaternion.identity, GameObject.Find("Customer").transform);

                    m_playerInput.OnLeftClick += newCust.OnTouch;
                    newCust.OnCreateOrder += AddOrder;
                    newCust.OnCancelOrder += RemoveOrder;




                    tempCustomers.Add(newCust);
                }

            }

            RealCustomersQueue ??= new();



            // randomize dummy queue
            var custCount = tempCustomers.Count;
            while (custCount > 0)
            {
                var rand = UnityEngine.Random.Range(0, custCount);
                var newCust = tempCustomers[rand];
                RealCustomersQueue.Enqueue(newCust);
                custCount--;
                tempCustomers.Remove(newCust);

            }


        }

        void OnResetDummy(DummyController dummy)
        {
            TotDummyWalking--;

            var widhtOffset = dummy.GetComponent<SpriteRenderer>().bounds.size.x;
            var heightOffset = dummy.GetComponent<SpriteRenderer>().bounds.size.y / 2;
            var pos = SetRandomPos(widhtOffset, heightOffset);

            dummy.transform.position = pos.start;
            dummy.TargetPos = pos.end;
            dummy.Setup();

            DummyCustomersQueue.Enqueue(dummy);
            CommandCustomer();
        }
        void CommandCustomer()
        {

            if (TotDummyWalking >= m_maxCustomerRunning) return;

            TotDummyWalking++;

            if (DummyCustomersQueue == null || DummyCustomersQueue.Count == 0) SpawnCustomers();



            var cus = DummyCustomersQueue.Dequeue();


            cus.CurrentCoro = cus.StartCoroutine(cus.Walking());

        }

        (Vector2 start, Vector2 end) SetRandomPos(float widhtOffset, float heightOffset)
        {
            var rand = UnityEngine.Random.Range(0, 2);
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
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, panel.sizeDelta.y + m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.y);

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
                        panel.sizeDelta = new Vector2(panel.sizeDelta.x, panel.sizeDelta.y - m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.y);
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
                    var panel = m_taskHUD.transform.GetChild(0).GetComponent<RectTransform>();
                    panel.sizeDelta = new Vector2(panel.sizeDelta.x, panel.sizeDelta.y - m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.y);
                    m_taskOrders.Remove(task);
                    Destroy(task.gameObject);
                    break;
                }
            }

            menu.Customer.ResetOrder();
            menu.Customer.StopCoroutine(menu.Customer.CurrentCoro);
            CustomerOrders.Remove(menu);
            if (menu.Customer.Variant.Type != CustomerType.OJOL)
            {
                menu.Customer.CurrentState = CustomerState.EAT;
                menu.Customer.CurrentCoro = menu.Customer.StartCoroutine(menu.Customer.Eating());
            }
            else
            {
                menu.Customer.CurrentState = CustomerState.PAY;
                menu.Customer.Pay();

            }


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

