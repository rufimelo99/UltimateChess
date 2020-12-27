using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class ChessPlayer : MonoBehaviour
{
    public bool isWhitePlayer;

    public List<ChessPiece> OwnPieces { set; get; } 

    void Start()
    {
    }
    public void UpdateOwnPieces()
    {
        OwnPieces = new List<ChessPiece>() ;
        ChessPiece[,] allPieces = BoardManager.Instance.chessPieces;
        foreach (ChessPiece cp in allPieces)
        {
            if (cp != null)
            {
                if(cp.isWhite == isWhitePlayer)
                {
                    OwnPieces.Add(cp);
                }
            }
        }
        
    }
    public void chooseRandomMove()
    {
        //TODO
        //dont think the castling and the en passant are being read here
        if (BoardManager.Instance.isWhiteTurn == isWhitePlayer)
        {
            bool actionExecuted = false;
            while (actionExecuted != true)
            {
                //Random.seed = System.DateTime.Now.Millisecond;
                int indexPiece = Random.Range(0, OwnPieces.Count);
                int x = OwnPieces[indexPiece].CurrentX;
                int z = OwnPieces[indexPiece].CurrentZ;
                BoardManager.Instance.activeChessPiece = OwnPieces[indexPiece];
                //Debug.Log(OwnPieces[indexPiece]);

                bool[,] possibleMov = OwnPieces[indexPiece].PossibleMove();
                BoardManager.Instance.allowedMoves = possibleMov;
                List<int[]> mov = new List<int[]>();
                for(int i =0; i<8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (possibleMov[i,j])
                        {
                            mov.Add(new int[] {i,j });
                        }
                    }

                }
                //there is a possible movement for the piece
                if (mov.Count > 0)
                {
                    int indexMovement = Random.Range(0, mov.Count);

                    Debug.Log("there is a possible movement for the piece");
                    int toX = mov[indexMovement][0];
                    int toZ = mov[indexMovement][1];
                    Debug.Log(toX);
                    Debug.Log(toZ);
                    BoardManager.Instance.MovePiece(x, z, toX, toZ);
                    


                    actionExecuted = true;
                }
                break;

            }

        }
    }




}
