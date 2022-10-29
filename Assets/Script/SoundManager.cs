
using UnityEngine;

namespace AdverGame.Sound
{
    public enum AudioType
    {
        BGM,
        AMBIENCE
    }
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager s_Instance;
        bool m_isBGMMute;

        [SerializeField] AudioSource m_BGMAudio;
        [SerializeField] AudioClip m_defaultBGM;
        private void Awake()
        {

            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;
        }


        private void Start()
        {
            LoadPlayerSetting();

            m_BGMAudio.clip = m_defaultBGM;
        }

        void LoadPlayerSetting()
        {
            m_isBGMMute = PlayerPrefs.GetInt("BGM") == 1 ? true : false;
        }

        public void PlayBGM(AudioClip clip)
        {
            m_BGMAudio.clip = clip;
            m_BGMAudio.Play();
        }
        public void PlayBGM()
        {

            m_BGMAudio.Play();
        }

        public void StopBGM()
        {

            m_BGMAudio.Pause();
        }
    }
}
