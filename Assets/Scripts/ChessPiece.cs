using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public int CurrentX  { set; get; }
    public int CurrentZ { set; get; }
    //dont know if ill need it.. depends on the implementation
    public bool isWhite;

    public void setPosition(int x, int z)
    {
        CurrentX = x;
        CurrentZ = z;
    }

    public virtual bool[,] PossibleMove() {
        return new bool[8,8];
    }

}
