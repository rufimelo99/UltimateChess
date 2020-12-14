﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }

    public ChessPiece[,] chessPieces { set; get; }
    private ChessPiece activeChessPiece { set; get; }

    //also declared in BoardHighlights -.-
    private const float TILE_SIZE = 1.0f;

    //Tile Selected atm
    private int CurrentTileX = -1;
    private int CurrentTileZ = -1;

    //pieces in the board 
    public List<GameObject> chessPiecesModels;  //no need to initialize.. objects were inserted through the UI on the inspector
    private List<GameObject> activeChessPieceModel;

    public int[] EnPassantMove { set; get; }

    public int[] WhiteKingPos { set; get; }
    public int[] BlackKingPos { set; get; }

    public bool WhiteinCheck = false;
    public bool BlackinCheck = false;

    private bool isWhiteTurn = true;    //first turn

    private void Start()
    {
        InitialSpawning();
        updateKingsCoords();
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        Draw();

        if (Input.GetMouseButtonDown(0)) { 
            if(CurrentTileX >= 0 && CurrentTileZ >= 0)
            {
                if (activeChessPiece == null)
                {
                    //select the piece
                    SelectPiece(CurrentTileX, CurrentTileZ);
                }
                else {
                    //move the piece
                    MovePiece(CurrentTileX, CurrentTileZ);
                }
            }
        }

        //VerifyChecks();
    }


    public void updateKingsCoords() {
        foreach (ChessPiece piece in chessPieces)
        {
            if (piece != null && piece.GetType() == typeof(KingPiece))
            {
                //get white king position
                if (piece.isWhite)
                {
                    WhiteKingPos[0] = piece.CurrentX;
                    WhiteKingPos[1] = piece.CurrentZ;
                }
                //get black king position
                else
                {
                    BlackKingPos[0] = piece.CurrentX;
                    BlackKingPos[1] = piece.CurrentZ;
                }
            }
        }
    }
    //bad implemented TODO
    public bool VerifyChecks()
    {
       
        //temp vars
        bool blackisInCheck = true;
        bool whiteisInCheck = true;
        foreach (ChessPiece piece in chessPieces)
        {
            if (piece != null)
            {
                if (piece.isWhite)
                {
                    //cant do this
                    bool[,] map = piece.PossibleMove();
                    //Black in check
                    if (map[BlackKingPos[0], BlackKingPos[1]] == true)
                    {
                        blackisInCheck = true;
                    }
                }
                else
                {
                    bool[,] map = piece.PossibleMove();
                    //White in check
                    if (map[BlackKingPos[0], BlackKingPos[1]] == true)
                    {
                        whiteisInCheck = true;
                    }
                }
            }
        }
        BlackinCheck = blackisInCheck;        
        WhiteinCheck = whiteisInCheck;


        return false;
    }
    

    private void FinishGame()
    {
        if (isWhiteTurn)
        {
            Debug.Log("You Win");
        }
        else 
        {
            Debug.Log("You Lost to an AI");
        }
        foreach (GameObject go in activeChessPieceModel) {
            Destroy(go);
        }
        //reset game if needed
        isWhiteTurn = true;
        HighligthsManager.Instance.RemoveHighlights();
        InitialSpawning();

    }

    private void SelectPiece(int x, int z)
    {
        if (chessPieces[x, z] == null)
        {
            //no corresponding piece 
            return;
        }
        if (chessPieces[x, z].isWhite != isWhiteTurn)
        {
            //check if the piece corresponds to the player... if not, exit
            return;
        }

        bool hasAtLeastOneMove = false;
        allowedMoves = chessPieces[x, z].PossibleMove();
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j]) {
                    hasAtLeastOneMove = true;
                }
            }
        }

        if (!hasAtLeastOneMove)
        {
            return;
        }

        //Debug.Log("position");
        allowedMoves = chessPieces[x, z].PossibleMove();
        activeChessPiece = chessPieces[x, z];
        HighligthsManager.Instance.AllowedMovesHighlight(allowedMoves);
    }
    private void UnselectPiece()
    {
        HighligthsManager.Instance.RemoveHighlights();
        activeChessPiece = null;
    }
    private void MovePiece(int x, int z)
    {
        if (allowedMoves[x, z]) {
            ChessPiece enemyPiece = chessPieces[x, z];
            //capture a piece
            if (enemyPiece != null)
            {
                //check if its the king? i dont think ill need it
                if (enemyPiece.GetType() == typeof(KingPiece))
                {
                    //gg
                    FinishGame();
                    return;
                }

                activeChessPieceModel.Remove(enemyPiece.gameObject);
                Destroy(enemyPiece.gameObject);
            }

            //actually did the 2 tiles movement
            if (x == EnPassantMove[0] && z == EnPassantMove[1])
            {
                //remove black pawn->is white Turn
                if (isWhiteTurn)
                {
                    enemyPiece = chessPieces[x, z - 1];
                }
                //remove white pawn->is black Turn
                else
                {
                    enemyPiece = chessPieces[x, z + 1];
                }
                activeChessPieceModel.Remove(enemyPiece.gameObject);
                Destroy(enemyPiece.gameObject);
            }

            //record possible en passant moves
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;
            if (activeChessPiece.GetType() == typeof(PawnPiece))
            {
                //trnaform pawn into other piece
                //white team
                if (z == 7)
                {
                    activeChessPieceModel.Remove(enemyPiece.gameObject);
                    Destroy(activeChessPiece.gameObject);
                    SpawnPiece(27, x, z);
                    activeChessPiece = chessPieces[x, z];
                }
                //black team
                if (z == 0)
                {
                    activeChessPieceModel.Remove(enemyPiece.gameObject);
                    Destroy(activeChessPiece.gameObject);
                    SpawnPiece(3, x, z);
                    activeChessPiece = chessPieces[x, z];
                }


                //if movement is 2 tiles
                if (activeChessPiece.CurrentZ == 1 && z == 3)
                {
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = z-1;
                }
                //if movement is 2 tiles
                else if (activeChessPiece.CurrentZ == 6 && z == 4)
                {
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = z+1;
                }
            }




            chessPieces[activeChessPiece.CurrentX, activeChessPiece.CurrentZ] = null;
            activeChessPiece.setPosition(x,z);
            activeChessPiece.transform.position = GetTileCenter(x, z);
            chessPieces[x, z] = activeChessPiece;

            //change Turn
            isWhiteTurn = !isWhiteTurn;
        }
        UnselectPiece();
    }

    private void InitialSpawning() {
        activeChessPieceModel = new List<GameObject>();
        chessPieces = new ChessPiece[8, 8];
        EnPassantMove = new int[2] { -1, -1 };

        WhiteKingPos = new int[2] { -1, -1 };
        BlackKingPos = new int[2] { -1, -1 };


        //  WhiteKingPos = new int[2] { 4, 0 };
        //  BlackKingPos = new int[2] { 4, 7 };


        //Spawn Black Towers
        SpawnPiece(0, 0, 7);
        SpawnPiece(7, 7, 7);
        //Spawn Black Horses
        SpawnPiece(1, 1, 7);
        SpawnPiece(6, 6, 7);
        //Spawn Black Bishops
        SpawnPiece(2, 2, 7);
        SpawnPiece(5, 5, 7);
        //Spawn Black Queen and King
        SpawnPiece(3, 3, 7);
        SpawnPiece(4, 4, 7);
        //Spawn Black Pawns
        SpawnPiece(8, 0, 6);
        SpawnPiece(9, 1, 6);
        SpawnPiece(10, 2, 6);
        SpawnPiece(11, 3, 6);
        SpawnPiece(12, 4, 6);
        SpawnPiece(13, 5, 6);
        SpawnPiece(14, 6, 6);
        SpawnPiece(15, 7, 6);

        //Spawn White Pawns
        SpawnPiece(16, 0, 1);
        SpawnPiece(17, 1, 1);
        SpawnPiece(18, 2, 1);
        SpawnPiece(19, 3, 1);
        SpawnPiece(20, 4, 1);
        SpawnPiece(21, 5, 1);
        SpawnPiece(22, 6, 1);
        SpawnPiece(23, 7, 1);

        //Spawn white Towers
        SpawnPiece(24, 0, 0);
        SpawnPiece(31, 7, 0);
        //Spawn white Horses           
        SpawnPiece(25, 1, 0);
        SpawnPiece(30, 6, 0);
        //Spawn white Bishops          
        SpawnPiece(26, 2, 0);
        SpawnPiece(29, 5, 0);
        //Spawn white Queen and King   
        SpawnPiece(27, 3, 0);
        SpawnPiece(28, 4, 0);

    }

    void UpdateSelection()
    {
        if (!Camera.main) {
            return;
        }
        RaycastHit hit;
        //LayerMask.GetMask("ChessPlane") returns the int in layers in the chess board
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            //where did the collision happened
            //Debug.Log(hit.point);
            CurrentTileX = (int)hit.point.x;
            CurrentTileZ = (int)hit.point.z;
        }
        else
        {
            CurrentTileX = -1;
            CurrentTileZ = -1;
        }
    }
    
    //Spawn a Certain Piece from the List ChessPieces and gives it a position (objects inside list defined in the BoardManager on the UI inspector)
    private void SpawnPiece(int index, int x, int  z) {
        //Debug.Log(chessPiecesModels.Count);
        //gotta lokk into Quarternions to spawn pieces facing the right way (Quaternion.Euler(0,180,0)?)
        GameObject go = Instantiate(chessPiecesModels[index], GetTileCenter(x,z), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        chessPieces[x, z] = go.GetComponent<ChessPiece>();

        //Debug.Log("x"+x);
        //Debug.Log("z" + z);
        chessPieces[x, z].setPosition(x, z);
        //Debug.Log("CurrentX" + chessPieces[x, z].CurrentX);
        //Debug.Log("CurrentZ" + chessPieces[x, z].CurrentZ);
        activeChessPieceModel.Add(go);
    }

    private Vector3 GetTileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_SIZE/2;
        origin.z += (TILE_SIZE * z) + TILE_SIZE/2;
        return origin;
    }


    private void Draw()
    {
        Vector3 widthLine  = Vector3.right    * 8;
        Vector3 heigthLine = Vector3.forward  * 8;


        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start+widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start +heigthLine );

            }
        }
        //Draw Selection
        if (CurrentTileX >= 0 && CurrentTileZ >= 0) {
            Debug.DrawLine(
                    Vector3.forward * CurrentTileZ + Vector3.right * CurrentTileX,
                    Vector3.forward * (CurrentTileZ + 1) + Vector3.right * (CurrentTileX + 1),
                    Color.red
                );
        }

    }
}
