
using AdverGame.CameraGame;
using AdverGame.CharacterCollection;
using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.UI;
using System.Collections;
using TMPro;
using UnityEngine;

namespace AdverGame.Tutorial
{
    public class TutorialHandler : MonoBehaviour
    {
        TMP_Text m_textComponent;
        CameraController m_mainCamerea;
        GameObject m_HUDTutorial;
        bool isClicked;
        bool isCustomerWaitOrderClicked;

        CustomerController m_cust;
        [SerializeField] GameObject m_HUDTutorialPrefab;
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
            PlayerManager.s_Instance.Player.InputBehaviour.OnLeftClick += (obj) => { isClicked = true; };
            var dummys = CustomerManager.s_Instance.DummyRunning;
            foreach (var dummy in dummys)
            {
                dummy.CurrentCoro = dummy.StartCoroutine(dummy.Walking());
            }
        }
        IEnumerator Setup()
        {
            PlayerManager.s_Instance.Player.InputBehaviour.OnLeftClick += (obj) => { isClicked = true; };
            var dummys = CustomerManager.s_Instance.DummyRunning;
            foreach (var dummy in dummys)
            {
                dummy.StopCoroutine(dummy.CurrentCoro);
            }
            yield return null;
            dummys[0].CurrentCoro = dummys[0].StartCoroutine(dummys[0].Walking());

            var cust = CustomerManager.s_Instance.RealCustomersQueue.Peek();
            while (cust.Variant.Type == CustomerType.OJOL)
            {
                CustomerManager.s_Instance.RealCustomersQueue.Dequeue();
                CustomerManager.s_Instance.RealCustomersQueue.Enqueue(cust);
                cust = CustomerManager.s_Instance.RealCustomersQueue.Peek();
            }
        }
        IEnumerator ExplainEnvironmentCook()
        {
            m_HUDTutorial.transform.SetAsLastSibling();
            m_HUDTutorial.SetActive(true);
            string text = "terdapat kompor untuk memasak yang akan bertambah seiring kenaikan level exp";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (!isClicked)
            {
                yield return null;
            }
            isClicked = false;
            text = "terdapat list menu yang bisa dimasak ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (!isClicked)
            {
                yield return null;
            }
            isClicked = false;
            text = "untuk memulai memasak pastikan kompor berada dalam posisi kosong  ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (!isClicked)
            {
                yield return null;
            }
            isClicked = false;
            text = "pilih dan klik menu yang ingin dimasak sesuai dengan permintaan pelanggan  ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

            while (PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemBeingCook == 0)
            {
                yield return null;
            }
            Time.timeScale = 1f;
            text = "Tunggu proses masak selesai ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            var currentItemBeingCooking = PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemBeingCook;
            while (currentItemBeingCooking == PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemBeingCook)
            {
                yield return null;
            }
            text = "klik item pada kompor untuk menaruh di display agar bisa memasak kembali ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            var itemCooked = PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemCooked;
            while (itemCooked == PlayerManager.s_Instance.Player.ItemPlayerBehaviour.CookItemHandler.ItemCooked)
            {

                yield return null;
            }

            text = "Mari sajikan masakan ke pelanggan, tekan tombol silang untuk kembali ke menu utama";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

            while (UIManager.s_Instance.HUDRegistered[HUDName.COOK_ITEM].activeInHierarchy)
            {

                yield return null;
            }

            m_HUDTutorial.SetActive(false);
        }
        IEnumerator LookMenuAavailable()
        {
            m_HUDTutorial.SetActive(true);
            string text = "klik tombol menu tersedia pada pojok bawah kiri untuk melihat menu yang sudah dimasak";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

            while (!UIManager.s_Instance.HUDRegistered[HUDName.ITEM_AVAILABLE].activeInHierarchy)
            {
                m_cust.CurrentState = CustomerManager.s_Instance.CustomersRunning[0].CurrentState;
                if (m_cust.CurrentState == CustomerState.WAITORDER) Time.timeScale = 0f;
                yield return null;
            }
            m_HUDTutorial.SetActive(false);
        }

        IEnumerator LookMenuAvailableAlternatif()
        {
            m_HUDTutorial.SetActive(true);
            string text = "anda juga bisa mengklik pelanggan yang sedang duduk untuk membuka menu tersedia sekaligus memilih pesanan pelanggan tersebut";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            var cust = CustomerManager.s_Instance.CustomersRunning[0].OnSeeOrder += (order, cust) => { isCustomerWaitOrderClicked = true; };
            isCustomerWaitOrderClicked = false;
            while (!isCustomerWaitOrderClicked)
            {
                yield return null;
            }

            text = "klik menu yang terseleksi untuk menyajikan masakan ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (CustomerManager.s_Instance.CustomersRunning[0].CurrentState != CustomerState.EAT)
            {
                yield return null;
            }
            text = "Setelah disajikan pelanggan akan memakan makanan dan membayar pesanan ";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            isClicked = false;
            while (!isClicked)
            {

                yield return null;
            }

            isClicked = false;
            m_HUDTutorial.SetActive(false);
            EndTutorial();
        }
        IEnumerator HowToCook()
        {

            m_HUDTutorial.SetActive(true);

            string text = "klik Gambar koki dikontainer untuk memasak";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

            while (!UIManager.s_Instance.HUDRegistered[HUDName.COOK_ITEM].activeInHierarchy)
            {

                yield return null;
            }
            m_HUDTutorial.SetActive(false);
            yield return StartCoroutine(ExplainEnvironmentCook());
        }

        IEnumerator LookCharacterOrdered()
        {
            m_HUDTutorial.SetActive(true);
            string text = "klik pelanggan yang sedang menunggu pesanan untuk melihat pesanan lebih jelas";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            var cust = CustomerManager.s_Instance.CustomersRunning[0].OnSeeOrder += (order, cust) => { isCustomerWaitOrderClicked = true; };

            while (!isCustomerWaitOrderClicked)
            {
                yield return null;
            }
            isCustomerWaitOrderClicked = false;
            m_HUDTutorial.transform.SetAsLastSibling();
            text = "Pesanan yang diminta pelanggan akan terpilih dalam menu tersedia";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            isClicked = false;
            while (!isClicked)
            {

                yield return null;
            }


        }
        IEnumerator OrderCustExplanation()
        {
            m_cust = CustomerManager.s_Instance.CustomersRunning[0];
            m_cust.CountDownWaitOrder += Mathf.Infinity;
            CameraController.s_Instance.MoveCamera(2);

            m_HUDTutorial.SetActive(true);
            string text = "Setelah Pelanggan duduk maka akan secara otomatis memesan makanan dan muncul popup notifikasi makanan yang dipesan";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            m_cust.CurrentState = CustomerManager.s_Instance.CustomersRunning[0].CurrentState;
            while (m_cust.CurrentState != CustomerState.WAITORDER)
            {
                m_cust.CurrentState = CustomerManager.s_Instance.CustomersRunning[0].CurrentState;
                yield return null;
            }
            Time.timeScale = 0f;
            text = "diarea pojok kanan atas juga akan muncul daftar pesanan pelanggan yang harus dimasak";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            isClicked = false;
            while (!isClicked)
            {

                yield return null;
            }


            yield return StartCoroutine(LookMenuAavailable());
            yield return StartCoroutine(LookCharacterOrdered());
            text = "menu yang diminta pelanggan belum tersedia, mari memasak menu terlebih dahulu";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            isClicked = false;
            while (!isClicked)
            {

                yield return null;
            }


            m_HUDTutorial.SetActive(false);
        }

        IEnumerator LookCharacterCollectionDetail()
        {
            m_HUDTutorial.SetActive(true);
            string text = "klik salah satu character  untuk melihat penjelasan character yang didapatkan";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            var obj = ItemCollection.m_HUDItemCollectionDetail;

            while (obj == null || !obj.gameObject.activeInHierarchy)
            {
                obj = ItemCollection.m_HUDItemCollectionDetail;
                yield return null;
            }
            m_HUDTutorial.SetActive(false);



            while (obj.gameObject.activeInHierarchy)
            {

                yield return null;
            }
        }

        IEnumerator ShowCharacterCollection()
        {


            var isHUDActive = false;
            m_HUDTutorial.SetActive(true);
            string text = "klik tombol character collection di pojok kiri bawah untuk melihat character yang didapatkan";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (!isHUDActive)
            {
                isHUDActive = CharacterCollectionManager.s_Instance.m_HUD.gameObject.activeInHierarchy;
                yield return null;
            }
            m_HUDTutorial.SetActive(false);

            yield return StartCoroutine(LookCharacterCollectionDetail());

            while (isHUDActive)
            {
                isHUDActive = CharacterCollectionManager.s_Instance.m_HUD.gameObject.activeInHierarchy;
                yield return null;
            }
            Time.timeScale = 1f;
        }

        IEnumerator SearchDummy()
        {
            m_mainCamerea ??= CameraController.s_Instance;
            m_HUDTutorial.SetActive(true);
            string text = "klik 2 kali pada dummy karakter untuk menarik pelanggan menuju toko";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

            while (CustomerManager.s_Instance.CustomersRunning == null || CustomerManager.s_Instance.CustomersRunning.Count == 0)
            {
                yield return null;
            }

            m_HUDTutorial.SetActive(false);

            while (UIManager.s_Instance.HUDRegistered.ContainsKey(HUDName.NEWCHARACTERNOTIF) && UIManager.s_Instance.HUDRegistered[HUDName.NEWCHARACTERNOTIF].activeInHierarchy)
            {
                yield return null;
            }
            m_HUDTutorial.SetActive(true);
            Time.timeScale = 0f;
            text = "Dummy karakter akan secara random akan berubah menjadi pelanggan yang bisa dikoleksi";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            isClicked = false;
            while (!isClicked)
            {

                yield return null;
            }
            isClicked = false;
            text = "Pelanggan akan otomatis menuju kursi yang tersedia untuk memesan makanan";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            isClicked = false;
            while (!isClicked)
            {

                yield return null;
            }
            isClicked = false;
            m_HUDTutorial.SetActive(false);
        }

        IEnumerator SwipeScreen()
        {

            m_HUDTutorial ??= Instantiate(m_HUDTutorialPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform);
            m_HUDTutorial.SetActive(true);
            var currentView = CameraController.s_Instance.CurrentView;

            string text = "Swipe kanan  untuk menggerakan camera ke kiri";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (CameraController.s_Instance.LastDir != CameraMoveDir.LEFT)
            {

                yield return null;
            }

            text = "Swipe kiri  untuk menggerakan camera ke kanan";
            m_HUDTutorial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            while (CameraController.s_Instance.LastDir != CameraMoveDir.RIGTH)
            {

                yield return null;
            }

            m_HUDTutorial.SetActive(false);
        }
    }
}
