using UnityEngine;

public class Student : Customer, ICustomer
{



    
    public void Move()
    {

    }

    public void OnTouch()
    {
        Debug.Log("Touched");
    }
}
