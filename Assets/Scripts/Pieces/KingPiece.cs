using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece
{
    /// <summary>
    /// kings can move 1 unit in each direction
    /// the main restrictions are:
    /// The positions of the pieces (must be inside the board)
    /// can not go into a own piece
    /// </summary>
    /// <returns>bidemensional boolean array with the possible moves</returns>
    public override bool[,] PossibleMove(BoardManager instance)
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
                enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ - 1];
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
                enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ + 1];
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
            enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ];
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
                enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ - 1];
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
                enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ + 1];
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
            enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ];
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
            enemyPiece = instance.chessPieces[CurrentX, CurrentZ + 1];
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
            enemyPiece = instance.chessPieces[CurrentX, CurrentZ - 1];
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
            if (!instance.didWhiteKingEverMoved && !instance.didWhiteRook0EverMoved && instance.chessPieces[CurrentX - 1, CurrentZ] == null && instance.chessPieces[CurrentX - 2, CurrentZ] == null && instance.chessPieces[CurrentX - 3, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX - 3, CurrentZ] = true;
            }
            if (!instance.didWhiteKingEverMoved && !instance.didWhiteRook1EverMoved && instance.chessPieces[CurrentX + 1, CurrentZ] == null && instance.chessPieces[CurrentX + 2, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX + 2, CurrentZ] = true;
            }
        }
        else
        {
            if (!instance.didBlackKingEverMoved && !instance.didBlackRook0EverMoved && instance.chessPieces[CurrentX - 1, CurrentZ] == null && instance.chessPieces[CurrentX - 2, CurrentZ] == null && instance.chessPieces[CurrentX - 3, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX - 3, CurrentZ] = true;
            }
            if (!instance.didBlackKingEverMoved && !instance.didBlackRook1EverMoved && instance.chessPieces[CurrentX + 1, CurrentZ] == null && instance.chessPieces[CurrentX + 2, CurrentZ] == null)
            {
                possibleMovesMap[CurrentX + 2, CurrentZ] = true;
            }
        }
        //TODO check problem
        return possibleMovesMap;
    }
}
