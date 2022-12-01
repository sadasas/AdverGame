

using AdverGame.Player;
using UnityEngine;

namespace AdverGame.CameraGame
{
    public enum CameraMoveDir
    {
        DEFAULT,
        LEFT,
        RIGTH
    }
    public class CameraController : MonoBehaviour
    {
        public static CameraController s_Instance;

        Camera camera;
        InputBehaviour m_inputPlayer;
        Transform[] m_cameraViews;


        [SerializeField] float m_minLengthSwipe;

        public bool isProhibited = false;
        
        public int CurrentView = 2;
        public CameraMoveDir LastDir = CameraMoveDir.DEFAULT;

        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;
        }
        private void Start()
        {
            camera = GetComponent<Camera>();
            m_cameraViews = new Transform[3];
            for (int i = 1; i <= 3; i++)
            {
                m_cameraViews[i - 1] = GameObject.Find("CameraView" + i).transform;
            }


        }

        public void MoveCamera(int index)
        {

            camera.transform.position = m_cameraViews[index - 1].position;
            CurrentView = index == 0 ? CurrentView - 1 : CurrentView + 1;
        }

        public void SetupCamera(InputBehaviour inputPlayer)
        {
            m_inputPlayer = inputPlayer;
            m_inputPlayer.OnLeftEndDrag += TrackTouch;
        }
        void TrackTouch(float length)
        {
            if (isProhibited) return;
            if (Mathf.Abs(length) < m_minLengthSwipe) return;


            if (length >= 0 && CurrentView - 1 >= 0)
            {
                camera.transform.position = m_cameraViews[CurrentView - 1].position;
                CurrentView--;
                LastDir = CameraMoveDir.LEFT;
            }
            else if (length < 0 && CurrentView + 1 < m_cameraViews.Length)
            {
                camera.transform.position = m_cameraViews[CurrentView + 1].position;
                CurrentView++;
                LastDir = CameraMoveDir.RIGTH;
            }



        }

    }
}
