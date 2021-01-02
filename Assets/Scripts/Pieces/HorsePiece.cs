using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorsePiece : ChessPiece
{
    /// <summary>
    /// horses can move in L shape moves
    /// the main restrictions are:
    /// The positions of the pieces (must be inside the board)
    /// can not go into a own piece
    /// </summary>
    /// <returns>bidemensional boolean array with the possible moves</returns>
    public override bool[,] PossibleMove(BoardManager instance)
    {
        bool[,] possibleMovesMap = new bool[8, 8];
        ChessPiece enemyPiece;
        //diagonal leftup+left
        if (CurrentX - 2 >= 0 && CurrentZ + 1 <= 7)
        {
            enemyPiece = instance.chessPieces[CurrentX - 2, CurrentZ + 1];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX - 2, CurrentZ + 1] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 2, CurrentZ + 1] = true;
                }
            }
        }
        //diagonal leftup+up
        if (CurrentX - 1 >= 0 && CurrentZ + 2 <= 7)
        {
            enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ + 2];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX - 1, CurrentZ + 2] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ + 2] = true;
                }
            }
        }
        //diagonal leftdown+left
        if (CurrentX - 2 >= 0 && CurrentZ - 1 >= 0)
        {
            enemyPiece = instance.chessPieces[CurrentX - 2, CurrentZ - 1];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX - 2, CurrentZ - 1] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 2, CurrentZ - 1] = true;
                }
            }
        }
        //diagonal leftdown+down
        if (CurrentX - 1 >= 0 && CurrentZ - 2 >= 0)
        {
            enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ - 2];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX - 1, CurrentZ - 2] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ - 2] = true;
                }
            }
        }
        //diagonal rightup+right
        if (CurrentX + 2 <= 7 && CurrentZ + 1 <= 7)
        {
            enemyPiece = instance.chessPieces[CurrentX + 2, CurrentZ + 1];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX + 2, CurrentZ + 1] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 2, CurrentZ + 1] = true;
                }
            }
        }
        //diagonal rightup+up
        if (CurrentX + 1 <= 7 && CurrentZ + 2 <= 7)
        {
            enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ + 2];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX + 1, CurrentZ + 2] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ + 2] = true;
                }
            }
        }
        //diagonal rightdown+right
        if (CurrentX + 2 <= 7 && CurrentZ - 1 > 0)
        {
            enemyPiece = instance.chessPieces[CurrentX + 2, CurrentZ - 1];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX + 2, CurrentZ - 1] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 2, CurrentZ - 1] = true;
                }
            }
        }
        //diagonal rightdown+down
        if (CurrentX + 1 <= 7 && CurrentZ - 2 > 0)
        {
            enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ - 2];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX + 1, CurrentZ - 2] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ - 2] = true;
                }
            }
        }
        return possibleMovesMap;
    }
}
