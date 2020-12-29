using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    [HideInInspector]
    public bool isWhiteTurn = true;
    /// <summary>
    /// Both whitePlayer and blackPlayers are variables that, for now, can only be edited through the unity UI
    /// They represent each player. if filled with an ChessAgent Object, there is an AI behind that player.
    /// Otherwise, in case of being null, it means that player is, in fact, human
    /// </summary>
    public ChessAgent whitePlayer;
    public ChessAgent blackPlayer;
    /// <summary>
    /// Variables used for Castling Movement
    /// Castling consists in almost "swapping" a rook with the king
    /// this movement can only be done when the king has never moved before neither that same rook
    /// it can not be done while the king is in check or if it tries to move into a checking position
    /// examples: 
    /// king from e1 to b1 and rook from a1 to c1
    /// king from e1 to g1 and rook from h1 to f1
    /// king from e8 to b8 and rook from a8 to c8
    /// king from e8 to g8 and rook from h8 to f8
    /// Each variables sees if that particular Piece was moves before
    /// </summary>
    [HideInInspector]
    public bool didWhiteRook0EverMoved = false;
    [HideInInspector]
    public bool didWhiteRook1EverMoved = false;
    [HideInInspector]
    public bool didWhiteKingEverMoved = false;
    [HideInInspector]
    public bool didBlackRook0EverMoved = false;
    [HideInInspector]
    public bool didBlackRook1EverMoved = false;
    [HideInInspector]
    public bool didBlackKingEverMoved = false;
    /// <summary>
    /// Variable for the En Passant Movement
    /// Bassically, when an opposite pawn makes the 2 tiles movement and a own pawn (immediately after) could eat that pawn in diagonal, case the enemy piece only moved 1 tile
    /// The own pawn moves in the diagonal like normal
    /// and the opposite pawn is removed
    /// One example:
    /// Pawn from a5 to b6 eats an opposite pawn that in its previous move went from b7 to b5
    /// </summary>
    public int[] EnPassantMove { set; get; }
    /// <summary>
    /// Bidimensional Array with all the pieces on the board
    /// </summary>
    public ChessPiece[,] chessPieces { set; get; }
    /// <summary>
    /// Bidimensional Array with all the pieces prefabs on the board
    /// It is initialized and changed in the Unity UI
    /// </summary>
    public List<GameObject> chessPiecesModels; 
    [HideInInspector]
    public float TILE_SIZE = 1.0f;
    /// <summary>
    /// Tile Selected at the moment
    /// </summary>
    private int CurrentTileX = -1;
    private int CurrentTileZ = -1;
    /// <summary>
    /// Piece Prefab selected at the moment (important for the human player)
    /// Allow higlights as well
    /// </summary>
    private List<GameObject> activeChessPieceModel;
    /// <summary>
    /// ChessPiece selected at the moment
    /// </summary>
    public ChessPiece activeChessPiece { set; get; }
    /// <summary>
    /// bidimensional array with true or false values that allows the BoardManager to know to where a certain piece (activeChessPiece and, subsequently, activeChessPieceModel) can move 
    /// </summary>
    public bool[,] allowedMoves { set; get; }
    /// <summary>
    /// Start() method
    /// initialize some variables 
    /// proceeds to spawn the Pieces 
    /// and make an instance to be accessible from other scripts
    /// </summary>
    private void Start()
    {
        activeChessPieceModel = new List<GameObject>();
        chessPieces = new ChessPiece[8, 8];
        EnPassantMove = new int[2] { -1, -1 };
        InitialSpawning();
        Instance = this;

    }
    /// <summary>
    /// Update() is called one per frame
    /// Proceeds to check if castling could be allowed in that move through updating some vars
    /// Note:
    /// dont know if the requestDecision() is needed when the AI is playing: i think it will be called either way
    /// </summary>
    void Update()
    {
        checkIfRooksOrKingMoves();
        Instance = this;
        //White PLaying
        if (isWhiteTurn)
        {
            //Human Player here
            if (whitePlayer == null)
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
            //AI playing
            else
            {
                whitePlayer.RequestDecision();
            }
        }
        //Black PLaying
        else
        {
            //Human Player here
            if (blackPlayer == null)
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
                            MovePiece(activeChessPiece.CurrentX, activeChessPiece.CurrentZ, CurrentTileX, CurrentTileZ);

                        }
                    }
                }
            }
            //AI playing
            else
            {
                blackPlayer.RequestDecision();
            }
        }

    }
    /// <summary>
    /// Destroy pieces
    /// if a Human if is playing, it will reset the board in order to play
    /// Otherwise, the AI when simulating, will restart on their own (at least when training)
    /// </summary>
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
        if( whitePlayer == null || blackPlayer == null)
        {
            ResetGame();
        }
    }
    /// <summary>
    /// Proceeds to reset some vars and delets pieces once again if needed to (need for the training)
    /// remove highlights that may exist on the floor
    /// and spawn the pieces again for a new game
    /// </summary>
    public void ResetGame()
    {
        //reset vars
        isWhiteTurn = true;    
        didWhiteRook0EverMoved = false;
        didWhiteRook1EverMoved = false;
        didWhiteKingEverMoved = false;
        didBlackRook0EverMoved = false;
        didBlackRook1EverMoved = false; 
        didBlackKingEverMoved = false;
        HighligthsManager.Instance.RemoveHighlights(); 
        foreach (GameObject go in activeChessPieceModel)
        {
            Destroy(go);
        }
        InitialSpawning();

    }
    /// <summary>
    /// Updates activeChessPiece in case it has at least one move 
    /// </summary>
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
        allowedMoves = chessPieces[x, z].PossibleMove();
        activeChessPiece = chessPieces[x, z];
        HighligthsManager.Instance.ownTileHighligth(x, z);
        HighligthsManager.Instance.AllowedMovesHighlight(allowedMoves);
    }
    /// <summary>
    /// sets activeChessPiece to null and removes possible highlights from allowed movements
    /// </summary>
    private void UnselectPiece()
    {
        HighligthsManager.Instance.RemoveHighlights();
        activeChessPiece = null;
    }
    /// <summary>
    /// Actually moves a certain piece chessPieces[column, line] in a particular position (column;line)
    /// to (to_Column, toLine)
    /// Also checks for special moves
    /// </summary>
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
                    SpawnPiece(to_Column, toLine, "Q");
                    activeChessPiece = chessPieces[to_Column, toLine];
                }
                //black team
                if (toLine == 0)
                {
                    activeChessPieceModel.Remove(activeChessPiece.gameObject);
                    Destroy(activeChessPiece.gameObject);
                    SpawnPiece(to_Column, toLine, "q");
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

            //castling
            //only need to change rooks positioning here
            //king's will change after this if either way
            if(activeChessPiece.GetType() == typeof(KingPiece))
            {
                if(to_Column == column - 3)
                {
                    ChessPiece rook = chessPieces[column - 4, line];
                    chessPieces[column - 4, line] = null;
                    rook.setPosition(column - 4 + 2, line);
                    rook.transform.position = GetTileCenter(column - 4 + 2, line);
                    chessPieces[column - 4 + 2, toLine] = rook;

                }
                else if(to_Column == column + 2)
                {
                    ChessPiece rook = chessPieces[column + 3, line];
                    chessPieces[column + 3, line] = null;
                    rook.setPosition(column + 3 - 2, line);
                    rook.transform.position = GetTileCenter(column + 3 - 2, line);
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
    /// <summary>
    /// Initial sawning presentation for the begining of the game
    /// </summary>
    public void InitialSpawning()
    {
        //White Pieces
        
        SpawnPiece(0, 0, "R");
        SpawnPiece(7, 0, "R");

        SpawnPiece(1, 0, "N"); 
        SpawnPiece(6, 0, "N");

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
         
        SpawnPiece(3, 0, "Q");
       SpawnPiece(4, 0, "K");


        //Black Pieces
       
        SpawnPiece(0, 7, "r");
        SpawnPiece(7, 7, "r"); 
        SpawnPiece(1, 7, "n");
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
        
        SpawnPiece(3, 7, "q");
        SpawnPiece(4, 7, "k");
    }
    /// <summary>
    /// updates the CurrentX and CurrentZ to the elected tile x and z 
    /// </summary>
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
    /// <summary>
    /// spawn a certain piece in the board at a certain position
    /// each piece follows chess notation
    /// </summary>
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
    /// <summary>
    /// returns the actual center of a tile in the board so we can place a Prefab correctly
    /// </summary>
    private Vector3 GetTileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_SIZE / 2;
        origin.z += (TILE_SIZE * z) + TILE_SIZE / 2;
        return origin;
    }
    /// <summary>
    /// return the index of a certain prefab given the chess notation for the same piece
    /// each prefab is in ChessPieceModels
    /// </summary>
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
    /// <summary>
    /// function that returns true or false in case it has one possible movement
    /// </summary>
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
    /// <summary>
    /// simple function to update castling variables
    /// </summary>
    public void checkIfRooksOrKingMoves()
    {
        if (didWhiteRook0EverMoved == false)
        {
            if (chessPieces[0, 0] == null || chessPieces[0, 0].GetType() != typeof(TowerPiece))
            {
                didWhiteRook0EverMoved = true;
            }
        }
        if (didWhiteRook1EverMoved == false)
        {
            if (chessPieces[7, 0] == null || chessPieces[7, 0].GetType() != typeof(TowerPiece))
            {
                didWhiteRook1EverMoved = true;
            }
        }
        if (didBlackRook0EverMoved == false)
        {
            if (chessPieces[0, 7] == null || chessPieces[0, 7].GetType() != typeof(TowerPiece))
            {
                didBlackRook0EverMoved = true;
            }
        }
        if (didBlackRook1EverMoved == false)
        {
            if (chessPieces[7, 7] == null || chessPieces[7, 7].GetType() != typeof(TowerPiece))
            {
                didBlackRook1EverMoved = true;
            }
        }
        if (didWhiteKingEverMoved == false)
        {
            if (chessPieces[4, 0] == null || chessPieces[4, 0].GetType() != typeof(KingPiece))
            {
                didWhiteKingEverMoved = true;
            }
        }
        if (didBlackKingEverMoved == false)
        {
            if (chessPieces[4, 7] == null || chessPieces[4, 7].GetType() != typeof(KingPiece))
            {
                didBlackKingEverMoved = true;
            }
        }
    }
}
