
using UnityEngine;

namespace AdverGame.Sound
{
    public enum BGMType
    {
        INGAME,

        MAINMENU

    }

    public enum AmbienceType
    {
        KITCHEN
    }
    public enum SFXType
    {
        BTNCLICK,
        DUMMYCLICK,
        NEWCHARACTER,
        CUSTOMERANGRY,
        CUSTOMERHAPPY

    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager s_Instance;

        [SerializeField] AudioSource m_BGMAudio;
        [SerializeField] AudioSource m_SFXAudio;
        [SerializeField] AudioSource m_AmbienceAudio;

        [Header("BGM List")]
        [SerializeField] AudioClip m_inGameBGM;
        [SerializeField] AudioClip m_cookingBGM;

        [Header("SFX List")]
        [SerializeField] AudioClip m_btnClickSFX;
        [SerializeField] AudioClip m_dummyClickSFX;
        [SerializeField] AudioClip m_newCharacterSFX;
        [SerializeField] AudioClip m_customerAngrySFX;
        [SerializeField] AudioClip m_customerHappySFX;


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
        public void StopAmbience()
        {
           
            m_AmbienceAudio.Pause();
        }

        public void PlaySFX(SFXType type)
        {
            if (IsSFXMute) return;
            var audioClip = (type) switch
            {
                SFXType.BTNCLICK => m_btnClickSFX,
                SFXType.DUMMYCLICK => m_dummyClickSFX,
                SFXType.NEWCHARACTER => m_newCharacterSFX,
                SFXType.CUSTOMERHAPPY => m_customerHappySFX,
                SFXType.CUSTOMERANGRY => m_customerAngrySFX,
                _ => null
            };

            m_SFXAudio.clip = audioClip;
            m_SFXAudio.Play();
        }

        public void PlayAmbience(AmbienceType type)
        {
            if (IsSFXMute) return;
            var audioClip = (type) switch
            {
                AmbienceType.KITCHEN => m_cookingBGM,
                _ => null
            };
            if (audioClip == m_AmbienceAudio.clip && m_AmbienceAudio.isPlaying) return;
          
            m_AmbienceAudio.clip = audioClip;
            m_AmbienceAudio.Play();
        }

    }
}
