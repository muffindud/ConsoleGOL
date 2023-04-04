using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cell : MonoBehaviour
{
    public bool isAlive = false;
    public int neighbors = 0;
    public int age = 0;

    public Rule activeRule = null;

    public void UpdateCell()
    {
        if (activeRule.cellAge && age == activeRule.cellMaxAge)
        {
            isAlive = false;
        }
        else if (activeRule.cellBirth.Contains(neighbors) && !isAlive)
        {
            isAlive = true;
            age = 0;
        }
        else if (activeRule.cellDeath.Contains(neighbors) && isAlive)
        {
            isAlive = false;
        }
        else if (isAlive)
        {
            age++;
        }
    }

    public void Draw()
    {
        GetComponent<SpriteRenderer>().enabled = isAlive;
    }
}
