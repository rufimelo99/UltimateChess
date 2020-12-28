using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece
{
    public override bool[,] PossibleMove()
    {
        bool[,] possibleMovesMap = new bool[8, 8];
        ChessPiece enemyPiece;


        //TODO
        //to be optimazed later
        //left side tiles
        if (CurrentX - 1 >= 0)
        {
            if (CurrentZ - 1 >= 0)
            {
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ - 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ - 1] = true;
                }
                else
                {
                    if (isWhite != enemyPiece.isWhite)
                    {
                        possibleMovesMap[CurrentX - 1, CurrentZ - 1] = true;
                    }
                }
            }
            if (CurrentZ + 1 <= 7)
            {
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ + 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ + 1] = true;
                }
                else
                {
                    if (isWhite != enemyPiece.isWhite)
                    {
                        possibleMovesMap[CurrentX - 1, CurrentZ + 1] = true;
                    }
                }
            }
            //to the left
            enemyPiece = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX - 1, CurrentZ] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ] = true;
                }
            }

        }
        //right side tiles
        if (CurrentX + 1 <= 7)
        {
            if (CurrentZ - 1 >= 0)
            {
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ - 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ - 1] = true;
                }
                else
                {
                    if (isWhite != enemyPiece.isWhite)
                    {
                        possibleMovesMap[CurrentX + 1, CurrentZ - 1] = true;
                    }
                }
            }
            if (CurrentZ + 1 <= 7)
            {
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ + 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ + 1] = true;
                }
                else
                {
                    if (isWhite != enemyPiece.isWhite)
                    {
                        possibleMovesMap[CurrentX + 1, CurrentZ + 1] = true;
                    }
                }
            }
            //to the right
            enemyPiece = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX + 1, CurrentZ] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ] = true;
                }
            }

        }
        //up
        if (CurrentZ + 1 <= 7)
        {
            enemyPiece = BoardManager.Instance.chessPieces[CurrentX, CurrentZ + 1];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX, CurrentZ + 1] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX, CurrentZ + 1] = true;
                }
            }
        }
        //down
        if (CurrentZ -1 > 0)
        {
            enemyPiece = BoardManager.Instance.chessPieces[CurrentX, CurrentZ - 1];
            if (enemyPiece == null)
            {
                possibleMovesMap[CurrentX, CurrentZ - 1] = true;
            }
            else
            {
                if (isWhite != enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX, CurrentZ - 1] = true;
                }
            }
        }
        //castling
        if (isWhite)
        {
            if (!BoardManager.Instance.didWhiteKingEverMoved && !BoardManager.Instance.didWhiteRook0EverMoved && BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ] == null && BoardManager.Instance.chessPieces[CurrentX - 2, CurrentZ] == null && BoardManager.Instance.chessPieces[CurrentX - 3, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX - 3, CurrentZ] = true;
            }
            if (!BoardManager.Instance.didWhiteKingEverMoved && !BoardManager.Instance.didWhiteRook1EverMoved && BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ] == null && BoardManager.Instance.chessPieces[CurrentX + 2, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX + 2, CurrentZ] = true;
            }
        }
        else
        {
            if (!BoardManager.Instance.didBlackKingEverMoved && !BoardManager.Instance.didBlackRook0EverMoved && BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ] == null && BoardManager.Instance.chessPieces[CurrentX - 2, CurrentZ] == null && BoardManager.Instance.chessPieces[CurrentX - 3, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX - 3, CurrentZ] = true;
            }
            if (!BoardManager.Instance.didBlackKingEverMoved && !BoardManager.Instance.didBlackRook1EverMoved && BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ] == null && BoardManager.Instance.chessPieces[CurrentX + 2, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX + 2, CurrentZ] = true;
            }

        }

        //TODO check problem
        return possibleMovesMap;
    }
}
