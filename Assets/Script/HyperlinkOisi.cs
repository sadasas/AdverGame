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
    public void LinkAddress()
    {
        Application.OpenURL("https://g.page/oishi-fried-chicken?share");
    }
    public void GoFood()
    {
        Application.OpenURL("https://gofood.co.id/semarang/restaurant/oishi-fried-chicken-jl-raya-muntal-gunungpati-7b93d121-1be2-44bc-bf94-fc549bfd5fa7");
    }
}
