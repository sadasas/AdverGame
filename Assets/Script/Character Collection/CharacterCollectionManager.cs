
using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.Utility;
using System.Collections.Generic;
using System.Linq;
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

            LoadCharacterCollection();
        }
        void LoadCharacterCollection()
        {
            var data = PlayerManager.s_Instance.Data.CharacterCollection;
            var customerVariantRegistered = AssetHelpers.GetAllCustomerVariantsRegistered().ToList();
            if (data != null && data.Count != 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    foreach (var variant in customerVariantRegistered.ToArray())
                    {
                        if (variant.Name.Equals(data[i]))
                        {
                            var bg = variant.Type == CustomerType.RARE ? m_rareBG : m_commonBG;

                            m_items ??= new();
                            var newItem = m_HUD.DisplayItem(variant, bg);
                            m_items.Add(newItem);
                            customerVariantRegistered.Remove(variant);
                            break;
                        }
                    }
                }
            }



            foreach (var variant in customerVariantRegistered)
            {
                var bg = variant.Type == CustomerType.RARE ? m_rareBG : m_commonBG;
                var black = new Color32(0, 0, 0, 255);
                var newItem = m_HUD.DisplayItem(variant, bg, black);
            }

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
                InitHUDNewCharacterNotif(bg, cust.Image);
                m_items ??= new();
                var newItem = m_HUD.DisplayItem(cust, bg);
                m_items.Add(newItem);
                PlayerManager.s_Instance.SaveCharacterCollected(cust.Name);
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

        void InitHUDNewCharacterNotif(Sprite bg, Sprite img)
        {
            Time.timeScale = 0f;
            if (m_HUDNewCharacterNotif == null) m_HUDNewCharacterNotif = Instantiate(m_HUDNewCharacterNotifPrefab, m_mainCanvas);
            m_HUDNewCharacterNotif.transform.GetChild(2).GetComponent<Image>().sprite = bg;
            m_HUDNewCharacterNotif.transform.GetChild(3).GetComponent<Image>().sprite = img;
            m_HUDNewCharacterNotif.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { Time.timeScale = 1f; });

            m_HUDNewCharacterNotif.SetActive(true);


        }
    }
}
