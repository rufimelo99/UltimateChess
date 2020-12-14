using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public string playerName { set; get; }
 
    public bool isWhite { set; get; }

    public bool isBot { set; get; }

}
