using UnityEngine;


public interface ICustomer
{
    void Move();
    void OnTouch();

}

public abstract class Customer : MonoBehaviour
{
    [SerializeField] int Xpos;
    private void Start()
    {
        
    }
    void Move()
    {

    }

    Vector2 SetRandomPos()
    {
        return new Vector2(Xpos, Random.Range(10, 10));
    }
}
