using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRand {
    uint high;
    uint low;

    void Seed(string seed)
    {
        high = 90210;
        low = 42424242;
        int hshift = 0;
        int lshift = 0;
        bool h = false;
        for (int i = 0; i < seed.Length; i++)
        {
            uint c = seed[i];
            if (h)
            {
                high ^= c << (hshift * 8);
                hshift = (hshift + 1) % sizeof(uint);
            }
            else
            {
                low ^= c << (lshift * 8);
                lshift = (lshift + 1) % sizeof(uint);
            }
            h = !h;
        }
    }

    public GRand(string seed)
    {
        Seed(seed);
    }

    public float value
    {
        get
        {
            high = (high << 16) + (high >> 16);
            high += low;
            low += high;
            return (float)high / uint.MaxValue;
        }
    }

    public float Rangef(float min, float max)
    {
        return (value * (max - min)) + min;
    }
}
