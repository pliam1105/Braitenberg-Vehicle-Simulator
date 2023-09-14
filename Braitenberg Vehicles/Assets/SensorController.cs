using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorController : MonoBehaviour
{
    public Function function;
    public int type = 0;
    public int connection = -1;
    public string functionType = "inc";
    public float functionSlopeUp = 1;
    public float functionSlopeDown = -1;
    public float functionX0 = 0;
    public float functionY0 = 0;
    // Start is called before the first frame update
    void Start()
    {
        function = new Function(functionType, functionSlopeUp, functionSlopeDown, functionX0, functionY0);
    }
}

public class Function
{
    private const float maxx = 10000f;
    private string type;
    private float slope_up, slope_down, x0, y0;
    public Function(string t, float sup, float sdown, float opx, float opy)
    {
        type = t;
        slope_up = sup;
        slope_down = sdown;
        x0 = opx*maxx;
        y0 = opy*maxx;
    }

    public float evaluate(float x)
    {
        if (type == "inc")
        {
            return y0 + slope_up * (x - x0);
        }
        else if (type == "dec")
        {
            x = Math.Min(x, maxx);
            return y0 + slope_down * (x - x0);
        }
        else if (type == "inc_dec")
        {
            x = Math.Min(x, maxx);
            if (x <= x0)
            {
                return y0 + slope_up * (x - x0);
            }
            else
            {
                return y0 + slope_down * (x - x0);
            }
        }
        else if (type == "dec_inc")
        {
            if (x > x0)
            {
                return y0 + slope_up * (x - x0);
            }
            else
            {
                return y0 + slope_down * (x - x0);
            }
        }
        else
        {
            throw new Exception("Invalid Function Type");
        }
    }
}
