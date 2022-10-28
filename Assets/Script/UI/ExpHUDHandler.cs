using AdverGame.Player;
using System.Collections;
using TMPro;
using UnityEngine;

namespace AdverGame.UI
{
    public class ExpHUDHandler : MonoBehaviour
    {
        Level m_currentLevel;
        [SerializeField] TextMeshProUGUI m_exp;
        [SerializeField] TextMeshProUGUI m_level;
        [SerializeField] TextMeshProUGUI m_increment;
        [SerializeField] GameObject m_newLevelNotif;
        [SerializeField] GameObject m_currentLevelDetail;

        private void Start()
        {
            Setup();
        }
         void Setup()
        {
            var dataPlayer = PlayerManager.s_Instance.Data.Level;
            m_currentLevel = dataPlayer.CurrentLevel;
            m_exp.text = dataPlayer.CurrentExp.ToString();
            m_level.text = dataPlayer.CurrentLevel.Sequence.ToString();
        }
        public void IncreaseXP(int xp, int increment)
        {
            m_exp.text = xp.ToString();

            StartCoroutine(IncrementNotif(increment));
        }
        public void IncreaseLevel(Level newLevel)
        {
            m_currentLevel = newLevel;
            m_level.text = newLevel.Sequence.ToString();
            m_newLevelNotif.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newLevel.ToString();
            m_newLevelNotif.SetActive(true);
            Time.timeScale = 0;

        }

        public void ExitNewLevelNotif()
        {
            Time.timeScale = 1;
        }

        public void OnClick()
        {
            var isActiveNext = !m_currentLevelDetail.activeInHierarchy;
            m_currentLevelDetail.SetActive(isActiveNext);
            if (isActiveNext)
            {
                m_currentLevelDetail.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_currentLevel.ToString();
            }
        }
        IEnumerator IncrementNotif(int increment)
        {

            m_increment.text = "+ " + increment;
            m_increment.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            m_increment.gameObject.SetActive(false);

        }
    }
}
