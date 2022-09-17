

using AdverGame.Player;
using Unity.Mathematics;
using UnityEngine;

namespace AdverGame.CameraGame
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController s_Instance;

        Camera camera;
        InputBehaviour m_inputPlayer;
        Transform m_minOffset;
        Transform m_maxOffset;
        private Vector3 stageDimensionEnd;
        private Vector3 stageDimensionStart;
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
            m_minOffset = GameObject.Find("CameraMinOffset").transform;
            m_maxOffset = GameObject.Find("CameraMaxOffset").transform;



        }

        private void FixedUpdate()
        {
            stageDimensionEnd = camera.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
            stageDimensionStart = camera.ScreenToWorldPoint(new Vector3(0, Screen.currentResolution.height, 0));
            if (stageDimensionStart.x <= m_minOffset.position.x)
            {
                var distance = math.abs(m_minOffset.position.x - stageDimensionStart.x);
                var rePos = new Vector3(camera.transform.position.x + distance, camera.transform.position.y, camera.transform.position.z);
                camera.transform.position = rePos;
            }
            if (stageDimensionEnd.x >= m_maxOffset.position.x)
            {
                var distance = math.abs(stageDimensionEnd.x - m_maxOffset.position.x);
                var rePos = new Vector3(camera.transform.position.x - distance, camera.transform.position.y, camera.transform.position.z);
                camera.transform.position = rePos;
            }

        }
        public void SetupCamera(InputBehaviour inputPlayer)
        {
            m_inputPlayer = inputPlayer;
            m_inputPlayer.OnLeftDrag += TrackTouch;
        }
        void TrackTouch(Vector2 pos)
        {

            if (stageDimensionStart.x >= m_minOffset.position.x && stageDimensionEnd.x <= m_maxOffset.position.x)
            {
                var actualPos = new Vector3(camera.transform.position.x + ((-1 * pos.x) * m_smoothTransformPos), camera.transform.position.y, camera.transform.position.z);

                camera.transform.position = actualPos;

            }

        }

    }
}
