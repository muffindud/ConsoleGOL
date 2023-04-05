using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public void Draw()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Remove()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
