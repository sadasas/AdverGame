using UnityEngine;

public class HyperlinkOisi : MonoBehaviour
{
    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }
    public void LinkIG()
    {
        Application.OpenURL("https://instagram.com/oishi.friedchicken?igshid=YmMyMTA2M2Y=");
    }
}
