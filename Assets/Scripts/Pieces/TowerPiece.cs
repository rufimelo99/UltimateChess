using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPiece : ChessPiece
{
     /// <summary>
     /// tower can move straigth as much as it can 
     /// the main restrictions are:
     /// The positions of the pieces (must be inside the board)
     /// can not go into a own piece
     /// can not jumo over pieces
     /// </summary>
     /// <returns>bidemensional boolean array with the possible moves</returns>
    public override bool[,] PossibleMove()
    {
        bool[,] possibleMovesMap = new bool[8, 8];
        ChessPiece enemyPiece;
        int amountOfMovesInXDirection;
        //up
        amountOfMovesInXDirection = CurrentZ;
        while (true)
        {
            amountOfMovesInXDirection++;
            if (amountOfMovesInXDirection >= 8)
            {
                break;
            }
            enemyPiece = BoardManager.Instance.chessPieces[CurrentX, amountOfMovesInXDirection];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX, amountOfMovesInXDirection] = true;
            }
            else
            {
                //if it is an enemy piece
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[CurrentX, amountOfMovesInXDirection] = true;
                }
                break;
            }
        }
        //down
        amountOfMovesInXDirection = CurrentZ;
        while (true)
        {
            amountOfMovesInXDirection--;
            if (amountOfMovesInXDirection < 0)
            {
                break;
            }
            enemyPiece = BoardManager.Instance.chessPieces[CurrentX, amountOfMovesInXDirection];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX, amountOfMovesInXDirection] = true;
            }
            else
            {
                //if it is an enemy piece
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[CurrentX, amountOfMovesInXDirection] = true;
                }
                break;
            }
        }
        //right
        amountOfMovesInXDirection = CurrentX;
        while (true)
        {
            amountOfMovesInXDirection++;
            if (amountOfMovesInXDirection >= 8)
            {
                break;
            }
            enemyPiece = BoardManager.Instance.chessPieces[amountOfMovesInXDirection, CurrentZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[amountOfMovesInXDirection,CurrentZ] = true;
            }
            else
            {
                //if it is an enemy piece
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[amountOfMovesInXDirection,CurrentZ] = true;
                }
                break;
            }
        }
        //left
        amountOfMovesInXDirection = CurrentX;
        while (true)
        {
            amountOfMovesInXDirection--;
            if (amountOfMovesInXDirection < 0)
            {
                break;
            }
            enemyPiece = BoardManager.Instance.chessPieces[amountOfMovesInXDirection, CurrentZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[amountOfMovesInXDirection, CurrentZ] = true;
            }
            else
            {
                //if it is an enemy piece
                if (enemyPiece.isWhite != isWhite)
                {
                    possibleMovesMap[amountOfMovesInXDirection, CurrentZ] = true;
                }
                break;
            }
        }
        return possibleMovesMap;
    }
}
