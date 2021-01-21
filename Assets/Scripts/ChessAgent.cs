﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class ChessAgent : Agent
{
    /// <summary>
    /// boolean editable in the UI inspector
    /// </summary>
    public bool isWhitePlayer;
    public BoardManager boardCurrentlyPlaying;
    /// <summary>
    /// Reward Values.. changed over time
    /// -actions
    /// -eat pieces
    /// -penalize for missing pieces ? (except extra queens)
    /// little incentive to castling since it is a powerful move
    /// </summary>
    private float validAction = 0.00001f;
    public float wonGame = 1.0f;
    public float lostGame = -1.0f;
    private float invalidAction_or_doNothing = -0.00005f;
    //strengths Update at 29-12-2020
    private float strengthPawn        = 0.000001f;
    private float strengthHorse       = 0.000003f;
    private float strengthBishop      = 0.000003f;
    private float strengthRook        = 0.000005f;
    private float strengthQueen       = 0.000009f;
    private float strengthKing        = 0.00005f;
    //extra incentive for castling
    private float incentiveToCastling = 0.0001f;
    private float incentiveToConvert  = 0.0001f;
    private bool validMove = false;
    private float tempReward = 0.0f;
    private float episodeTime;
    public bool debug = false;
    public bool resetWithTime = false;
    public float maxTimeForEpisode = 30.0f;

    float[,] tableKingWhite = new float[8, 8] {     { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -2.0f, -3.0f , -3.0f , -4.0f , -4.0f , -3.0f , -3.0f , -2.0f },
                                                    { -1.0f, -2.0f , -2.0f , -2.0f , -2.0f , -2.0f , -2.0f , -1.0f },
                                                    {  2.0f,  0.0f,   0.0f,   0.0f,   0.0f,   0.0f,   0.0f ,  2.0f },
                                                    {  2.0f,  3.0f ,  0.0f ,  0.0f ,  0.0f ,  0.0f ,  3.0f ,  2.0f }};

    float[,] tableQueenWhite = new float[8, 8] {    { -2.0f, -1.0f , -1.0f , -0.5f , -0.5f , -1.0f , -1.0f , -2.0f },
                                                    { -1.0f,  0.0f ,  0.0f ,  0.0f ,  0.0f ,  0.0f ,  0.0f , -1.0f },
                                                    { -1.0f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -1.0f },
                                                    { -0.5f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -0.5f },
                                                    { -0.5f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -0.5f },
                                                    { -0.5f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -0.5f },
                                                    { -1.0f,  0.0f ,  0.5f ,  0.0f ,  0.0f ,  0.0f ,  0.0f , -1.0f },
                                                    { -2.0f, -1.0f , -1.0f , -0.5f , -0.5f , -1.0f , -1.0f , -2.0f }};

    float[,] tableRookWhite = new float[8, 8] {     {  0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,  0.0f},
                                                    {  0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f,  0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    {  0.0f, 0.0f, 0.0f, 0.5f, 0.5f, 0.0f, 0.0f,  0.0f}
                                                    };

    float[,] tableBishopWhite = new float[8, 8] {   { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f},
                                                    { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
                                                    { -1.0f,  0.0f,  0.5f,  1.0f,  1.0f,  0.5f,  0.0f, -1.0f},
                                                    { -1.0f,  0.5f,  0.5f,  1.0f,  1.0f,  0.5f,  0.5f, -1.0f},
                                                    { -1.0f,  0.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.0f, -1.0f},
                                                    { -1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f, -1.0f},
                                                    { -1.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.5f, -1.0f},
                                                    { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f}
                                                    };

    float[,] tableHorseWhite = new float[8, 8] {    { -5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f},
                                                    { -4.0f, -2.0f,  0.0f,  0.0f,  0.0f,  0.0f, -2.0f, -4.0f},
                                                    { -3.0f,  0.0f,  1.0f,  1.5f,  1.5f,  1.0f,  0.0f, -3.0f},
                                                    { -3.0f,  0.5f,  1.5f,  2.0f,  2.0f,  1.5f,  0.5f, -3.0f},
                                                    { -3.0f,  0.0f,  1.5f,  2.0f,  2.0f,  1.5f,  0.0f, -3.0f},
                                                    { -3.0f,  0.5f,  1.0f,  1.5f,  1.5f,  1.0f,  0.5f, -3.0f},
                                                    { -4.0f, -2.0f,  0.0f,  0.5f,  0.5f,  0.0f, -2.0f, -4.0f},
                                                    { -5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f}
                                                    };

    float[,] tablePawnWhite = new float[8, 8] {     { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                    { 5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f, 5.0f},
                                                    { 1.0f,  1.0f,  2.0f,  3.0f,  3.0f,  2.0f,  1.0f, 1.0f},
                                                    { 0.5f,  0.5f,  1.0f,  2.5f,  2.5f,  1.0f,  0.5f, 0.5f},
                                                    { 0.0f,  0.0f,  0.0f,  2.0f,  2.0f,  0.0f,  0.0f, 0.0f},
                                                    { 0.5f, -0.5f, -1.0f,  0.0f,  0.0f, -1.0f, -0.5f, 0.5f},
                                                    { 0.5f,  1.0f,  1.0f, -2.0f, -2.0f,  1.0f,  1.0f, 0.5f},
                                                    { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, 0.0f}
                                                    };

    //the board is not relative.. i have to invert the tables
    float[,] tableKingBlack = new float[8, 8] {     {  2.0f,  3.0f ,  0.0f ,  0.0f ,  0.0f ,  0.0f ,  3.0f ,  2.0f }, 
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -3.0f, -4.0f , -4.0f , -5.0f , -5.0f , -4.0f , -4.0f , -3.0f },
                                                    { -2.0f, -3.0f , -3.0f , -4.0f , -4.0f , -3.0f , -3.0f , -2.0f },
                                                    { -1.0f, -2.0f , -2.0f , -2.0f , -2.0f , -2.0f , -2.0f , -1.0f },
                                                    {  2.0f,  0.0f,   0.0f,   0.0f,   0.0f,   0.0f,   0.0f ,  2.0f }};

    float[,] tableQueenBlack = new float[8, 8] {    { -2.0f, -1.0f , -1.0f , -0.5f , -0.5f , -1.0f , -1.0f , -2.0f },
                                                    { -2.0f, -1.0f , -1.0f , -0.5f , -0.5f , -1.0f , -1.0f , -2.0f },
                                                    { -1.0f,  0.0f ,  0.0f ,  0.0f ,  0.0f ,  0.0f ,  0.0f , -1.0f },
                                                    { -1.0f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -1.0f },
                                                    { -0.5f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -0.5f },
                                                    { -0.5f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -0.5f },
                                                    { -0.5f,  0.0f ,  0.5f ,  0.5f ,  0.5f ,  0.5f ,  0.0f , -0.5f },
                                                    { -1.0f,  0.0f ,  0.5f ,  0.0f ,  0.0f ,  0.0f ,  0.0f , -1.0f }};

    float[,] tableRookBlack = new float[8, 8] {     {  0.0f, 0.0f, 0.0f, 0.5f, 0.5f, 0.0f, 0.0f,  0.0f},
                                                    {  0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,  0.0f},
                                                    {  0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f,  0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f},
                                                    { -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.5f}};

    float[,] tableBishopBlack = new float[8, 8] {   { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f},
                                                    { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f},
                                                    { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
                                                    { -1.0f,  0.0f,  0.5f,  1.0f,  1.0f,  0.5f,  0.0f, -1.0f},
                                                    { -1.0f,  0.5f,  0.5f,  1.0f,  1.0f,  0.5f,  0.5f, -1.0f},
                                                    { -1.0f,  0.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.0f, -1.0f},
                                                    { -1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f, -1.0f},
                                                    { -1.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.5f, -1.0f}};

    float[,] tableHorseBlack = new float[8, 8] {    { -5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f},
                                                    { -5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f},
                                                    { -4.0f, -2.0f,  0.0f,  0.0f,  0.0f,  0.0f, -2.0f, -4.0f},
                                                    { -3.0f,  0.0f,  1.0f,  1.5f,  1.5f,  1.0f,  0.0f, -3.0f},
                                                    { -3.0f,  0.5f,  1.5f,  2.0f,  2.0f,  1.5f,  0.5f, -3.0f},
                                                    { -3.0f,  0.0f,  1.5f,  2.0f,  2.0f,  1.5f,  0.0f, -3.0f},
                                                    { -3.0f,  0.5f,  1.0f,  1.5f,  1.5f,  1.0f,  0.5f, -3.0f},
                                                    { -4.0f, -2.0f,  0.0f,  0.5f,  0.5f,  0.0f, -2.0f, -4.0f}};

    float[,] tablePawnBlack = new float[8, 8] {     { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                    { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                    { 5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f, 5.0f},
                                                    { 1.0f,  1.0f,  2.0f,  3.0f,  3.0f,  2.0f,  1.0f, 1.0f},
                                                    { 0.5f,  0.5f,  1.0f,  2.5f,  2.5f,  1.0f,  0.5f, 0.5f},
                                                    { 0.0f,  0.0f,  0.0f,  2.0f,  2.0f,  0.0f,  0.0f, 0.0f},
                                                    { 0.5f, -0.5f, -1.0f,  0.0f,  0.0f, -1.0f, -0.5f, 0.5f},
                                                    { 0.5f,  1.0f,  1.0f, -2.0f, -2.0f,  1.0f,  1.0f, 0.5f}};

    //Kings Positions
    int kingX;
    int kingZ;
    int enemykingX;
    int enemykingZ;

    //Towers Positions
    int rookX0;
    int rookZ0;
    int rookX1;
    int rookZ1;
    int enemyRookX0;
    int enemyRookZ0;
    int enemyRookX1;
    int enemyRookZ1;

    //Horses/Knights Positions
    int horseX0;
    int horseZ0;
    int horseX1;
    int horseZ1;
    int enemyHorseX0;
    int enemyHorseZ0;
    int enemyHorseX1;
    int enemyHorseZ1;

    //bishops Positions
    int bishopX0;
    int bishopZ0;
    int enemyBishopX0;
    int enemyBishopZ0;
    int bishopX1;
    int bishopZ1;
    int enemyBishopX1;
    int enemyBishopZ1;

    //queen positions
    int queenX;
    int queenZ;
    int enemyQueenX;
    int enemyQueenZ;

    //need to add eventual Pieces: extra queens->extreme case 8 extra queens from each +player
    int extraQueenX0;
    int extraQueenZ0;
    int extraQueenX1;
    int extraQueenZ1;
    int extraQueenX2;
    int extraQueenZ2;

    int enemyExtraQueenX0;
    int enemyExtraQueenZ0;
    int enemyExtraQueenX1;
    int enemyExtraQueenZ1;
    int enemyExtraQueenX2;
    int enemyExtraQueenZ2;


    //pawns -.-
    int pawnX0;
    int pawnZ0;
    int pawnX1;
    int pawnZ1;
    int pawnX2;
    int pawnZ2;
    int pawnX3;
    int pawnZ3;
    int pawnX4;
    int pawnZ4;
    int pawnX5;
    int pawnZ5;
    int pawnX6;
    int pawnZ6;
    int pawnX7;
    int pawnZ7;
    int enemyPawnX0;
    int enemyPawnZ0;
    int enemyPawnX1;
    int enemyPawnZ1;
    int enemyPawnX2;
    int enemyPawnZ2;
    int enemyPawnX3;
    int enemyPawnZ3;
    int enemyPawnX4;
    int enemyPawnZ4;
    int enemyPawnX5;
    int enemyPawnZ5;
    int enemyPawnX6;
    int enemyPawnZ6;
    int enemyPawnX7;
    int enemyPawnZ7;

    private void Awake()
    {

    }

    void Start()
    {
    }
    private void InitializeVars()
    {
        //initialize Kings Positions
        kingX = -1;
        kingZ = -1;
        enemykingX = -1;
        enemykingZ = -1;

        //initialize Towers Positions
        rookX0 = -1;
        rookZ0 = -1;
        rookX1 = -1;
        rookZ1 = -1;
        enemyRookX0 = -1;
        enemyRookZ0 = -1;
        enemyRookX1 = -1;
        enemyRookZ1 = -1;
        //initialize knights positions
        horseX0 = -1;
        horseZ0 = -1;
        horseX1 = -1;
        horseZ1 = -1;
        enemyHorseX0 = -1;
        enemyHorseZ0 = -1;
        enemyHorseX1 = -1;
        enemyHorseZ1 = -1;
        //initialize bishops positions
        bishopX0 = -1;
        bishopZ0 = -1;
        bishopX1 = -1;
        bishopZ1 = -1;
        enemyBishopX0 = -1;
        enemyBishopZ0 = -1;
        enemyBishopX1 = -1;
        enemyBishopZ1 = -1;
        //initialize queens
        queenX = -1;
        queenZ = -1;
        enemyQueenX = -1;
        enemyQueenZ = -1;

        //extra queens
        extraQueenX0 = -1;
        extraQueenZ0 = -1;
        extraQueenX1 = -1;
        extraQueenZ1 = -1;
        extraQueenX2 = -1;
        extraQueenZ2 = -1;
        enemyExtraQueenX0 = -1;
        enemyExtraQueenZ0 = -1;
        enemyExtraQueenX1 = -1;
        enemyExtraQueenZ1 = -1;
        enemyExtraQueenX2 = -1;
        enemyExtraQueenZ2 = -1;
        //pawns  
        pawnX0 = -1;
        pawnZ0 = -1;
        pawnX1 = -1;
        pawnZ1 = -1;
        pawnX2 = -1;
        pawnZ2 = -1;
        pawnX3 = -1;
        pawnZ3 = -1;
        pawnX4 = -1;
        pawnZ4 = -1;
        pawnX5 = -1;
        pawnZ5 = -1;
        pawnX6 = -1;
        pawnZ6 = -1;
        pawnX7 = -1;
        pawnZ7 = -1;
        enemyPawnX0 = -1;
        enemyPawnZ0 = -1;
        enemyPawnX1 = -1;
        enemyPawnZ1 = -1;
        enemyPawnX2 = -1;
        enemyPawnZ2 = -1;
        enemyPawnX3 = -1;
        enemyPawnZ3 = -1;
        enemyPawnX4 = -1;
        enemyPawnZ4 = -1;
        enemyPawnX5 = -1;
        enemyPawnZ5 = -1;
        enemyPawnX6 = -1;
        enemyPawnZ6 = -1;
        enemyPawnX7 = -1;
        enemyPawnZ7 = -1;
    }
    public void getPiecesPositions()
    {
        InitializeVars();
        bool oneOwnTowerTook = false;
        bool oneOtherTowerTook = false;

        
        bool oneOwnHorseTook = false;
        bool oneOtherHorseTook = false;

       
        bool oneOwnBishopTook = false;
        bool oneOtherBishopTook = false;

        

        bool oneOwnQueenTook = false;
        bool oneOtherQueenTook = false;
        bool twoOtherQueenTook = false;
        bool twoOwnQueenTook = false;
        bool threeOwnQueenTook = false;
        bool threeOtherQueenTook = false;

        bool oneOwnPawnTook = false;
        bool oneOtherPawnTook = false;
        bool twoOwnPawnTook = false;
        bool twoOtherPawnTook = false;
        bool threeOwnPawnTook = false;
        bool threeOtherPawnTook = false;
        bool fourOwnPawnTook = false;
        bool fourOtherPawnTook = false;
        bool fiveOwnPawnTook = false;
        bool fiveOtherPawnTook = false;
        bool sixOwnPawnTook = false;
        bool sixOtherPawnTook = false;
        bool sevenOwnPawnTook = false;
        bool sevenOtherPawnTook = false;
        foreach (ChessPiece cp in boardCurrentlyPlaying.chessPieces)
        {
            if (cp != null)
            {
                if (cp.GetType() == typeof(KingPiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        kingX = cp.CurrentX;
                        kingZ = cp.CurrentZ;
                    }
                    else
                    {
                        enemykingX = cp.CurrentX;
                        enemykingZ = cp.CurrentZ;
                    }
                }
                else if (cp.GetType() == typeof(TowerPiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        if (oneOwnTowerTook)
                        {
                            rookX1 = cp.CurrentX;
                            rookZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            rookZ0 = cp.CurrentX;
                            rookZ0 = cp.CurrentZ;
                            oneOwnTowerTook = true;
                        }
                    }
                    else
                    {
                        if (oneOtherTowerTook)
                        {
                            enemyRookX1 = cp.CurrentX;
                            enemyRookZ1 = cp.CurrentZ;

                        }
                        else
                        {
                            enemyRookX0 = cp.CurrentX;
                            enemyRookZ0 = cp.CurrentZ;
                            oneOtherTowerTook = true;
                        }
                    }
                }
                else if (cp.GetType() == typeof(HorsePiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        if (oneOwnHorseTook)
                        {
                            horseX1 = cp.CurrentX;
                            horseZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            horseX0 = cp.CurrentX;
                            horseZ0 = cp.CurrentZ;
                            oneOwnHorseTook = true;
                        }
                    }
                    else
                    {
                        if (oneOtherHorseTook)
                        {
                            enemyHorseX1 = cp.CurrentX;
                            enemyHorseZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            enemyHorseX0 = cp.CurrentX;
                            enemyHorseZ0 = cp.CurrentZ;
                            oneOtherHorseTook = true;
                        }
                    }
                }
                else if (cp.GetType() == typeof(BishopPiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        if (oneOwnBishopTook)
                        {
                            bishopX1 = cp.CurrentX;
                            bishopZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            bishopX0 = cp.CurrentX;
                            bishopZ0 = cp.CurrentZ;
                            oneOwnBishopTook = true;
                        }
                    }
                    else
                    {
                        if (oneOtherBishopTook)
                        {
                            enemyBishopX1 = cp.CurrentX;
                            enemyBishopZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            enemyBishopX0 = cp.CurrentX;
                            enemyBishopZ0 = cp.CurrentZ;
                            oneOtherBishopTook = true;
                        }
                    }
                }
                else if (cp.GetType() == typeof(QueenPiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        
                        if (threeOwnQueenTook)
                        {
                            extraQueenX2 = cp.CurrentX;
                            extraQueenZ2 = cp.CurrentZ;
                        }
                        else if (twoOwnQueenTook)
                        {
                            extraQueenX1 = cp.CurrentX;
                            extraQueenZ1 = cp.CurrentZ;
                            threeOwnQueenTook = true;
                        }
                        else if (oneOwnQueenTook)
                        {
                            extraQueenX0 = cp.CurrentX;
                            extraQueenZ0 = cp.CurrentZ;
                            twoOwnQueenTook = true;
                        }
                        else
                        {
                            queenX = cp.CurrentX;
                            queenZ = cp.CurrentZ;
                            oneOwnQueenTook = true;
                        }
                    }
                    else
                    {
                        if (threeOtherQueenTook)
                        {
                            enemyExtraQueenX2 = cp.CurrentX;
                            enemyExtraQueenZ2 = cp.CurrentZ;
                        }
                        else if (twoOtherQueenTook)
                        {
                            enemyExtraQueenX1 = cp.CurrentX;
                            enemyExtraQueenZ1 = cp.CurrentZ;
                            threeOtherQueenTook = true;
                        }
                        else if (oneOtherQueenTook)
                        {
                            enemyExtraQueenX0 = cp.CurrentX;
                            enemyExtraQueenZ0 = cp.CurrentZ;
                            twoOtherQueenTook = true;
                        }
                        else
                        {
                            enemyQueenX = cp.CurrentX;
                            enemyQueenZ = cp.CurrentZ;
                            oneOtherQueenTook = true;

                        }
                    }
                }
                else if (cp.GetType() == typeof(PawnPiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        if (sevenOwnPawnTook)
                        {
                            pawnX7 = cp.CurrentX;
                            pawnZ7 = cp.CurrentZ;

                        }
                        else if (sixOwnPawnTook)
                        {
                            pawnX6 = cp.CurrentX;
                            pawnZ6 = cp.CurrentZ;
                            sevenOwnPawnTook = true;
                        }
                        else if (fiveOwnPawnTook)
                        {
                            pawnX5 = cp.CurrentX;
                            pawnZ5 = cp.CurrentZ;
                            sixOwnPawnTook = true;
                        }
                        else if (fourOwnPawnTook)
                        {
                            pawnX4 = cp.CurrentX;
                            pawnZ4 = cp.CurrentZ;
                            fiveOwnPawnTook = true;
                        }
                        else if (threeOwnPawnTook)
                        {
                            pawnX3 = cp.CurrentX;
                            pawnZ3 = cp.CurrentZ;
                            fourOwnPawnTook = true;
                        }
                        else if (twoOwnPawnTook)
                        {
                            pawnX2 = cp.CurrentX;
                            pawnZ2 = cp.CurrentZ;
                            threeOwnPawnTook = true;
                        }
                        else if (oneOwnPawnTook)
                        {
                            pawnX1 = cp.CurrentX;
                            pawnZ1 = cp.CurrentZ;
                            twoOwnPawnTook = true;
                        }
                        else
                        {
                            pawnX0 = cp.CurrentX;
                            pawnZ0 = cp.CurrentZ;
                            oneOwnPawnTook = true;
                        }
                    }
                    else
                    {
                        if (sevenOtherPawnTook)
                        {
                            enemyPawnX7 = cp.CurrentX;
                            enemyPawnZ7 = cp.CurrentZ;
                        }
                        else if (sixOtherPawnTook)
                        {
                            enemyPawnX6 = cp.CurrentX;
                            enemyPawnZ6 = cp.CurrentZ;
                            sevenOtherPawnTook = true;
                        }
                        else if (fiveOtherPawnTook)
                        {
                            enemyPawnX5 = cp.CurrentX;
                            enemyPawnZ5 = cp.CurrentZ;
                            sixOtherPawnTook = true;
                        }
                        else if (fourOtherPawnTook)
                        {
                            enemyPawnX4 = cp.CurrentX;
                            enemyPawnZ4 = cp.CurrentZ;
                            fiveOtherPawnTook = true;
                        }
                        else if (threeOtherPawnTook)
                        {
                            enemyPawnX3 = cp.CurrentX;
                            enemyPawnZ3 = cp.CurrentZ;
                            fourOtherPawnTook = true;
                        }
                        else if (twoOtherPawnTook)
                        {
                            enemyPawnX2 = cp.CurrentX;
                            enemyPawnZ2 = cp.CurrentZ;
                            threeOtherPawnTook = true;
                        }
                        else if (oneOtherPawnTook)
                        {
                            enemyPawnX1 = cp.CurrentX;
                            enemyPawnZ1 = cp.CurrentZ;
                            twoOtherPawnTook = true;
                        }
                        else
                        {
                            enemyPawnX0 = cp.CurrentX;
                            enemyPawnZ0 = cp.CurrentZ;
                            oneOtherPawnTook = true;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// method called just i dont get warning messages... i dont know fully how to do heuristic since i can not attach a key for each possible action
    /// </summary>
    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);
        
    }
    public override void OnEpisodeBegin()
    {
        episodeTime = 0.0f;
        base.OnEpisodeBegin();
        if(debug){ 
            Debug.Log("New Episode");
            //Debug.Log("isWhitePlayer: " + isWhitePlayer);
        }
       
        
    }
    void FixedUpdate()
    {
        validMove = false;
        
        episodeTime += Time.deltaTime;
        if (resetWithTime)
        {
            if (episodeTime > maxTimeForEpisode)
            {
                if (debug)
                {
                    if (isWhitePlayer)
                    {
                        Debug.Log("White took too much time");
                    }
                    else{
                        Debug.Log("Black took too much time");
                    }
                }
                boardCurrentlyPlaying.FinishGame(true);
            }

        }
    }
    public void endAgentEpisode()
    {
        if (debug)
        {
            Debug.Log("Final reward: " + GetCumulativeReward());
            if (isWhitePlayer)
            {
                Debug.Log("Episode ended for white player");
            }
            else
            {
                Debug.Log("Episode ended for black player");
            }
        }
        
        EndEpisode();
    }
    /// <summary>
    /// Observations of all the x and z position of all the pieces (including potencial queens)
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
            //according to vecotr size
            getPiecesPositions();
            sensor.AddObservation(isWhitePlayer);
            //kings +4
            sensor.AddObservation(kingX);
            sensor.AddObservation(kingZ);
            sensor.AddObservation(enemykingX);
            sensor.AddObservation(enemykingZ);

            //rooks +8 observations
            sensor.AddObservation(rookX0);
            sensor.AddObservation(rookZ0);
            sensor.AddObservation(enemyRookX0);
            sensor.AddObservation(enemyRookZ0);
            sensor.AddObservation(rookX1);
            sensor.AddObservation(rookZ1);
            sensor.AddObservation(enemyRookX1);
            sensor.AddObservation(enemyRookZ1);

            //horses +8 observations
            sensor.AddObservation(horseX0);
            sensor.AddObservation(horseZ0);
            sensor.AddObservation(enemyHorseX0);
            sensor.AddObservation(enemyHorseZ0);
            sensor.AddObservation(horseX1);
            sensor.AddObservation(horseZ1);
            sensor.AddObservation(enemyHorseX1);
            sensor.AddObservation(enemyHorseZ1);

            //bishops +8 observations
            sensor.AddObservation(bishopX0);
            sensor.AddObservation(bishopZ0);
            sensor.AddObservation(enemyBishopX0);
            sensor.AddObservation(enemyBishopZ0);
            sensor.AddObservation(bishopX1);
            sensor.AddObservation(bishopZ1);
            sensor.AddObservation(enemyBishopX1);
            sensor.AddObservation(enemyBishopZ1);

            //queen +4 observations
            sensor.AddObservation(queenX);
            sensor.AddObservation(queenZ);
            sensor.AddObservation(enemyQueenX);
            sensor.AddObservation(enemyQueenZ);

            //extra queens thta may show up +32 observations
            sensor.AddObservation(extraQueenX0);
            sensor.AddObservation(extraQueenZ0);
            sensor.AddObservation(extraQueenX1);
            sensor.AddObservation(extraQueenZ1);
            sensor.AddObservation(extraQueenX2);
            sensor.AddObservation(extraQueenZ2);
            sensor.AddObservation(enemyExtraQueenX0);
            sensor.AddObservation(enemyExtraQueenZ0);
            sensor.AddObservation(enemyExtraQueenX1);
            sensor.AddObservation(enemyExtraQueenZ1);
            sensor.AddObservation(enemyExtraQueenX2);
            sensor.AddObservation(enemyExtraQueenZ2);

            //pawns +32 observations
            sensor.AddObservation(pawnX0);
            sensor.AddObservation(pawnZ0);
            sensor.AddObservation(enemyPawnX0);
            sensor.AddObservation(enemyPawnZ0);
            sensor.AddObservation(pawnX1);
            sensor.AddObservation(pawnZ1);
            sensor.AddObservation(enemyPawnX1);
            sensor.AddObservation(enemyPawnZ1);
            sensor.AddObservation(pawnX2);
            sensor.AddObservation(pawnZ2);
            sensor.AddObservation(enemyPawnX2);
            sensor.AddObservation(enemyPawnZ2);
            sensor.AddObservation(pawnX3);
            sensor.AddObservation(pawnZ3);
            sensor.AddObservation(enemyPawnX3);
            sensor.AddObservation(enemyPawnZ3);
            sensor.AddObservation(pawnX4);
            sensor.AddObservation(pawnZ4);
            sensor.AddObservation(enemyPawnX4);
            sensor.AddObservation(enemyPawnZ4);
            sensor.AddObservation(pawnX5);
            sensor.AddObservation(pawnZ5);
            sensor.AddObservation(enemyPawnX5);
            sensor.AddObservation(enemyPawnZ5);
            sensor.AddObservation(pawnX6);
            sensor.AddObservation(pawnZ6);
            sensor.AddObservation(enemyPawnX6);
            sensor.AddObservation(enemyPawnZ6);
            sensor.AddObservation(pawnX7);
            sensor.AddObservation(pawnZ7);
            sensor.AddObservation(enemyPawnX7);
            sensor.AddObservation(enemyPawnZ7);

            //total observations:
            //96 observations - 10 (5 extra queens from each side removed) + iswhitePLayer=77
        
    }
    /// <summary>
    /// Function that permits to check if a certain move is possible
    /// it calls the Board Manager and checks if the piece on position (initialX,initialZ) can move to (toX,toZ)
    /// </summary>
    /// <param name="initialX">Current x piece position</param>
    /// <param name="initialZ">Current z piece position</param>
    /// <param name="toX">to where it should move on the x axis</param>
    /// <param name="toZ">to where it should move on the z axis</param>
    private void verifyMove(int initialX, int initialZ, int toX, int toZ)
    {
        //initialX and initialZ corresponds to the position of the particulkar piece that is trying toi move
        boardCurrentlyPlaying.allowedMoves = boardCurrentlyPlaying.chessPieces[initialX, initialZ].PossibleMove(boardCurrentlyPlaying);
        boardCurrentlyPlaying.activeChessPiece = boardCurrentlyPlaying.chessPieces[initialX, initialZ];
        //is it possible move there
        if (toX > -1 && toX < 8 && toZ > -1 && toZ < 8)
        {
            if (boardCurrentlyPlaying.allowedMoves[toX, toZ])
            {
                validMove = true;
                checkIfEatsPiece(toX, toZ);
                boardCurrentlyPlaying.MovePiece(initialX, initialZ, toX, toZ);


                float[,] tableKing = new float[8, 8];
                float[,] tableQueen = new float[8, 8];
                float[,] tableHorse = new float[8, 8];
                float[,] tableRook = new float[8, 8];
                float[,] tableBishop = new float[8, 8];
                float[,] tablePawn = new float[8, 8];
                if (isWhitePlayer)
                {
                    tableKing = tableKingWhite;
                    tableQueen = tableQueenWhite;
                    tableHorse = tableHorseWhite;
                    tableRook = tableRookWhite;
                    tableBishop = tableBishopWhite;
                    tablePawn = tablePawnWhite;
                }
                else
                {
                    tableKing = tableKingBlack;
                    tableQueen = tableQueenBlack;
                    tableHorse = tableHorseBlack;
                    tableRook = tableRookBlack;
                    tableBishop = tableBishopBlack;
                    tablePawn = tablePawnBlack;
                }
                ChessPiece movingPiece = boardCurrentlyPlaying.chessPieces[initialX, initialZ];
                if (movingPiece != null)
                {
                    if (movingPiece.GetType() == typeof(KingPiece))
                    {
                        tempReward += (strengthKing * tableKing[toX, toZ]);
                    }
                    else if (movingPiece.GetType() == typeof(TowerPiece))
                    {
                        tempReward += (strengthRook * tableRook[toX, toZ]);
                    }
                    else if (movingPiece.GetType() == typeof(HorsePiece))
                    {
                        tempReward += (strengthHorse * tableHorse[toX, toZ]);
                    }
                    else if (movingPiece.GetType() == typeof(BishopPiece))
                    {
                        tempReward += (strengthBishop * tableBishop[toX, toZ] );
                    }
                    else if (movingPiece.GetType() == typeof(QueenPiece))
                    {
                        tempReward += (strengthQueen * tableQueen[toX, toZ] );
                    }
                    else if (movingPiece.GetType() == typeof(PawnPiece))
                    {
                        tempReward += (strengthPawn * tablePawn[toX, toZ] );
                    }
                }

                tempReward += validAction;

                if (boardCurrentlyPlaying.chessPieces[initialX, initialZ]!=null && boardCurrentlyPlaying.chessPieces[initialX, initialZ].GetType() == typeof(PawnPiece))
                {
                    if (toZ == 7 || toZ == 0)
                    {
                        tempReward += incentiveToConvert;
                    }
                }

            }
            else
            {
                validMove = false;
                AddReward(invalidAction_or_doNothing);
            }
        }
        else
        {
            validMove = false;
            AddReward(invalidAction_or_doNothing);
        }
    }

    /// <summary>
    /// Behaviors functions that try to check and attribute if a piece of y type can do certain action given an action (number)
    /// </summary>
    //0 -> not move; 1-> left; 2->left+up; ...
    private void kingBehavior(float action, int king_X, int king_Z)
    {
        switch (action)
        {
            case 1:
                int toX = king_X - 1;
                int toZ = king_Z;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 2:
                toX = king_X - 1;
                toZ = king_Z + 1;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 3:
                toX = king_X;
                toZ = king_Z + 1;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 4:
                toX = king_X + 1;
                toZ = king_Z + 1;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 5:
                toX = king_X + 1;
                toZ = king_Z;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 6:
                toX = king_X + 1;
                toZ = king_Z - 1;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 7:
                toX = king_X;
                toZ = king_Z - 1;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 8:
                toX = king_X - 1;
                toZ = king_Z - 1;
                verifyMove(king_X, king_Z, toX, toZ);
                break;
            case 9:
                //castling left 
                toX = king_X - 3;
                toZ = king_Z;
                verifyMove(king_X, king_Z, toX, toZ);
                //training porposes
                AddReward(incentiveToCastling);
                break;
            case 10:
                //castling right 
                toX = king_X +2;
                toZ = king_Z;
                verifyMove(king_X, king_Z, toX, toZ);
                //training porposes
                AddReward(incentiveToCastling);
                break;
            default:
                if (debug)
                {
                    Debug.Log("No valid action on behavior");
                }
               
                break;
        }
    }
    //0 -> not move; 1-> left 1unit; 2->left 2 units; ...7-> left 7 units; 8->up 1 unit; ....
    private void towerBehavior(float action, int tower_X, int tower_Z)
    {
        if (tower_X != -1 && tower_Z != -1)
        {
            switch (action)
            {
                case 1:
                    int toX = tower_X - 1;
                    int toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 2:
                    toX = tower_X - 2;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 3:
                    toX = tower_X - 3;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 4:
                    toX = tower_X - 4;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 5:
                    toX = tower_X - 5;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 6:
                    toX = tower_X - 6;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 7:
                    toX = tower_X - 7;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 8:
                    toX = tower_X + 1;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 9:
                    toX = tower_X + 2;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 10:
                    toX = tower_X + 3;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 11:
                    toX = tower_X + 4;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 12:
                    toX = tower_X + 5;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 13:
                    toX = tower_X + 6;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 14:
                    toX = tower_X + 7;
                    toZ = tower_Z;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 15:
                    toX = tower_X;
                    toZ = tower_Z - 1;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 16:
                    toX = tower_X;
                    toZ = tower_Z - 2;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 17:
                    toX = tower_X;
                    toZ = tower_Z - 3;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 18:
                    toX = tower_X;
                    toZ = tower_Z - 4;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 19:
                    toX = tower_X;
                    toZ = tower_Z - 5;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 20:
                    toX = tower_X;
                    toZ = tower_Z - 6;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 21:
                    toX = tower_X;
                    toZ = tower_Z - 7;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 22:
                    toX = tower_X;
                    toZ = tower_Z + 1;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 23:
                    toX = tower_X;
                    toZ = tower_Z + 2;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 24:
                    toX = tower_X;
                    toZ = tower_Z + 3;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 25:
                    toX = tower_X;
                    toZ = tower_Z + 4;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 26:
                    toX = tower_X;
                    toZ = tower_Z + 5;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 27:
                    toX = tower_X;
                    toZ = tower_Z + 6;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                case 28:
                    toX = tower_X;
                    toZ = tower_Z + 7;
                    verifyMove(tower_X, tower_Z, toX, toZ);
                    break;
                default:
                    if (debug)
                    {
                        Debug.Log("No valid action on behavior");
                    }
                    break;
            }

        }
    }
    //0 -> not move; 1-> 2left+1up; 2->2up+1left; ...
    private void knightBehavior(float action, int knight_X, int knight_Z)
    {
        if (knight_X != -1 && knight_Z != -1)
        {
            switch (action)
            {
                case 1:
                    int toX = knight_X - 2;
                    int toZ = knight_Z + 1;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 2:
                    toX = knight_X - 1;
                    toZ = knight_Z + 2;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 3:
                    toX = knight_X + 1;
                    toZ = knight_Z + 2;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 4:
                    toX = knight_X + 2;
                    toZ = knight_Z + 1;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 5:
                    toX = knight_X + 2;
                    toZ = knight_Z - 1;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 6:
                    toX = knight_X + 1;
                    toZ = knight_Z - 2;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 7:
                    toX = knight_X - 1;
                    toZ = knight_Z - 2;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                case 8:
                    toX = knight_X - 2;
                    toZ = knight_Z - 1;
                    verifyMove(knight_X, knight_Z, toX, toZ);
                    break;
                default:
                    if (debug)
                    {
                        Debug.Log("No valid action on behavior");
                    }
                    break;
            }
        }
        
    }
    //0 -> not move; 1-> 1left+1up; 2->2up+2left; ...
    private void bishopBehavior(float action, int bishop_X, int bishop_Z)
    {
        if (bishop_X != -1 && bishop_Z != -1)
        {
            switch (action)
            {
                case 1:
                    int toX = bishop_X - 1;
                    int toZ = bishop_Z + 1;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 2:
                    toX = bishop_X - 2;
                    toZ = bishop_Z + 2;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 3:
                    toX = bishop_X - 3;
                    toZ = bishop_Z + 3;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 4:
                    toX = bishop_X - 4;
                    toZ = bishop_Z + 4;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 5:
                    toX = bishop_X - 5;
                    toZ = bishop_Z + 5;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 6:
                    toX = bishop_X - 6;
                    toZ = bishop_Z + 6;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 7:
                    toX = bishop_X - 7;
                    toZ = bishop_Z + 7;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 8:
                    toX = bishop_X + 1;
                    toZ = bishop_Z + 1;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 9:
                    toX = bishop_X + 2;
                    toZ = bishop_Z + 2;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 10:
                    toX = bishop_X + 3;
                    toZ = bishop_Z + 3;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 11:
                    toX = bishop_X + 4;
                    toZ = bishop_Z + 4;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 12:
                    toX = bishop_X + 5;
                    toZ = bishop_Z + 5;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 13:
                    toX = bishop_X + 6;
                    toZ = bishop_Z + 6;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 14:
                    toX = bishop_X + 7;
                    toZ = bishop_Z + 7;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 15:
                    toX = bishop_X + 1;
                    toZ = bishop_Z - 1;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 16:
                    toX = bishop_X + 2;
                    toZ = bishop_Z - 2;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 17:
                    toX = bishop_X + 3;
                    toZ = bishop_Z - 3;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 18:
                    toX = bishop_X + 4;
                    toZ = bishop_Z - 4;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 19:
                    toX = bishop_X + 5;
                    toZ = bishop_Z - 5;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 20:
                    toX = bishop_X + 6;
                    toZ = bishop_Z - 6;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 21:
                    toX = bishop_X + 7;
                    toZ = bishop_Z - 7;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 22:
                    toX = bishop_X - 1;
                    toZ = bishop_Z - 1;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 23:
                    toX = bishop_X - 2;
                    toZ = bishop_Z - 2;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 24:
                    toX = bishop_X - 3;
                    toZ = bishop_Z - 3;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 25:
                    toX = bishop_X - 4;
                    toZ = bishop_Z - 4;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 26:
                    toX = bishop_X - 5;
                    toZ = bishop_Z - 5;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 27:
                    toX = bishop_X - 6;
                    toZ = bishop_Z - 6;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                case 29:
                    toX = bishop_X - 7;
                    toZ = bishop_Z - 7;
                    verifyMove(bishop_X, bishop_Z, toX, toZ);
                    break;
                default:
                    if (debug)
                    {
                        Debug.Log("No valid action on behavior");
                    }
                    break;
            }
        }
    }
    //fisrt 28 are the tower movement and the rest corresponds to the bishop
    private void queenBehavior(float action, int queen_X, int queen_Z)
    {
        if (queen_X != -1 && queen_Z != -1)
        {
            switch (action)
            {
                case 1:
                    int toX = queen_X - 1;
                    int toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 2:
                    toX = queen_X - 2;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 3:
                    toX = queen_X - 3;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 4:
                    toX = queen_X - 4;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 5:
                    toX = queen_X - 5;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 6:
                    toX = queen_X - 6;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 7:
                    toX = queen_X - 7;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 8:
                    toX = queen_X + 1;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 9:
                    toX = queen_X + 2;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 10:
                    toX = queen_X + 3;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 11:
                    toX = queen_X + 4;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 12:
                    toX = queen_X + 5;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 13:
                    toX = queen_X + 6;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 14:
                    toX = queen_X + 7;
                    toZ = queen_Z;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 15:
                    toX = queen_X;
                    toZ = queen_Z - 1;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 16:
                    toX = queen_X;
                    toZ = queen_Z - 2;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 17:
                    toX = queen_X;
                    toZ = queen_Z - 3;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 18:
                    toX = queen_X;
                    toZ = queen_Z - 4;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 19:
                    toX = queen_X;
                    toZ = queen_Z - 5;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 20:
                    toX = queen_X;
                    toZ = queen_Z - 6;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 21:
                    toX = queen_X;
                    toZ = queen_Z - 7;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 22:
                    toX = queen_X;
                    toZ = queen_Z + 1;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 23:
                    toX = queen_X;
                    toZ = queen_Z + 2;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 24:
                    toX = queen_X;
                    toZ = queen_Z + 3;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 25:
                    toX = queen_X;
                    toZ = queen_Z + 4;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 26:
                    toX = queen_X;
                    toZ = queen_Z + 5;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 27:
                    toX = queen_X;
                    toZ = queen_Z + 6;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 28:
                    toX = queen_X;
                    toZ = queen_Z + 7;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 29:
                    toX = queen_X - 1;
                    toZ = queen_Z + 1;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 30:
                    toX = queen_X - 2;
                    toZ = queen_Z + 2;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 31:
                    toX = queen_X - 3;
                    toZ = queen_Z + 3;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 32:
                    toX = queen_X - 4;
                    toZ = queen_Z + 4;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 33:
                    toX = queen_X - 5;
                    toZ = queen_Z + 5;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 34:
                    toX = queen_X - 6;
                    toZ = queen_Z + 6;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 35:
                    toX = queen_X - 7;
                    toZ = queen_Z + 7;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 36:
                    toX = queen_X + 1;
                    toZ = queen_Z + 1;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 37:
                    toX = queen_X + 2;
                    toZ = queen_Z + 2;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 38:
                    toX = queen_X + 3;
                    toZ = queen_Z + 3;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 39:
                    toX = queen_X + 4;
                    toZ = queen_Z + 4;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 40:
                    toX = queen_X + 5;
                    toZ = queen_Z + 5;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 41:
                    toX = queen_X + 6;
                    toZ = queen_Z + 6;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 42:
                    toX = queen_X + 7;
                    toZ = queen_Z + 7;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 43:
                    toX = queen_X + 1;
                    toZ = queen_Z - 1;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 44:
                    toX = queen_X + 2;
                    toZ = queen_Z - 2;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 45:
                    toX = queen_X + 3;
                    toZ = queen_Z - 3;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 46:
                    toX = queen_X + 4;
                    toZ = queen_Z - 4;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 47:
                    toX = queen_X + 5;
                    toZ = queen_Z - 5;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 48:
                    toX = queen_X + 6;
                    toZ = queen_Z - 6;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 49:
                    toX = queen_X + 7;
                    toZ = queen_Z - 7;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 50:
                    toX = queen_X - 1;
                    toZ = queen_Z - 1;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 51:
                    toX = queen_X - 2;
                    toZ = queen_Z - 2;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 52:
                    toX = queen_X - 3;
                    toZ = queen_Z - 3;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 53:
                    toX = queen_X - 4;
                    toZ = queen_Z - 4;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 54:
                    toX = queen_X - 5;
                    toZ = queen_Z - 5;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 55:
                    toX = queen_X - 6;
                    toZ = queen_Z - 6;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                case 56:
                    toX = queen_X - 7;
                    toZ = queen_Z - 7;
                    verifyMove(queen_X, queen_Z, toX, toZ);
                    break;
                default:
                    if (debug)
                    {
                        Debug.Log("No valid action on behavior");
                    }
                    break;
            }
        }
    }
    //0 -> not move; 1-> 1up; 2->2up+; 3-> eat left and up;....
    private void pawnBehavior(float action, int pawn_X, int pawn_Z)
    {
        switch (action)
        {
            case 1:
                int toX = -1;
                int toZ = -1;
                if (isWhitePlayer)
                {
                    toX = pawn_X;
                    toZ= pawn_Z + 1;
                }
                else
                {
                    toX = pawn_X;
                    toZ = pawn_Z - 1;
                }
                verifyMove(pawn_X, pawn_Z, toX, toZ);
                break;
            case 2:
                if (isWhitePlayer)
                {
                    toX = pawn_X;
                    toZ = pawn_Z + 2;
                }
                else
                {
                    toX = pawn_X;
                    toZ = pawn_Z - 2;
                }
                verifyMove(pawn_X, pawn_Z, toX, toZ);
                break;
            case 3:
                if (isWhitePlayer)
                {
                    toX = pawn_X - 1;
                    toZ = pawn_Z + 1;
                }
                else
                {
                    toX = pawn_X - 1;
                    toZ = pawn_Z - 1;
                }
                verifyMove(pawn_X, pawn_Z, toX, toZ);
                break;
            case 4:
                if (isWhitePlayer)
                {
                    toX = pawn_X + 1;
                    toZ = pawn_Z + 1;
                }
                else
                {
                    toX = pawn_X + 1;
                    toZ = pawn_Z - 1;
                }
                verifyMove(pawn_X, pawn_Z, toX, toZ);
                break;
            default:
                if (debug)
                {
                    Debug.Log("No valid action on behavior");
                }
                break;
        }
    }

    /// <summary>
    /// 3rd iteration:
    ///  branch 0 with all the possibilities
    ///     -> king behavior:          10  different possibilities
    ///     -> tower behavior:         28  different possibilities * 2
    ///     -> horse behavior:         8   different possibilities * 2
    ///     -> bishop behavior:        28  different possibilities * 2
    ///     -> queen behavior:         56  different possibilities 
    ///     -> pawn behavior:          4   different possibilities * 8
    ///     -> extra queen behavior:   56  different possibilities * 3
    /// </summary>
    public override void OnActionReceived(float[] vectorAction)
    {
        
        if (boardCurrentlyPlaying.isWhiteTurn == isWhitePlayer)
        {
            if (debug)
            {
                Debug.Log("Action received: " + vectorAction[0]);

            }
            //number max of the generated action is the max on the last condition+1 ->675 updtae: 394
            //king behavior:            10  different possibilities
            if (vectorAction[0] >= 0 && vectorAction[0] <= 9)
            {   
                if(kingX != -1 && kingZ != -1 && boardCurrentlyPlaying.hasOnePossibleMove(kingX, kingZ))
                {
                    kingBehavior(vectorAction[0]+1, kingX, kingZ);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                   
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //tower 0 behavior:         28  different possibilities
            else if (vectorAction[0] >= 10 && vectorAction[0] <= 37)
            {
                if (rookX0 != -1 && rookZ0 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(rookX0, rookZ0))
                {
                    towerBehavior(vectorAction[0] - 9, rookX0, rookZ0);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //tower 1 behavior:         28  different possibilities
            else if (vectorAction[0] >= 38 && vectorAction[0] <= 65)
            {
                if (rookX1 != -1 && rookZ1 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(rookX1, rookZ1))
                {
                    towerBehavior(vectorAction[0] - 37, rookX1, rookZ1);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //horse 0 behavior:         8  different possibilities
            else if (vectorAction[0] >= 66 && vectorAction[0] <= 73)
            {
                if (horseX0 != -1 && horseZ0 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(horseX0, horseZ0))
                {
                    knightBehavior(vectorAction[0] - 65, horseX0, horseZ0);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //horse 1 behavior:         8  different possibilities
            else if (vectorAction[0] >= 74 && vectorAction[0] <= 81)
            {
                if (horseX1 != -1 && horseZ1 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(horseX1, horseZ1))
                {
                    knightBehavior(vectorAction[0] - 73, horseX1, horseZ1);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //bishop 0 behavior:        28  different possibilities
            else if (vectorAction[0] >= 82 && vectorAction[0] <= 109)
            {
                if (bishopX0 != -1 && bishopZ0 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(bishopX0, bishopZ0))
                {
                    bishopBehavior(vectorAction[0] - 81, bishopX0, bishopZ0);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //bishop 1 behavior:        28  different possibilities
            else if (vectorAction[0] >= 110 && vectorAction[0] <= 137)
            {
                if (bishopX1 != -1 && bishopZ1 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(bishopX1, bishopZ1))
                {
                    bishopBehavior(vectorAction[0] - 109, bishopX1, bishopZ1);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //queen behavior:           56  different possibilities
            else if (vectorAction[0] >= 138 && vectorAction[0] <= 193)
            {
                if (queenX != -1 && queenZ != -1 && boardCurrentlyPlaying.hasOnePossibleMove(queenX, queenZ))
                {
                    queenBehavior(vectorAction[0] - 137, queenX, queenZ);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 194 && vectorAction[0] <= 197)
            {
                if (pawnX0 != -1 && pawnZ0 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX0, pawnZ0))
                {
                    pawnBehavior(vectorAction[0] - 193, pawnX0, pawnZ0);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 198 && vectorAction[0] <= 201)
            {
                if (pawnX1 != -1 && pawnZ1 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX1, pawnZ1))
                {
                    pawnBehavior(vectorAction[0] - 197, pawnX1, pawnZ1);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 202 && vectorAction[0] <= 205)
            {
                if (pawnX2 != -1 && pawnZ2 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX2, pawnZ2))
                {
                    pawnBehavior(vectorAction[0] - 201, pawnX2, pawnZ2);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 206 && vectorAction[0] <= 209)
            {
                if (pawnX3 != -1 && pawnZ3 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX3, pawnZ3))
                {
                    pawnBehavior(vectorAction[0] - 205, pawnX3, pawnZ3);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 210 && vectorAction[0] <= 213)
            {
                if (pawnX4 != -1 && pawnZ4 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX4, pawnZ4))
                {
                    pawnBehavior(vectorAction[0] - 209, pawnX4, pawnZ4);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 214 && vectorAction[0] <= 217)
            {
                if (pawnX5 != -1 && pawnZ5 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX5, pawnZ5))
                {
                    pawnBehavior(vectorAction[0] - 213, pawnX5, pawnZ5);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 218 && vectorAction[0] <= 221)
            {
                if (pawnX6 != -1 && pawnZ6 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX6, pawnZ6))
                {
                    pawnBehavior(vectorAction[0] - 217, pawnX6, pawnZ6);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 222 && vectorAction[0] <= 225)
            {
                if (pawnX7 != -1 && pawnZ7 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(pawnX7, pawnZ7))
                {
                    pawnBehavior(vectorAction[0] - 221, pawnX7, pawnZ7);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 226 && vectorAction[0] <= 281)
            {
                if (extraQueenX0 != -1 && extraQueenZ0 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(extraQueenX0, extraQueenZ0))
                {
                    queenBehavior(vectorAction[0] - 225, extraQueenX0, extraQueenZ0);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 282 && vectorAction[0] <= 337)
            {
                if (extraQueenX1 != -1 && extraQueenZ1 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(extraQueenX1, extraQueenZ1))
                {
                    queenBehavior(vectorAction[0] - 281, extraQueenX1, extraQueenZ1);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 338 && vectorAction[0] <= 393)
            {
                if (extraQueenX2 != -1 && extraQueenZ2 != -1 && boardCurrentlyPlaying.hasOnePossibleMove(extraQueenX2, extraQueenZ2))
                {
                    queenBehavior(vectorAction[0] - 337, extraQueenX2, extraQueenZ2);
                }
                else
                {
                    if (debug)
                    {
                        Debug.Log("not valid move");
                    }
                    validMove = false;
                    AddReward(invalidAction_or_doNothing);
                }
            }


            //evaluate positioning of the pieces on the board
            if (validMove)
            {
                AddReward(tempReward);
                tempReward = 0.0f;
            }


            if (debug)
            {
                //Debug.Log("isWhitePlayer: " + isWhitePlayer);
                //Debug.Log("turnIsWhite?: " + boardCurrentlyPlaying.isWhiteTurn);
                Debug.Log("Reward: " + GetCumulativeReward());

                //Debug.Log("---------------------------------------");
            }
        }

        
    }
    /// <summary>
    /// checks if certain movement will eat a piece so we can reward the agent
    /// </summary>
    public void checkIfEatsPiece(int toX, int toZ)
    {
        if (toX > -1 && toZ > -1)
        {
            ChessPiece enemyPiece = boardCurrentlyPlaying.chessPieces[toX, toZ];
            if (enemyPiece != null)
            {
                if (enemyPiece.GetType() == typeof(KingPiece))
                {
                   //gj
                }
                else if (enemyPiece.GetType() == typeof(TowerPiece))
                {
                    AddReward(strengthRook*10);
                }
                else if (enemyPiece.GetType() == typeof(HorsePiece))
                {
                    AddReward(strengthHorse * 10);
                }
                else if (enemyPiece.GetType() == typeof(BishopPiece))
                {
                    AddReward(strengthBishop * 10);
                }
                else if (enemyPiece.GetType() == typeof(QueenPiece))
                {
                    AddReward(strengthQueen * 10);
                }
                else if (enemyPiece.GetType() == typeof(PawnPiece))
                {
                    AddReward(strengthPawn * 10);
                }
            }

        }
    }
    /// <summary>
    /// simple functiont to verify if the agent lost
    /// </summary>
    /// <returns></returns>
    
}
