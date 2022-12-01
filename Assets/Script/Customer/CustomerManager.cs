using AdverGame.Chair;
using AdverGame.Player;
using AdverGame.UI;
using AdverGame.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Customer
{
    /// <summary>
    /// TODO: make pathfinding for customer when walk to their chair destination
    /// </summary>
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager s_Instance;

        ChairManager m_chairManager;
        InputBehaviour m_playerInput;
        GameObject m_taskHUD;
        Transform m_customerSpawnPostStart;
        Transform m_customerSpawnPostEnd;
        Task m_selectedTask;

        [SerializeField] List<CustomerController> m_customerVariants;
        [SerializeField] List<DummyController> m_dummyVariantsPrefabs;


        [SerializeField] GameObject m_taskHUDPrefab;
        [SerializeField] GameObject m_orderTaskPrefab;
        [SerializeField] int m_maxCustomerRunning;
        [SerializeField] int m_maxCustomerQueued;

        public List<Task> m_taskOrders { get; set; }

        public List<DummyController> DummyRunning;
        public List<CustomerController> CustomersRunning;
        public Queue<CustomerController> RealCustomersQueue;
        public Queue<DummyController> DummyCustomersQueue;
        public Action<CustomerVariant> OnCustomerChoosed;
        public Action<Task> OnAddOrder;
        public Action<Task> OnRemoveOrder;
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

            PlayerManager.s_Instance.OnIncreaseLevel += UpdateLevel;
            OnAddOrder += PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.AddTaskUnCompleted;
            OnRemoveOrder += PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.RemoveUncompleteOrder;
        }

        void UpdateLevel(Level newLevel)
        {
            if (newLevel.VariantCust == null || newLevel.VariantCust.Length == 0) return;
            foreach (var variant in newLevel.VariantCust)
            {
                m_customerVariants.Add(variant);
            }

            foreach (var variant in newLevel.VariantCust)
            {
                SpawnRealCustomers(variant);

            }

        }
        void SetupCustomers()
        {
            m_customerSpawnPostStart = GameObject.Find("CustomerMinOffset").transform;
            m_customerSpawnPostEnd = GameObject.Find("CustomerMaxOffset").transform;
            var level = AssetHelpers.GetAllLevelVariantRegistered();
            var currentLevel = PlayerManager.s_Instance.GetCurrentLevel();

            for (int i = 0; i < level.Length; i++)
            {
                if (level[i].Sequence <= currentLevel.Sequence)
                {
                    if (level[i].VariantCust == null) break;
                    m_customerVariants ??= new();
                    foreach (var variant in level[i].VariantCust)
                    {
                        m_customerVariants.Add(variant);
                    }
                }
            }
            SpawnCustomers();

            for (int i = 0; i < m_maxCustomerRunning; i++)
            {
                CommandDummy();
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

                    newDummy.SpawnDelay += i;
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
            CustomersRunning ??= new();
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
                        CustomersRunning.Add(cust);
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
                CustomersRunning.Add(cust);
            }

            cust.CurrentChair = tempChair;
            cust.transform.position = dummy.transform.position;
            cust.TargetPos = tempChair.transform.position;
            cust.DefaultPos = defaultPos;
            cust.CurrentCoro = cust.StartCoroutine(cust.ToChair());


            OnResetDummy(dummy);



        }
        void SpawnRealCustomers(CustomerController variant)
        {

            // spawn cust by their variant presentage spawned 
            // quota =  maxCustQueued % custRate

            var posVariant = 0;
            var persmax = 0;

            for (int i = 0; i < m_customerVariants.Count; i++)
            {

                persmax += m_customerVariants[i].Variant.OccurrencePercentage;
            }

            posVariant = (int)Math.Round(((float)variant.Variant.OccurrencePercentage / persmax) * 100, MidpointRounding.AwayFromZero);


            var tempCustomers = new List<CustomerController>();

            var quota = (int)Math.Round((float)posVariant / 100 * (RealCustomersQueue.Count + CustomersRunning.Count), MidpointRounding.AwayFromZero);
            for (int j = 0; j < quota; j++)
            {

                var widhtOffset = variant.GetComponent<SpriteRenderer>().bounds.size.x;
                var heightOffset = variant.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                var pos = SetRandomPos(widhtOffset, heightOffset);
                if (variant.Variant.Type == CustomerType.OJOL)
                {

                    if (CustomersRunning != null && CustomersRunning.Count > 1)
                    {
                        break;
                    }


                }
                var newCust = GameObject.Instantiate(variant, pos.start, Quaternion.identity, GameObject.Find("Customer").transform);

                m_playerInput.OnLeftClick += newCust.OnTouch;
                newCust.OnCreateOrder += AddOrder;
                newCust.OnCancelOrder += RemoveOrder;
                newCust.OnSeeOrder += SeeOrder;
                newCust.OnReset += OnResetCustomer;




                tempCustomers.Add(newCust);
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
        void SpawnRealCustomers()
        {

            // spawn cust by their variant presentage spawned 
            // quota =  maxCustQueued % custRate

            var posVariant = new int[m_customerVariants.Count];
            var persmax = 0;
            var tempCustomers = new List<CustomerController>();

            for (int i = 0; i < m_customerVariants.Count; i++)
            {

                persmax += m_customerVariants[i].Variant.OccurrencePercentage;
            }

            foreach (var cus in m_customerVariants)
            {
                if (cus.Variant.Type == CustomerType.OJOL)
                {
                    persmax -= cus.Variant.OccurrencePercentage;
                    var widhtOffset = cus.GetComponent<SpriteRenderer>().bounds.size.x;
                    var heightOffset = cus.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                    var pos = SetRandomPos(widhtOffset, heightOffset);
                    for (int i = 0; i < 2; i++)
                    {
                        var newCust = GameObject.Instantiate(cus, pos.start, Quaternion.identity, GameObject.Find("Customer").transform);

                        m_playerInput.OnLeftClick += newCust.OnTouch;
                        newCust.OnCreateOrder += AddOrder;
                        newCust.OnCancelOrder += RemoveOrder;
                        newCust.OnSeeOrder += SeeOrder;
                        newCust.OnReset += OnResetCustomer;




                        tempCustomers.Add(newCust);
                    }

                }

            }
            for (int i = 0; i < m_customerVariants.Count; i++)
            {

                posVariant[i] = (int)Math.Round(((float)m_customerVariants[i].Variant.OccurrencePercentage / persmax) * 100, MidpointRounding.AwayFromZero);
            }


            for (int i = 0; i < posVariant.Length; i++)
            {
                var quota = (int)Math.Round((float)posVariant[i] / 100 * (m_customerVariants.Count * 2), MidpointRounding.AwayFromZero);
                for (int j = 0; j < quota; j++)
                {

                    var widhtOffset = m_customerVariants[i].GetComponent<SpriteRenderer>().bounds.size.x;
                    var heightOffset = m_customerVariants[i].GetComponent<SpriteRenderer>().bounds.size.y / 2;
                    var pos = SetRandomPos(widhtOffset, heightOffset);
                    if (m_customerVariants[i].Variant.Type == CustomerType.OJOL)
                    {

                        break;


                    }
                    var newCust = GameObject.Instantiate(m_customerVariants[i], pos.start, Quaternion.identity, GameObject.Find("Customer").transform);

                    m_playerInput.OnLeftClick += newCust.OnTouch;
                    newCust.OnCreateOrder += AddOrder;
                    newCust.OnCancelOrder += RemoveOrder;
                    newCust.OnSeeOrder += SeeOrder;
                    newCust.OnReset += OnResetCustomer;




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
        void OnResetCustomer(CustomerController cust)
        {
            CustomersRunning.Remove(cust);
            RealCustomersQueue.Enqueue(cust);
        }
        void OnResetDummy(DummyController dummy)
        {
            DummyRunning.Remove(dummy);

            var widhtOffset = dummy.GetComponent<SpriteRenderer>().bounds.size.x;
            var heightOffset = dummy.GetComponent<SpriteRenderer>().bounds.size.y / 2;
            var pos = SetRandomPos(widhtOffset, heightOffset);

            dummy.transform.position = pos.start;
            dummy.TargetPos = pos.end;
            dummy.Setup();

            DummyCustomersQueue.Enqueue(dummy);
            CommandDummy();
        }
        void CommandDummy()
        {

            if (DummyRunning.Count >= m_maxCustomerRunning) return;



            if (DummyCustomersQueue == null || DummyCustomersQueue.Count == 0) SpawnCustomers();



            var cus = DummyCustomersQueue.Dequeue();
            DummyRunning.Add(cus);

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

        public void SeeOrder(ItemSerializable item, CustomerController cus)
        {
            if (m_selectedTask != null)
            {
                m_selectedTask.CustomerOrder.Customer.BgItemSelected.SetActive(value: false);
            }
            UIManager.s_Instance.ForceHUD(HUDName.ITEM_AVAILABLE);
            PlayerManager.s_Instance.Player.ItemPlayerBehaviour.ItemAvailableHandler.SelectItemInHUD(item);
            foreach (var task in m_taskOrders)
            {
                if (task.CustomerOrder.ItemOrder.Content.Name.Equals(item.Content.Name) && task.CustomerOrder.Customer == cus)
                {
                    m_selectedTask = task;
                    task.CustomerOrder.Customer.BgItemSelected.SetActive(true);
                    return;
                }

            }
        }
        void AddOrder(CustomerController obj, ItemSerializable order)
        {

            var cusOrder = new Order(order, obj);


            var panel = m_taskHUD.transform.GetChild(0).GetComponent<RectTransform>();
            panel.sizeDelta = new Vector2(panel.sizeDelta.x + m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.x, panel.sizeDelta.y);

            var task = Instantiate(m_orderTaskPrefab, m_taskHUD.transform.GetChild(0)).GetComponent<Task>();
            task.transform.GetChild(0).GetComponent<Image>().sprite = order.Content.Image;
            // task.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;

            task.GetComponent<Task>().CustomerOrder = cusOrder;
            m_taskOrders ??= new();
            m_taskOrders.Add(task);

            OnAddOrder?.Invoke(task);

        }
        void RemoveOrder(ItemSerializable menu, CustomerController cus)
        {

            if (m_selectedTask != null)
            {
                if (cus == m_selectedTask.CustomerOrder.Customer) PlayerManager.s_Instance.Player.ItemPlayerBehaviour.ItemAvailableHandler.UnselectItemInHUD();
                if (m_selectedTask.CustomerOrder.Customer == cus) m_selectedTask = null;
            }
            if (CheckOrder(menu, out Order order))
            {
                foreach (var task in m_taskOrders)
                {
                    if (task.CustomerOrder.Customer == order.Customer)
                    {

                        var panel = m_taskHUD.transform.GetChild(0).GetComponent<RectTransform>();
                        panel.sizeDelta = new Vector2(panel.sizeDelta.x - m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.x, panel.sizeDelta.y);
                        m_taskOrders.Remove(task);
                        Destroy(task.gameObject);
                        OnRemoveOrder?.Invoke(task);
                        break;
                    }
                }

            }

        }
        public void GetOrder(Order menu)
        {

            foreach (var task in m_taskOrders)
            {
                if (task.CustomerOrder.Customer == menu.Customer)
                {
                    var panel = m_taskHUD.transform.GetChild(0).GetComponent<RectTransform>();
                    panel.sizeDelta = new Vector2(panel.sizeDelta.x - m_orderTaskPrefab.GetComponent<RectTransform>().sizeDelta.x, panel.sizeDelta.y);
                    m_taskOrders.Remove(task);
                    Destroy(task.gameObject);
                    OnRemoveOrder?.Invoke(task);
                    break;
                }
            }

            menu.Customer.ResetOrder();
            menu.Customer.StopCoroutine(menu.Customer.CurrentCoro);

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
            if (m_selectedTask != null)
            {
                if (m_selectedTask.CustomerOrder.ItemOrder.Content.Name.Equals(menu.Content.Name))
                {

                    order = m_selectedTask.CustomerOrder;
                    m_selectedTask = null;
                    return true;
                }
            }

            if (m_taskOrders == null) return false;
            foreach (var task in m_taskOrders)
            {
                if (task.CustomerOrder.ItemOrder.Content.name.Equals(menu.Content.name))
                {
                    order = task.CustomerOrder;
                    return true;
                }

            }

            return false;
        }
        #endregion
    }
}

