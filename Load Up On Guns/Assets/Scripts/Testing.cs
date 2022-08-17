using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : SingletonMonobehaviour<Testing>
{
    public void TestSingleton()
    {
        print("Veikia");
    }
}
