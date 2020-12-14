using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    public override bool[,] PossibleMove() {
        bool[,] possibleMovesMap = new bool[8, 8];
        ChessPiece enemyPiece, enemyPiece2;

        //check en passant moves
        int[] e = BoardManager.Instance.EnPassantMove;
        int[] WhiteKingPosition = BoardManager.Instance.WhiteKingPos;
        int[] BlackKingPosition = BoardManager.Instance.BlackKingPos;
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
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ + 1];
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

                enemyPiece = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ + 1];
                if (enemyPiece != null && !enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ + 1] = true;
                }
            }
            //1 forward
            if(CurrentZ != 7)
            {
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX, CurrentZ + 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX, CurrentZ + 1] = true;
                }
                //2 forward
                //in the begining
                if (CurrentZ == 1)
                {
                    enemyPiece = BoardManager.Instance.chessPieces[CurrentX, CurrentZ + 1];
                    enemyPiece2 = BoardManager.Instance.chessPieces[CurrentX, CurrentZ + 2];
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


                enemyPiece = BoardManager.Instance.chessPieces[CurrentX + 1, CurrentZ - 1];
                if (enemyPiece != null && enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX + 1, CurrentZ - 1] = true;
                }
            }
            //Diagonal left
            if (CurrentX != 7 && CurrentZ != 7)
            {

                //Check if En Passant Move is allowed
                if (e[0] == CurrentX - 1 && e[1] == CurrentZ - 1)
                {
                    possibleMovesMap[CurrentX -  1, CurrentZ - 1] = true;
                }


                enemyPiece = BoardManager.Instance.chessPieces[CurrentX - 1, CurrentZ - 1];
                if (enemyPiece != null && enemyPiece.isWhite)
                {
                    possibleMovesMap[CurrentX - 1, CurrentZ - 1] = true;
                }
            }
            //1 forward
            if (CurrentZ != 0)
            {
                enemyPiece = BoardManager.Instance.chessPieces[CurrentX, CurrentZ - 1];
                if (enemyPiece == null)
                {
                    possibleMovesMap[CurrentX, CurrentZ - 1] = true;
                } 
                //2 forward
                //in the begining
                if (CurrentZ == 6)
                {
                    enemyPiece = BoardManager.Instance.chessPieces[CurrentX, CurrentZ - 1];
                    enemyPiece2 = BoardManager.Instance.chessPieces[CurrentX, CurrentZ - 2];
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
