using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    public ChessAgent[] players;

    public int[] EnPassantMove { set; get; }

    public bool WhiteinCheck = false;
    public bool BlackinCheck = false;
    public ChessPiece[,] chessPieces { set; get; }
    //pieces in the board 
    public List<GameObject> chessPiecesModels;  //no need to initialize.. objects were inserted through the UI on the inspector

    [HideInInspector]
    public float TILE_SIZE = 1.0f;
    //Tile Selected atm
    private int CurrentTileX = -1;
    private int CurrentTileZ = -1;
    private List<GameObject> activeChessPieceModel;
    public ChessPiece activeChessPiece { set; get; }
    public bool isWhiteTurn = true;    //first turn
    public bool[,] allowedMoves { set; get; }


    private void Start()
    {
        activeChessPieceModel = new List<GameObject>();
        chessPieces = new ChessPiece[8, 8];
        EnPassantMove = new int[2] { -1, -1 };


        InitialSpawning();
        Instance = this;

    }



    // Update is called once per frame
    void Update()
    {

        Instance = this;
        if (isWhiteTurn)
        {
            //AI playing
            if (players[0] == null)
            {
                UpdateSelection();
                //Draw();

                if (Input.GetMouseButtonDown(0))
                {
                    if (CurrentTileX >= 0 && CurrentTileZ >= 0)
                    {
                        if (activeChessPiece == null)
                        {
                            //select the piece
                            SelectPiece(CurrentTileX, CurrentTileZ);
                        }
                        else
                        {
                            //move the piece
                            MovePiece(activeChessPiece.CurrentX, activeChessPiece.CurrentZ, CurrentTileX, CurrentTileZ);

                        }
                    }
                }
            }
            else
            {
                //Debug.Log("White is AI... it should play -.-");
                //AI playing
                //players[0].chooseRandomMove();
                players[0].RequestDecision();
                //players[0].RequestAction();
            }
        }
        else
        {
            //AI playing for Black
            if (players[1] == null)
            {
                UpdateSelection();
                //Draw();

                if (Input.GetMouseButtonDown(0))
                {
                    if (CurrentTileX >= 0 && CurrentTileZ >= 0)
                    {
                        if (activeChessPiece == null)
                        {
                            //select the piece
                            SelectPiece(CurrentTileX, CurrentTileZ);
                        }
                        else
                        {
                            //move the piece
                            MovePiece(activeChessPiece.CurrentX, activeChessPiece.CurrentZ, CurrentTileX, CurrentTileZ);

                        }
                    }
                }
            }
            else
            {
                //Debug.Log("Black is AI... it should play -.-");
                //AI playing
                //players[1].chooseRandomMove();
                players[1].RequestDecision();
                //players[1].RequestAction();
            }
        }

    }


    private void FinishGame()
    {
        if (isWhiteTurn)
        {
            Debug.Log("White Win");
            //white win
        }
        else 
        {
            Debug.Log("Black Win");
            //black win
        }
        foreach (GameObject go in activeChessPieceModel) {
            Destroy(go);
        }
        foreach(ChessAgent ca in players)
        {
            if (ca != null)
            {
                ca.EndEpisode();
            }
        }
        ResetGame();
    }

    //resets game
    private void ResetGame()
    {
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
        HighligthsManager.Instance.ownTileHighligth(x, z);
        HighligthsManager.Instance.AllowedMovesHighlight(allowedMoves);
    }
    private void UnselectPiece()
    {
        HighligthsManager.Instance.RemoveHighlights();
        activeChessPiece = null;
    }
    public void MovePiece(int column, int line, int to_Column, int toLine)
    {
        if (allowedMoves[to_Column, toLine]) {
            ChessPiece enemyPiece = chessPieces[to_Column, toLine];
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
            if (to_Column == EnPassantMove[0] && toLine == EnPassantMove[1])
            {
                //remove black pawn->is white Turn
                if (isWhiteTurn)
                {
                    enemyPiece = chessPieces[to_Column, toLine - 1];
                }
                //remove white pawn->is black Turn
                else
                {
                    enemyPiece = chessPieces[to_Column, toLine + 1];
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
                if (toLine == 7)
                {
                    activeChessPieceModel.Remove(activeChessPiece.gameObject);
                    
                    Destroy(activeChessPiece.gameObject);
                    SpawnPiece(column, toLine, "Q");
                    activeChessPiece = chessPieces[to_Column, toLine];
                }
                //black team
                if (toLine == 0)
                {
                    activeChessPieceModel.Remove(activeChessPiece.gameObject);

                    Destroy(activeChessPiece.gameObject);
                    SpawnPiece(column, toLine, "q");
                    activeChessPiece = chessPieces[to_Column, toLine];
                }


                //if movement is 2 tiles
                if (activeChessPiece.CurrentZ == 1 && toLine == 3)
                {
                    EnPassantMove[0] = to_Column;
                    EnPassantMove[1] = toLine - 1;
                }
                //if movement is 2 tiles
                else if (activeChessPiece.CurrentZ == 6 && toLine == 4)
                {
                    EnPassantMove[0] = to_Column;
                    EnPassantMove[1] = toLine + 1;
                }
            }




            chessPieces[activeChessPiece.CurrentX, activeChessPiece.CurrentZ] = null;
            activeChessPiece.setPosition(to_Column, toLine);
            activeChessPiece.transform.position = GetTileCenter(to_Column, toLine);
            chessPieces[to_Column, toLine] = activeChessPiece;

            //change Turn
            isWhiteTurn = !isWhiteTurn;
        }
        UnselectPiece();
    }

    public void InitialSpawning()
    {
        //White Pieces
        SpawnPiece(0, 0, "R");
        SpawnPiece(7, 0, "R");
        SpawnPiece(1, 0, "N"); 
        SpawnPiece(6, 0, "N");
        SpawnPiece(3, 0, "Q");
        SpawnPiece(4, 0, "K");

        SpawnPiece(5, 0, "B");
        SpawnPiece(2, 0, "B");
        
         SpawnPiece(0, 1, "P");
         SpawnPiece(1, 1, "P");
         SpawnPiece(2, 1, "P");
         SpawnPiece(3, 1, "P");
         SpawnPiece(4, 1, "P");
         SpawnPiece(5, 1, "P");
         SpawnPiece(6, 1, "P");
         SpawnPiece(7, 1, "P");
         


        //Black Pieces

        SpawnPiece(0, 7, "r");
        SpawnPiece(7, 7, "r");
        SpawnPiece(1, 7, "n");
        SpawnPiece(3, 7, "q");
        SpawnPiece(4, 7, "k");
        SpawnPiece(6, 7, "n");
        SpawnPiece(5, 7, "b");
        SpawnPiece(2, 7, "b");

        SpawnPiece(0, 6, "p");
        SpawnPiece(1, 6, "p");
        SpawnPiece(2, 6, "p");
        SpawnPiece(3, 6, "p");
        SpawnPiece(4, 6, "p");
        SpawnPiece(5, 6, "p");
        SpawnPiece(6, 6, "p");
        SpawnPiece(7, 6, "p");
        
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
    

    public void SpawnPiece(int x, int z, string piece)
    {
        int indexInModelsArray = convertPieceNotationToIndex(piece);
        //GameObject go = Instantiate(chessPiecesModels[indexInModelsArray], GetTileCenter(x, z - 1), Quaternion.identity) as GameObject;
        GameObject go = Instantiate(chessPiecesModels[indexInModelsArray], GetTileCenter(x, z), Quaternion.Euler(0, 90, 0)) as GameObject;
        go.transform.SetParent(transform);

        chessPieces[x, z] = go.GetComponent<ChessPiece>();

        chessPieces[x, z].setPosition(x, z);
        activeChessPieceModel.Add(go);

    }
    private Vector3 GetTileCenter(int x, int z)
    {
       
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_SIZE / 2;
        origin.z += (TILE_SIZE * z) + TILE_SIZE / 2;
        return origin;
    }
    //receives piece notation and returns the index of the piece in the chessPiecesModels
    public int convertPieceNotationToIndex(string notation)
    {
        //small Leters ->Black
        //Capital Letter ->white
        switch (notation)
        {
            //rooks
            case "r":
                return 0;
            case "R":
                return 6;
            //knights->horses
            case "n":
                return 1;
            case "N":
                return 7;
            //bishops
            case "b":
                return 2;
            case "B":
                return 8;
            //Queen
            case "q":
                return 3;
            case "Q":
                return 9;
            //King
            case "k":
                return 4;
            case "K":
                return 10;
            //pawns
            case "p":
                return 5;
            case "P":
                return 11;
            default:
                return -1;
        }


    }

   
    public ChessPiece[,] getchessPieces()
    {
        return chessPieces;
    }

    public bool hasOnePossibleMove(int x, int z)
    {
        bool hasAtLeastOneMove = false;
        allowedMoves = chessPieces[x, z].PossibleMove();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                }
            }
        }
        return hasAtLeastOneMove;
    }
}
