
using AdverGame.Customer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.CharacterCollection
{
    public class CharacterCollectionManager : MonoBehaviour
    {
        CharacterCollectionHUDHandler m_HUD;
        Transform m_mainCanvas;
        Button m_buttonCharacterCollection;
        [SerializeField] List<ItemCollection> m_items;
        GameObject m_HUDNewCharacterNotif;

        [SerializeField] GameObject m_buttonCharacterCollectionPrefab;
        [SerializeField] GameObject m_HUDCharacterCollectionPrefab;
        [SerializeField] GameObject m_HUDNewCharacterNotifPrefab;
        [SerializeField] Sprite m_rareBG;
        [SerializeField] Sprite m_commonBG;

        private void Start()
        {
            m_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            CustomerManager.s_Instance.OnCustomerChoosed += TrackNewCharacter;
            InitButton();
            InitHUD();
            m_HUD.gameObject.SetActive(false);
        }
        void InitButton()
        {
            m_buttonCharacterCollection = Instantiate(m_buttonCharacterCollectionPrefab, m_mainCanvas).GetComponent<Button>();
            m_buttonCharacterCollection.onClick.AddListener(InitHUD);

        }

        void InitHUD()
        {
            if (m_HUD == null) m_HUD = Instantiate(m_HUDCharacterCollectionPrefab, m_mainCanvas).GetComponent<CharacterCollectionHUDHandler>();
            m_HUD.gameObject.SetActive(true);
        }
        public void TrackNewCharacter(CustomerVariant cust)
        {
            if (IsNewCharacter(cust.Name))
            {
                var bg = cust.Type == CustomerType.RARE ? m_rareBG : m_commonBG;
                InitHUDNewCharacter(bg, cust.Image);
                m_items ??= new();
                var newItem = m_HUD.DisplayItem(cust, bg);
                m_items.Add(newItem);
            }

        }
        bool IsNewCharacter(string name)
        {
            if (m_items == null || m_items.Count == 0)
            {

                return true;
            }
            foreach (var itemOwned in m_items)
            {

                if (itemOwned.Name.text.Equals(name)) return false;
            }
            return true;
        }

        void InitHUDNewCharacter(Sprite bg, Sprite img)
        {
            Time.timeScale = 0f;
            if (m_HUDNewCharacterNotif == null) m_HUDNewCharacterNotif = Instantiate(m_HUDNewCharacterNotifPrefab, m_mainCanvas);
            m_HUDNewCharacterNotif.transform.GetChild(2).GetComponent<Image>().sprite = bg;
            m_HUDNewCharacterNotif.transform.GetChild(3).GetComponent<Image>().sprite = img;
            m_HUDNewCharacterNotif.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Time.timeScale = 1f; });

            m_HUDNewCharacterNotif.SetActive(true);


        }
    }
}
