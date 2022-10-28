using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Variant", menuName = "Level Variant")]
public class Level : ScriptableObject
{
    public int MaxChair;
    public int MaxArea;
    public int MaxStove;
    public int MaxExp;
    public int Sequence;

    public override String ToString()
    {
        return $"maksimal kursi : {MaxChair} \n" +
               $"maksimal kompor : {MaxStove} \n" +
               $"maksimal exp : {MaxExp}";

    }
}
