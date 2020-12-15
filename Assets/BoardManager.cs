using System.Collections;
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
        activeChessPieceModel = new List<GameObject>();
        chessPieces = new ChessPiece[8, 8];
        EnPassantMove = new int[2] { -1, -1 };

        WhiteKingPos = new int[2] { -1, -1 };
        BlackKingPos = new int[2] { -1, -1 };



        InitialSpawning();
        //updateKingsCoords();
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        //Draw();

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
                    MovePiece(convertColumnInttoString(activeChessPiece.CurrentX), activeChessPiece.CurrentZ, convertColumnInttoString(CurrentTileX), CurrentTileZ);

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
        HighligthsManager.Instance.ownTileHighligth(x, z);
        HighligthsManager.Instance.AllowedMovesHighlight(allowedMoves);
    }
    private void UnselectPiece()
    {
        HighligthsManager.Instance.RemoveHighlights();
        activeChessPiece = null;
    }
    private void MovePiece(string column, int line, string toColumn, int toLine)
    {
        int from_Column = convertColumnStringtoInt(column);
        int to_Column = convertColumnStringtoInt(toColumn);


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
            activeChessPiece.transform.position = GetTileCenter(toColumn, toLine);
            chessPieces[to_Column, toLine] = activeChessPiece;

            //change Turn
            isWhiteTurn = !isWhiteTurn;
        }
        UnselectPiece();
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
    

    private void SpawnPiece(string x, int z, string piece)
    {
        //Debug.Log(chessPiecesModels.Count);
        //gotta lokk into Quarternions to spawn pieces facing the right way (Quaternion.Euler(0,180,0)?)

        int indexInModelsArray = convertPieceNotationToIndex(piece);
        GameObject go = Instantiate(chessPiecesModels[indexInModelsArray], GetTileCenter(x, z - 1), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);

        chessPieces[convertColumnStringtoInt(x), z - 1] = go.GetComponent<ChessPiece>();

        //Debug.Log("x"+x);
        //Debug.Log("z" + z);
        chessPieces[convertColumnStringtoInt(x), z - 1].setPosition(convertColumnStringtoInt(x), z - 1);
        //Debug.Log("CurrentX" + chessPieces[x, z].CurrentX);
        //Debug.Log("CurrentZ" + chessPieces[x, z].CurrentZ);
        activeChessPieceModel.Add(go);

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
            case 3:
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


    //debug only
    /*
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
    */
}
