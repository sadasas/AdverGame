
using AdverGame.CameraGame;
using AdverGame.CharacterCollection;
using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Tutorial
{
    public class TutorialHandler : MonoBehaviour
    {
       
        CameraController m_mainCamerea;
        GameObject m_HUDMidleTutorial, m_HUDTutorialTop, m_HUDTutorialEnd;
       
        bool isCustomerWaitOrderClicked;
        bool m_isNext = false;
        CustomerController m_cust;
        DummyController m_currentDummy;
        GameObject m_nextBtn;
        GameObject m_bgBlur;
        [SerializeField] GameObject m_bgBlurPrefab;

        [SerializeField] GameObject m_HUDTutorialMidlePrefab, m_HUDTutorialTopPrefab, m_HUDTutorialEndPrefab;
        private void Start()
        {

            StartCoroutine(PlayTutorial());
        }

        /*     
         * -awal masuk tutorial:
     -tutorial cara narik pelanggan
     -tutorial karakter terbuka
     -teks penjelasan karakter
     -tutorial menerima pesanan
     -teks penjelasan menu yang di pesan
     -tutorial mengerjakan pesanan
     -teks penjelasan pelanggan makan
     -teks penjelasan pelanggan selesai makan(nambah duit sama exp)
     -tutorial menambah kursi(kalo duit udah cukup)
     -tutorial nambah area(kalo exp udah cukup)
     -teks penjelasan area baru terbuka*/


        IEnumerator PlayTutorial()
        {

            yield return StartCoroutine(Setup());

            yield return StartCoroutine(SwipeScreen());

            yield return StartCoroutine(SearchDummy());

            yield return StartCoroutine(ShowCharacterCollection());

            yield return StartCoroutine(OrderCustExplanation());



            yield return StartCoroutine(HowToCook());

            yield return StartCoroutine(LookMenuAvailableAlternatif());
        }

        void EndTutorial()
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            Destroy(m_HUDTutorialEnd.gameObject);
            Destroy(m_HUDTutorialTop.gameObject);
            Destroy(m_HUDMidleTutorial.gameObject);
            var dummys = CustomerManager.s_Instance.DummyRunning;
            foreach (var dummy in dummys)
            {
                dummy.CurrentCoro = dummy.StartCoroutine(dummy.Walking());
            }
            Destroy(this.gameObject);
        }
        IEnumerator Setup()
        {

            var dummys = CustomerManager.s_Instance.DummyRunning;
            foreach (var dummy in dummys)
            {
                dummy.StopCoroutine(dummy.CurrentCoro);
            }
            yield return null;
            m_currentDummy = dummys[0];
            dummys[0].CurrentCoro = dummys[0].StartCoroutine(dummys[0].Walking());

            var cust = CustomerManager.s_Instance.RealCustomersQueue.Peek();
            while (cust.Variant.Type == CustomerType.OJOL)
            {
                CustomerManager.s_Instance.RealCustomersQueue.Dequeue();
                CustomerManager.s_Instance.RealCustomersQueue.Enqueue(cust);
                cust = CustomerManager.s_Instance.RealCustomersQueue.Peek();
            }
            m_HUDMidleTutorial ??= Instantiate(m_HUDTutorialMidlePrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_HUDTutorialTop ??= Instantiate(m_HUDTutorialTopPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_HUDTutorialEnd ??= Instantiate(m_HUDTutorialEndPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_nextBtn = m_HUDMidleTutorial.transform.GetChild(2).gameObject;
            m_bgBlur = Instantiate(m_bgBlurPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_bgBlur.SetActive(false);
            m_HUDTutorialTop.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => Next());
            m_HUDTutorialEnd.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => Next());
            m_HUDMidleTutorial.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => Next());
        }
        IEnumerator SwipeScreen()
        {
            m_HUDMidleTutorial.SetActive(true);
            UIManager.s_Instance.isProhibited = true;
            m_nextBtn.SetActive(true);
           var text = "Selamat Datang, disini player akan bermain sebagai pengusaha ayam yang sedang merintis usaha dari 0";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }

             text = "Sebelum bermain , ikuti tutorial terlebih dahulu";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }
            m_nextBtn.SetActive(false);
            var currentView = CameraController.s_Instance.CurrentView;

             text = "Swipe kanan  untuk menggerakan camera ke kiri";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            while (CameraController.s_Instance.LastDir != CameraMoveDir.LEFT)
            {

                yield return null;
            }

            text = "Swipe kiri  untuk menggerakan camera ke kanan";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            while (CameraController.s_Instance.LastDir != CameraMoveDir.RIGTH)
            {

                yield return null;
            }


        }
        IEnumerator SearchDummy()
        {
            m_mainCamerea ??= CameraController.s_Instance;
            m_HUDMidleTutorial.SetActive(true);
            string text = "klik 2 kali pada dummy karakter untuk menarik pelanggan menuju toko";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            while (CustomerManager.s_Instance.CustomersRunning == null || CustomerManager.s_Instance.CustomersRunning.Count == 0)
            {
                m_currentDummy.transform.position = new Vector2(m_mainCamerea.transform.position.x, m_currentDummy.transform.position.y);
                yield return null;
            }



            while (UIManager.s_Instance.HUDRegistered.ContainsKey(HUDName.NEWCHARACTERNOTIF) && UIManager.s_Instance.HUDRegistered[HUDName.NEWCHARACTERNOTIF].activeInHierarchy)
            {
                yield return null;
            }

            Time.timeScale = 0f;
            m_nextBtn.SetActive(true);
            text = "Dummy karakter akan secara random akan berubah menjadi pelanggan yang bisa dikoleksi";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }


            text = "Pelanggan akan otomatis menuju kursi yang tersedia untuk memesan makanan";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_isNext = false;
            Time.timeScale = 1f;
            var dummys = CustomerManager.s_Instance.DummyRunning;
            foreach (var dummy in dummys)
            {
                dummy.StopCoroutine(dummy.CurrentCoro);
            }
            m_cust = CustomerManager.s_Instance.CustomersRunning[0];
            m_cust.CountDownWaitOrder += Mathf.Infinity;

            while (!m_isNext)
            {

                yield return null;
            }
            m_nextBtn.SetActive(false);

        }
        IEnumerator ShowCharacterCollection()
        {
            UIManager.s_Instance.isProhibited = false;
            m_bgBlur.SetActive(true);
            var isHUDActive = false;
            m_HUDMidleTutorial.transform.SetAsLastSibling();
            string text = "klik tombol character collection di pojok kiri bawah untuk melihat character yang didapatkan";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;

            CharacterCollectionManager.s_Instance.ButtonCharacterCollection.gameObject.transform.SetAsLastSibling();

            while (!isHUDActive)
            {
                isHUDActive = CharacterCollectionManager.s_Instance.HUD.gameObject.activeInHierarchy;
                yield return null;
            }
            m_bgBlur.SetActive(value: false);


            yield return StartCoroutine(LookCharacterCollectionDetail());

            while (isHUDActive)
            {
             
                isHUDActive = CharacterCollectionManager.s_Instance.HUD.gameObject.activeInHierarchy;
                yield return null;
            }

        }
        IEnumerator LookCharacterCollectionDetail()
        {
            m_HUDMidleTutorial.SetActive(false);

            m_HUDTutorialTop.SetActive(true);
            string text = "klik salah satu character  untuk melihat penjelasan character yang didapatkan";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            var obj = ItemCollection.m_HUDItemCollectionDetail;

            while (obj == null || !obj.gameObject.activeInHierarchy)
            {
                m_HUDTutorialTop.transform.SetAsLastSibling();
                obj = ItemCollection.m_HUDItemCollectionDetail;
                yield return null;
            }
            m_HUDTutorialTop.SetActive(false);



            while (obj.gameObject.activeInHierarchy)
            {
                Debug.Log("f");
                yield return null;
            }
        }
        IEnumerator OrderCustExplanation()
        {
            UIManager.s_Instance.isProhibited = true;

          
            CameraController.s_Instance.MoveCamera(2);

            m_HUDTutorialTop.SetActive(false);
            m_HUDTutorialEnd.SetActive(true);
            string text = "Setelah Pelanggan duduk maka akan secara otomatis memesan makanan dan muncul popup notifikasi makanan yang dipesan";
            m_HUDTutorialEnd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_cust.CurrentState = CustomerManager.s_Instance.CustomersRunning[0].CurrentState;
            m_nextBtn = m_HUDTutorialEnd.transform.GetChild(2).gameObject;
            m_nextBtn.SetActive(true);
            m_isNext = false;
            while (m_cust.CurrentState != CustomerState.WAITORDER || !m_isNext)
            {

                m_cust.CurrentState = CustomerManager.s_Instance.CustomersRunning[0].CurrentState;
                yield return null;
            }
            Time.timeScale = 0f;
            m_HUDTutorialEnd.SetActive(false);
            m_HUDMidleTutorial.SetActive(true);
            m_bgBlur.SetActive(true);
            m_bgBlur.transform.SetAsLastSibling();
            m_HUDMidleTutorial.transform.SetAsLastSibling();
            GameObject.FindGameObjectWithTag("TaskHUD").transform.SetAsLastSibling();
            text = "diarea pojok kanan atas juga akan muncul daftar pesanan pelanggan yang harus dimasak";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn = m_HUDMidleTutorial.transform.GetChild(2).gameObject;
            m_nextBtn.SetActive(value: true);
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }

            text = "anda juga bisa menenekan daftar pesanan tersebut untuk melihat customer mana yang memesan";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn.SetActive(true);
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }

            m_nextBtn.SetActive(false);
            yield return StartCoroutine(LookMenuAavailable());
            yield return StartCoroutine(LookCharacterOrdered());
            text = "menu yang diminta pelanggan belum tersedia, mari memasak menu terlebih dahulu";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }
            m_HUDTutorialTop.SetActive(false);

        }
        IEnumerator LookMenuAavailable()
        {
            m_bgBlur.transform.SetAsLastSibling();
            m_HUDMidleTutorial.transform.SetAsLastSibling();
            GameObject.FindGameObjectWithTag("BtnItemAvailable").transform.SetAsLastSibling();
            UIManager.s_Instance.isProhibited = false;
            Time.timeScale = 1f;
            string text = "untuk menyajikan makanan ke pelanggan pertama tama  klik tombol menu tersedia pada pojok bawah kiri untuk melihat menu yang sudah dimasak";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;

            while (!UIManager.s_Instance.HUDRegistered[HUDName.ITEM_AVAILABLE].activeInHierarchy)
            {
                m_cust.CurrentState = CustomerManager.s_Instance.CustomersRunning[0].CurrentState;

                yield return null;
            }

        }
        IEnumerator LookCharacterOrdered()
        {
            m_HUDMidleTutorial.SetActive(false);
            m_HUDTutorialTop.SetActive(true);
            m_nextBtn = m_HUDTutorialTop.transform.GetChild(2).gameObject;
            m_bgBlur.SetActive(false);
            m_HUDTutorialTop.transform.SetAsLastSibling();
            string text = " untuk lebih jelas klik pelanggan yang sedang menunggu pesanan untuk melihat pesanan lebih jelas";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            var cust = CustomerManager.s_Instance.CustomersRunning[0].OnSeeOrder += (order, cust) => { isCustomerWaitOrderClicked = true; };

            while (!isCustomerWaitOrderClicked)
            {
                yield return null;
            }
            isCustomerWaitOrderClicked = false;
            m_HUDTutorialTop.transform.SetAsLastSibling();
            text = "Pesanan yang diminta pelanggan akan terpilih dalam menu tersedia ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn.SetActive(true);
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }


        }
        IEnumerator HowToCook()
        {


            m_HUDMidleTutorial.SetActive(true);
            GameObject.FindGameObjectWithTag("BtnCookItem").transform.SetAsLastSibling();
            string text = "klik Gambar kompor dan panci  untuk  memasak";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;

            while (!UIManager.s_Instance.HUDRegistered[HUDName.COOK_ITEM].activeInHierarchy)
            {

                yield return null;
            }
            m_HUDMidleTutorial.SetActive(false);
            yield return StartCoroutine(ExplainEnvironmentCook());
        }

        IEnumerator ExplainEnvironmentCook()
        {

            m_HUDMidleTutorial.SetActive(false);
            m_HUDTutorialTop.SetActive(true);
            m_nextBtn = m_HUDTutorialTop.transform.GetChild(2).gameObject;
            string text = "terdapat kompor untuk memasak yang akan bertambah seiring kenaikan level exp";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn.SetActive(true);
            m_isNext = false;
            while (!m_isNext)
            {
                m_HUDTutorialTop.transform.SetAsLastSibling();
                yield return null;
            }
            m_isNext = false;
            m_HUDTutorialEnd.transform.SetAsLastSibling();

            text = "terdapat list menu yang bisa dimasak ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn.SetActive(true);
            while (!m_isNext)
            {
                yield return null;
            }
            m_isNext = false;
            text = "untuk memulai memasak pastikan kompor berada dalam posisi kosong  ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            while (!m_isNext)
            {
                yield return null;
            }
            m_isNext = false;
            m_nextBtn.SetActive(value: false);
            text = "pilih dan klik menu yang ingin dimasak sesuai dengan permintaan pelanggan  ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;

            while (PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemBeingCook == 0)
            {
                yield return null;
            }
            Time.timeScale = 1f;
            text = "Tunggu proses masak selesai ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            var currentItemBeingCooking = PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemBeingCook;
            while (currentItemBeingCooking == PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemBeingCook)
            {
                yield return null;
            }
            text = "Setelah makanan jadi, klik item pada kompor untuk menaruh di display agar bisa memasak kembali ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            var itemCooked = PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemCooked;
            while (itemCooked == PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemCooked)
            {

                yield return null;
            }
            m_HUDMidleTutorial.SetActive(true);
            m_HUDTutorialTop.SetActive(false);
            m_HUDMidleTutorial.transform.SetAsLastSibling();
            m_nextBtn = m_HUDMidleTutorial.transform.GetChild(2).gameObject;
            text = "Mari sajikan masakan ke pelanggan, tekan tombol silang untuk kembali ke menu utama";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;

            while (UIManager.s_Instance.HUDRegistered[HUDName.COOK_ITEM].activeInHierarchy)
            {

                yield return null;
            }


        }

        IEnumerator LookMenuAvailableAlternatif()
        {
            m_HUDMidleTutorial.SetActive(false);
            m_HUDTutorialTop.SetActive(true);
            m_nextBtn = m_HUDTutorialTop.transform.GetChild(2).gameObject;
            string text = "anda juga bisa mengklik pelanggan yang sedang duduk untuk membuka menu tersedia sekaligus memilih pesanan pelanggan tersebut";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            var cust = CustomerManager.s_Instance.CustomersRunning[0].OnSeeOrder += (order, cust) => { isCustomerWaitOrderClicked = true; };
            isCustomerWaitOrderClicked = false;
            while (!isCustomerWaitOrderClicked)
            {
                yield return null;
            }

            text = "klik menu yang terseleksi untuk menyajikan masakan ";
            m_HUDTutorialTop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            while (CustomerManager.s_Instance.CustomersRunning[0].CurrentState != CustomerState.EAT)
            {
                yield return null;
            }

            Time.timeScale = 0f;
            m_HUDTutorialTop.SetActive(false);
            m_HUDTutorialEnd.SetActive(true);
            m_HUDTutorialEnd.transform.SetAsLastSibling();
            m_nextBtn = m_HUDTutorialEnd.transform.GetChild(2).gameObject;
            text = "Setelah disajikan pelanggan akan memakan makanan dan membayar pesanan, exp dan koin anda akan bertambah ";
            m_HUDTutorialEnd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn.SetActive(true);
            m_isNext = false;
            while (!m_isNext)
            {

                yield return null;
            }


            m_HUDTutorialEnd.SetActive(false);
            m_HUDMidleTutorial.SetActive(true);

            text = "Koin digunakan untuk menambah meja dan kursi yang tersedia, sehingga menambah pelanggan yang mmemesan ";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            m_nextBtn = m_HUDMidleTutorial.transform.GetChild(2).gameObject;
            m_nextBtn.SetActive(true);
            m_isNext = false;
            while (!m_isNext)
            {
               
                yield return null;
            }
            Time.timeScale = 1f;
            text = "Semalat Bermain";
            m_HUDMidleTutorial.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
            yield return new WaitForSeconds(3f);
            m_HUDMidleTutorial.SetActive(false);
            EndTutorial();
        }





        public void Next()
        {
            m_isNext = true;
        }
    }
}
