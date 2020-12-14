using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BishopPiece : ChessPiece
{
    
    public override bool[,] PossibleMove()
    {
        bool[,] possibleMovesMap = new bool[8, 8];

        ChessPiece enemyPiece;
        int posX;
        int posZ;

        //diagonals
        posX = CurrentX;
        posZ = CurrentZ;
        //upper right diagonal
        while (posX < 7 && posZ < 7)
        {
            posX++;
            posZ++;
            enemyPiece = BoardManager.Instance.chessPieces[posX, posZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[posX, posZ] = true;
            }
            else
            {
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[posX, posZ] = true;
                }
                break;
            }
        }
        posX = CurrentX;
        posZ = CurrentZ;
        //upper left diagonal
        while (posX > 0 && posZ < 7)
        {
            posX--;
            posZ++;
            enemyPiece = BoardManager.Instance.chessPieces[posX, posZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[posX, posZ] = true;
            }
            else
            {
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[posX, posZ] = true;
                }
                break;
            }
        }
        posX = CurrentX;
        posZ = CurrentZ;
        //lower left diagonal
        while (posX > 0 && posZ > 0)
        {
            posX--;
            posZ--;
            enemyPiece = BoardManager.Instance.chessPieces[posX, posZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[posX, posZ] = true;
            }
            else
            {
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[posX, posZ] = true;
                }
                break;
            }
        }
        posX = CurrentX;
        posZ = CurrentZ;
        //lower right diagonal
        while (posX < 7 && posZ > 0)
        {
            posX++;
            posZ--;
            enemyPiece = BoardManager.Instance.chessPieces[posX, posZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[posX, posZ] = true;
            }
            else
            {
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[posX, posZ] = true;
                }
                break;
            }
        }



        return possibleMovesMap;
    }
}