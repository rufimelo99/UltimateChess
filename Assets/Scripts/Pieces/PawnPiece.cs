using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    /// <summary>
    /// pawns can move 1unit in front or 2 if never moved
    /// can also eat 1 unit in diagonal to the front and apply the en passant
    /// the main restrictions are:
    /// The positions of the pieces (must be inside the board)
    /// can not go into a own piece
    /// </summary>
    /// <returns>bidemensional boolean array with the possible moves</returns>
    public override bool[,] PossibleMove(BoardManager instance) {
        bool[,] possibleMovesMap = new bool[8, 8];
        ChessPiece enemyPiece, enemyPiece2;
        //check en passant moves
        int[] e = instance.EnPassantMove;
        //White Turn
        if (isWhite)
        {
            //Diagonals
            //Diagonal left
            if (CurrentX != 0 && CurrentZ != 7)
            {
                //CHeck if En Passant Move is allowed
                if (e[0] == CurrentX - 1 && e[1] == CurrentZ + 1) 
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ + 1] = true;
                }
                enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ + 1];
                if (enemyPiece != null && !enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ + 1] = true;
                }
            }
            //Diagonal right
            if (CurrentX != 7 && CurrentZ != 7)
            {
                //CHeck if En Passant Move is allowed
                if (e[0] == CurrentX + 1 && e[1] == CurrentZ + 1)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ + 1] = true;
                }
                enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ + 1];
                if (enemyPiece != null && !enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ + 1] = true;
                }
            }
            //1 forward
            if(CurrentZ != 7)
            {
                enemyPiece = instance.chessPieces[CurrentX, CurrentZ + 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX, CurrentZ + 1] = true;
                }
                //2 forward
                //in the begining
                if (CurrentZ == 1)
                {
                    enemyPiece = instance.chessPieces[CurrentX, CurrentZ + 1];
                    enemyPiece2 = instance.chessPieces[CurrentX, CurrentZ + 2];
                    if (enemyPiece == null && enemyPiece2 == null)
                    {
                        possibleMovesMap[CurrentX, CurrentZ + 2] = true;
                    }
                }
            }
        }
        //Black Turn
        else
        {
            //Diagonals
            //Diagonal right
            if (CurrentX != 7 && CurrentZ != 0)
            {
                //CHeck if En Passant Move is allowed
                if (e[0] == CurrentX + 1 && e[1] == CurrentZ - 1)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ - 1] = true;
                }
                enemyPiece = instance.chessPieces[CurrentX + 1, CurrentZ - 1];
                if (enemyPiece != null && enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ - 1] = true;
                }
            }
            //Diagonal left
            if (CurrentX != 0 && CurrentZ != 7)
            {
                //Check if En Passant Move is allowed
                if (e[0] == CurrentX - 1 && e[1] == CurrentZ - 1)
                {
                    possibleMovesMap[CurrentX -  1, CurrentZ - 1] = true;
                }
                enemyPiece = instance.chessPieces[CurrentX - 1, CurrentZ - 1];
                if (enemyPiece != null && enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ - 1] = true;
                }
            }
            //1 forward
            if (CurrentZ != 0)
            {
                enemyPiece = instance.chessPieces[CurrentX, CurrentZ - 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX, CurrentZ - 1] = true;
                } 
                //2 forward
                //in the begining
                if (CurrentZ == 6)
                {
                    enemyPiece = instance.chessPieces[CurrentX, CurrentZ - 1];
                    enemyPiece2 = instance.chessPieces[CurrentX, CurrentZ - 2];
                    if (enemyPiece == null && enemyPiece2 == null)
                    {
                        possibleMovesMap[CurrentX, CurrentZ - 2] = true;
                    }
                }
            }
        }
        return possibleMovesMap;
    }
}
