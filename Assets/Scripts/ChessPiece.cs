using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    /// <summary>
    /// Positions of the piece
    /// </summary>
    public int CurrentX  { set; get; }
    public int CurrentZ { set; get; }
    /// <summary>
    /// boolean that reporesent the color of the piece
    /// </summary>
    public bool isWhite;
    /// <summary>
    /// Function that changes position of the piece
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="z">z position</param>
    public void setPosition(int x, int z)
    {
        CurrentX = x;
        CurrentZ = z;
    }
    /// <summary>
    /// Function that returns a bidemensional boolean array with the possible moves
    /// </summary>
    public virtual bool[,] PossibleMove() {
        return new bool[8,8];
    }

}
