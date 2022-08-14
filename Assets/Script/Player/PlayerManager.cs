using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager s_Instance;

    InputBehaviour m_inputPlayer;

    [Header("INPUT BEHAVIOUR SETTING")]
    [SerializeField] LayerMask m_customerMask;


    private void Awake()
    {
        if (s_Instance != null) Destroy(s_Instance.gameObject);
        s_Instance = this;
    }

    private void Start()
    {
        m_inputPlayer = new(m_customerMask);
    }

    private void Update()
    {
        m_inputPlayer.Update();
    }
}
