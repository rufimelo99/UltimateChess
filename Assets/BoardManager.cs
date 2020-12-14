using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }
    private const float TILE_SIZE = 1.0f;
    //Tile Selected atm
    private int CurrentTileX = -1;
    private int CurrentTileZ = -1;
    //pieces in the board 
    public ChessPiece[,] chessPieces { set; get; }
    private ChessPiece activeChessPiece { set; get; }

    private bool isWhiteTurn = true;    //first turn

    //history of the previous dispositions
    public List<ChessPiece[,]> history { set; get; }

    public List<GameObject> chessPiecesModels; 
    private List<GameObject> activeChessPieceModel;


   //history of the moves
   public string[] record { set; get; }

    private void Start()
    {

        activeChessPieceModel = new List<GameObject>();
        chessPieces = new ChessPiece[8, 8];
        InitialSpawning();
        Instance = this;

        movePiece("a", 1, "a", 4);
        movePiece("b", 1, "a", 3);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateSelection();

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
                    if (allowedMoves[CurrentTileX, CurrentTileZ])
                    {
                        movePiece(convertColumnInttoString(activeChessPiece.CurrentX), activeChessPiece.CurrentZ, convertColumnInttoString(CurrentTileX), CurrentTileZ);

                        UnselectPiece();
                    }

                    UnselectPiece();
                }
            }
            else{
                UnselectPiece(); 
            }
        }

    }

    public void InitialSpawning()
    {
        //White Pieces
        SpawnPiece("a", 1, "R");
        SpawnPiece("b", 1, "N");
        SpawnPiece("c", 1, "B");
        SpawnPiece("d", 1, "Q");
        SpawnPiece("e", 1, "K");
        SpawnPiece("f", 1, "B");
        SpawnPiece("g", 1, "N");
        SpawnPiece("h", 1, "R");
        SpawnPiece("a", 2, "P");
        SpawnPiece("b", 2, "P");
        SpawnPiece("c", 2, "P");
        SpawnPiece("d", 2, "P");
        SpawnPiece("e", 2, "P");
        SpawnPiece("f", 2, "P");
        SpawnPiece("g", 2, "P");
        SpawnPiece("h", 2, "P");

        //Black Pieces
       
        SpawnPiece("a", 8, "r");
        SpawnPiece("b", 8, "n");
        SpawnPiece("c", 8, "b");
        SpawnPiece("d", 8, "q");
        SpawnPiece("e", 8, "k");
        SpawnPiece("f", 8, "b");
        SpawnPiece("g", 8, "n");
        SpawnPiece("h", 8, "r");
        SpawnPiece("a", 7, "p");
        SpawnPiece("b", 7, "p");
        SpawnPiece("c", 7, "p");
        SpawnPiece("d", 7, "p");
        SpawnPiece("e", 7, "p");
        SpawnPiece("f", 7, "p");
        SpawnPiece("g", 7, "p");
        SpawnPiece("h", 7, "p");

        //history.Add(chessPieces);
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
        foreach (GameObject go in activeChessPieceModel)
        {
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

        if (!hasAtLeastOneMove)
        {
            return;
        }

        //Debug.Log("position");
        allowedMoves = chessPieces[x, z].PossibleMove();
        activeChessPiece = chessPieces[x, z];
        HighligthsManager.Instance.AllowedMovesHighlight(allowedMoves);
        HighligthsManager.Instance.ownTileHighligth(x, z);

    }
    private void UnselectPiece()
    {
        HighligthsManager.Instance.RemoveHighlights();
        activeChessPiece = null;
    }
    void UpdateSelection()
    {
        if (!Camera.main)
        {
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
    private void SpawnPiece(string x, int z, string piece)
    {
        int indexInModelsArray = convertPieceNotationToIndex(piece);
        GameObject go = Instantiate(chessPiecesModels[indexInModelsArray], GetTileCenter(x, z-1), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
      
        chessPieces[convertColumnStringtoInt(x), z-1] = go.GetComponent<ChessPiece>();

        chessPieces[convertColumnStringtoInt(x), z-1].setPosition(convertColumnStringtoInt(x), z-1);
        activeChessPieceModel.Add(go);

    }

    private void movePiece(string column, int line, string toColumn, int toLine)
    {
        int from_Column = convertColumnStringtoInt(column);
        int to_Column = convertColumnStringtoInt(toColumn);

        
            ChessPiece enemyPiece = chessPieces[to_Column, toLine];
            if (enemyPiece != null)
            {
                // check if its the king? i dont think ill need it
                if (enemyPiece.GetType() == typeof(KingPiece))
                {
                    //gg
                    FinishGame();
                    return;
                }
               
                if (activeChessPiece.GetType() == typeof(PawnPiece))
                {
                    //trnaform pawn into other piece
                    //white team
                    if (toLine == 7)
                    {
                        activeChessPieceModel.Remove(enemyPiece.gameObject);
                        Destroy(activeChessPiece.gameObject);
                        SpawnPiece(column, toLine, "Q");
                        activeChessPiece = chessPieces[to_Column, toLine];
                    }
                    //black team
                    if (toLine == 0)
                    {
                        activeChessPieceModel.Remove(enemyPiece.gameObject);
                        Destroy(activeChessPiece.gameObject);
                        SpawnPiece(column, toLine, "q");
                        activeChessPiece = chessPieces[to_Column, toLine];
                    }


                    
                }


                activeChessPieceModel.Remove(enemyPiece.gameObject);
                Destroy(enemyPiece.gameObject);
            }

            activeChessPiece = chessPieces[from_Column, line];
            chessPieces[from_Column, line] = null;
            activeChessPiece.setPosition(to_Column, toLine);
            activeChessPiece.transform.position = GetTileCenter(toColumn, toLine);
            chessPieces[to_Column, toLine] = activeChessPiece;

            //change Turn
            isWhiteTurn = !isWhiteTurn;
            activeChessPiece = null;
        
        //history.Add(chessPieces);
    }
    private Vector3 GetTileCenter(string x, int z)
    {
        int xValue = -1;
        switch (x)
        {
            case "a":
                xValue = 0;
                break;
            case "b":
                xValue = 1;
                break;
            case "c":
                xValue = 2;
                break;
            case "d":
                xValue = 3;
                break;
            case "e":
                xValue = 4;
                break;
            case "f":
                xValue = 5;
                break;
            case "g":
                xValue = 6;
                break;
            case "h":
                xValue = 7;
                break;
            default:
                xValue = -1;
                break;
        }
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * xValue) + TILE_SIZE / 2;
        origin.z += (TILE_SIZE * z) + TILE_SIZE / 2;
        return origin;
    }

    //receives piece notation and returns the index of the piece in the chessPiecesModels
    private int convertPieceNotationToIndex(string notation)
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

    //converts columns string into a int
    private int convertColumnStringtoInt(string column)
    {
        switch (column)
        {
            case "a":
                return 0;
            case "b":
                return 1;
            case "c":
                return 2;
            case "d":
                return 3;
            case "e":
                return 4;
            case "f":
                return 5;
            case "g":
                return 6;
            case "h":
                return 7;
            default:
                return -1;
        }

    }
    private string convertColumnInttoString(int column)
    {
        switch (column)
        {
            case 0:
                return "a";
            case 1:
                return "b";
            case 2:
                return "c";
            case 3 :
                return "d";
            case 4:
                return "e";
            case 5:
                return "f";
            case 6:
                return "g";
            case 7:
                return "h";
            default:
                return "x";
        }

    }
}
