
using UnityEngine;

namespace AdverGame.Sound
{
    public enum BGMType
    {
        INGAME,
        AMBIENCE,
        MAINMENU

    }

    public enum SFXType
    {
        BTNCLICK,
        DUMMYCLICK

    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager s_Instance;

        [SerializeField] AudioSource m_BGMAudio;
        [SerializeField] AudioSource m_SFXAudio;

        [Header("BGM List")]
        [SerializeField] AudioClip m_inGameBGM;

        [Header("SFX List")]
        [SerializeField] AudioClip m_btnClickSFX;
        [SerializeField] AudioClip m_dummyClickSFX;


        public bool IsBGMMute;
        public bool IsSFXMute;

        private void Awake()
        {

            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;
        }


        private void Start()
        {
            LoadPlayerSetting();


            if (!IsBGMMute) PlayBGM(BGMType.INGAME);
        }

        void LoadPlayerSetting()
        {
            IsBGMMute = PlayerPrefs.GetInt("BGM") == 1 ? true : false;
            IsSFXMute = PlayerPrefs.GetInt("SFX") == 1 ? true : false;
        }

        public void PlayBGM(BGMType type)
        {
            var audioClip = (type) switch
            {
                BGMType.INGAME => m_inGameBGM,
                _ => null
            };
            m_BGMAudio.clip = audioClip;
            m_BGMAudio.Play();
        }


        public void StopBGM()
        {
            m_BGMAudio.Pause();
        }

        public void PlaySFX(SFXType type)
        {
            if (IsSFXMute) return;
            var audioClip = (type) switch
            {
                SFXType.BTNCLICK => m_btnClickSFX,
                SFXType.DUMMYCLICK => m_dummyClickSFX,
                _ => null
            };

            m_SFXAudio.clip = audioClip;
            m_SFXAudio.Play();
        }
    }
}
