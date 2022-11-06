using AdverGame.Player;
using System.Collections;
using System.Text;
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
            m_exp.text = "Current exp :\n" + dataPlayer.CurrentExp.ToString() + " / " + m_currentLevel.MaxExp;
            m_level.text = dataPlayer.CurrentLevel.Sequence.ToString();
        }
        public void IncreaseXP(int xp, int increment)
        {
            m_exp.text = "Current exp : " + xp.ToString() + " / " + m_currentLevel.MaxExp;

            StartCoroutine(IncrementNotif(increment));
        }
        public void IncreaseLevel(Level newLevel)
        {
            StringBuilder newFeature = new();

            m_level.text = newLevel.Sequence.ToString();
            if (newLevel.MaxStove - m_currentLevel.MaxStove != 0) newFeature.Append("Kompor + " + (newLevel.MaxStove - m_currentLevel.MaxStove).ToString());
            if (newLevel.MaxArea - m_currentLevel.MaxArea != 0) newFeature.Append("\n Area Terbuka + " + (newLevel.MaxArea - m_currentLevel.MaxArea).ToString());
            m_newLevelNotif.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level " + newLevel.Sequence.ToString();
            m_newLevelNotif.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = newFeature.ToString();
            m_newLevelNotif.SetActive(true);
            m_currentLevel = newLevel;
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
