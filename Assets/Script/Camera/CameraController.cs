

using AdverGame.Player;
using UnityEngine;

namespace AdverGame.CameraGame
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController s_Instance;

        Camera camera;
        int m_currentView = 2;
        InputBehaviour m_inputPlayer;
        Transform[] m_cameraViews;

        [Range(0, 0.01f)]
        [SerializeField] float m_smoothTransformPos;

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


        public void SetupCamera(InputBehaviour inputPlayer)
        {
            m_inputPlayer = inputPlayer;
            m_inputPlayer.OnLeftEndDrag += TrackTouch;
        }
        void TrackTouch(Vector2 pos)
        {
            var dir = pos.normalized.x;

            if (dir >= 0 && m_currentView - 1 >= 0)
            {
                camera.transform.position = m_cameraViews[m_currentView - 1].position;
                m_currentView--;
            }
            else if (dir < 0 && m_currentView + 1 < m_cameraViews.Length)
            {
                camera.transform.position = m_cameraViews[m_currentView + 1].position;
                m_currentView++;
            }





        }

    }
}
