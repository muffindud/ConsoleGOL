using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Rule
{
    public int[] cellBirth = {3};
    public int[] cellDeath = {0, 1, 4, 5, 6};

    public bool cellAge = false;
    public int cellMaxAge = 0;

    public Rule(int maxAge = 0)
    {
        cellMaxAge = maxAge;
        cellAge = maxAge > 0;
    }
}
