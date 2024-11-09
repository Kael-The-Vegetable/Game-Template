using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factorial : MonoBehaviour
{
    public int factorialVal;

    public void Awake()
    {
        Debug.Log(CalculateFactorial(factorialVal));

        int sum = 1;

        if (factorialVal < 0)
        { // negative values don't exist for Factorials
            throw new System.ArgumentOutOfRangeException("value cannot be negative.");
        }
        for (int i = 1; i <= factorialVal; i++)
        {
            sum *= i;
        }
        Debug.Log(sum);
    }
    private int CalculateFactorial(int value)
    {
        if (value < 0)
        { // negative values don't exist for Factorials
            throw new System.ArgumentOutOfRangeException("value cannot be negative.");
        }
        if (value < 2)
        { // if value is 0 or 1 then return 1;
            return 1;
        }

        return value * CalculateFactorial(value - 1);
    }
}
