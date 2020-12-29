using System.Collections;
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
    /// <summary>
    /// since this game interacts with two agents, i needed to reward when a king would be eaten
    /// </summary>
    private bool KingeatenNextMove = false;
    /// <summary>
    /// Reward Values.. changed over time
    /// -actions
    /// -eat pieces
    /// -penalize for missing pieces ? (except extra queens)
    /// little incentive to castling since it is a powerful move
    /// </summary>
    const float invalidAction = -0.1f;   //try for now
    const float validAction = 0.1f;
    const float wonGame = 100.0f;
    const float lostGame = -100.0f;
    const float doNothing = -0.1f;
    
    const float ateRook = 5.0f;
    const float ateKnight = 5.0f;
    const float ateBishop = 5.0f;
    const float ateQueen = 20.0f;
    const float atePawn = 1.0f;

    const float rookPieceMissing = 0.0f;
    const float knightPieceMissing = 0.0f;
    const float bishopPieceMissing = 0.0f;
    const float pawnPieceMissing = 0.0f;
    const float QueenPieceMissing = 0.0f;

    const float incentiveToCastling = 0.1f;

    //Kings Positions
    int kingX;
    int kingZ;
    int otherkingX;
    int otherkingZ;

    //Towers Positions
    int rookX0;
    int rookZ0;
    int rookX1;
    int rookZ1;
    int otherRookX0;
    int otherRookZ0;
    int otherRookX1;
    int otherRookZ1;

    //Horses/Knights Positions
    int horseX0;
    int horseZ0;
    int horseX1;
    int horseZ1;
    int otherHorseX0;
    int otherHorseZ0;
    int otherHorseX1;
    int otherHorseZ1;

    //bishops Positions
    int bishopX0;
    int bishopZ0;
    int otherBishopX0;
    int otherBishopZ0;
    int bishopX1;
    int bishopZ1;
    int otherBishopX1;
    int otherBishopZ1;

    //queen positions
    int queenX;
    int queenZ;
    int otherQueenX;
    int otherQueenZ;

    //need to add eventual Pieces: extra queens->extreme case 8 extra queens from each +player
    int extraQueenX0;
    int extraQueenZ0;
    int extraQueenX1;
    int extraQueenZ1;
    int extraQueenX2;
    int extraQueenZ2;
    int extraQueenX3;
    int extraQueenZ3;
    int extraQueenX4;
    int extraQueenZ4;
    int extraQueenX5;
    int extraQueenZ5;
    int extraQueenX6;
    int extraQueenZ6;
    int extraQueenX7;
    int extraQueenZ7;
    int otherExtraQueenX0;
    int otherExtraQueenZ0;
    int otherExtraQueenX1;
    int otherExtraQueenZ1;
    int otherExtraQueenX2;
    int otherExtraQueenZ2;
    int otherExtraQueenX3;
    int otherExtraQueenZ3;
    int otherExtraQueenX4;
    int otherExtraQueenZ4;
    int otherExtraQueenX5;
    int otherExtraQueenZ5;
    int otherExtraQueenX6;
    int otherExtraQueenZ6;
    int otherExtraQueenX7;
    int otherExtraQueenZ7;

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
    int otherPawnX0;
    int otherPawnZ0;
    int otherPawnX1;
    int otherPawnZ1;
    int otherPawnX2;
    int otherPawnZ2;
    int otherPawnX3;
    int otherPawnZ3;
    int otherPawnX4;
    int otherPawnZ4;
    int otherPawnX5;
    int otherPawnZ5;
    int otherPawnX6;
    int otherPawnZ6;
    int otherPawnX7;
    int otherPawnZ7;

    public void getPiecesPositions()
    {
        //initialize Kings Positions
        kingX = -1;
        kingZ = -1;
        otherkingX = -1;
        otherkingZ = -1;

        //initialize Towers Positions
        rookX0 = -1;
        rookZ0 = -1;
        rookX1 = -1;
        rookZ1 = -1;
        otherRookX0 = -1;
        otherRookZ0 = -1;
        otherRookX1 = -1;
        otherRookZ1 = -1;
        bool oneOwnTowerTook = false;
        bool oneOtherTowerTook = false;

        //initialize knights positions
        horseX0 = -1;
        horseZ0 = -1;
        horseX1 = -1;
        horseZ1 = -1;
        otherHorseX0 = -1;
        otherHorseZ0 = -1;
        otherHorseX1 = -1;
        otherHorseZ1 = -1;
        bool oneOwnHorseTook = false;
        bool oneOtherHorseTook = false;

        //initialize bishops positions
        bishopX0 = -1;
        bishopZ0 = -1;
        bishopX1 = -1;
        bishopZ1 = -1;
        otherBishopX0 = -1;
        otherBishopZ0 = -1;
        otherBishopX1 = -1;
        otherBishopZ1 = -1;
        bool oneOwnBishopTook = false;
        bool oneOtherBishopTook = false;

        //initialize queens
        queenX = -1;
        queenZ = -1;
        otherQueenX = -1;
        otherQueenZ = -1;

        //extra queens
        extraQueenX0 = -1;
        extraQueenZ0 = -1;
        extraQueenX1 = -1;
        extraQueenZ1 = -1;
        extraQueenX2 = -1;
        extraQueenZ2 = -1;
        extraQueenX3 = -1;
        extraQueenZ3 = -1;
        extraQueenX4 = -1;
        extraQueenZ4 = -1;
        extraQueenX5 = -1;
        extraQueenZ5 = -1;
        extraQueenX6 = -1;
        extraQueenZ6 = -1;
        extraQueenX7 = -1;
        extraQueenZ7 = -1;
        otherExtraQueenX0 = -1;
        otherExtraQueenZ0 = -1;
        otherExtraQueenX1 = -1;
        otherExtraQueenZ1 = -1;
        otherExtraQueenX2 = -1;
        otherExtraQueenZ2 = -1;
        otherExtraQueenX3 = -1;
        otherExtraQueenZ3 = -1;
        otherExtraQueenX4 = -1;
        otherExtraQueenZ4 = -1;
        otherExtraQueenX5 = -1;
        otherExtraQueenZ5 = -1;
        otherExtraQueenX6 = -1;
        otherExtraQueenZ6 = -1;
        otherExtraQueenX7 = -1;
        otherExtraQueenZ7 = -1;
        bool oneOwnQueenTook = false;
        bool twoOwnQueenTook = false;
        bool threeOwnQueenTook = false;
        bool fourOwnQueenTook = false;
        bool fiveOwnQueenTook = false;
        bool sixOwnQueenTook = false;
        bool sevenOwnQueenTook = false;
        bool eightOwnQueenTook = false;
        bool oneOtherQueenTook = false;
        bool twoOtherQueenTook = false;
        bool threeOtherQueenTook = false;
        bool fourOtherQueenTook = false;
        bool fiveOtherQueenTook = false;
        bool sixOtherQueenTook = false;
        bool sevenOtherQueenTook = false;
        bool eightOtherQueenTook = false; 

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
        otherPawnX0 = -1;
        otherPawnZ0 = -1;
        otherPawnX1 = -1;
        otherPawnZ1 = -1;
        otherPawnX2 = -1;
        otherPawnZ2 = -1;
        otherPawnX3 = -1;
        otherPawnZ3 = -1;
        otherPawnX4 = -1;
        otherPawnZ4 = -1;
        otherPawnX5 = -1;
        otherPawnZ5 = -1;
        otherPawnX6 = -1;
        otherPawnZ6 = -1;
        otherPawnX7 = -1;
        otherPawnZ7 = -1;
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
        foreach (ChessPiece cp in BoardManager.Instance.chessPieces)
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
                        otherkingX = cp.CurrentX;
                        otherkingZ = cp.CurrentZ;
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
                            otherRookX1 = cp.CurrentX;
                            otherRookZ1 = cp.CurrentZ;

                        }
                        else
                        {
                            otherRookX0 = cp.CurrentX;
                            otherRookZ0 = cp.CurrentZ;
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
                            otherHorseX1 = cp.CurrentX;
                            otherHorseZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            otherHorseX0 = cp.CurrentX;
                            otherHorseZ0 = cp.CurrentZ;
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
                            otherBishopX1 = cp.CurrentX;
                            otherBishopZ1 = cp.CurrentZ;
                        }
                        else
                        {
                            otherBishopX0 = cp.CurrentX;
                            otherBishopZ0 = cp.CurrentZ;
                            oneOtherBishopTook = true;
                        }
                    }
                }
                else if (cp.GetType() == typeof(QueenPiece))
                {
                    if (cp.isWhite == isWhitePlayer)
                    {
                        if (eightOwnQueenTook)
                        {
                            extraQueenX7 = cp.CurrentX;
                            extraQueenZ7 = cp.CurrentZ;
                        }
                        else if (sevenOwnQueenTook)
                        {
                            extraQueenX6 = cp.CurrentX;
                            extraQueenZ6 = cp.CurrentZ;
                            eightOwnQueenTook = true;
                        }
                        else if (sixOwnQueenTook)
                        {
                            extraQueenX5 = cp.CurrentX;
                            extraQueenZ5 = cp.CurrentZ;
                            sevenOwnQueenTook = true;
                        }
                        else if (fiveOwnQueenTook)
                        {
                            extraQueenX4 = cp.CurrentX;
                            extraQueenZ4 = cp.CurrentZ;
                            sixOwnQueenTook = true;
                        }
                        else if (fourOwnQueenTook)
                        {
                            extraQueenX3 = cp.CurrentX;
                            extraQueenZ3 = cp.CurrentZ;
                            fiveOwnQueenTook = true;
                        }
                        else if (threeOwnQueenTook)
                        {
                            extraQueenX2 = cp.CurrentX;
                            extraQueenZ2 = cp.CurrentZ;
                            fourOwnQueenTook = true;
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
                        if (eightOtherQueenTook)
                        {
                            otherExtraQueenX7 = cp.CurrentX;
                            otherExtraQueenZ7 = cp.CurrentZ;

                        }
                        else if (sevenOtherQueenTook)
                        {
                            otherExtraQueenX6 = cp.CurrentX;
                            otherExtraQueenZ6 = cp.CurrentZ;
                            eightOtherQueenTook = true;
                        }
                        else if (sixOtherQueenTook)
                        {
                            otherExtraQueenX5 = cp.CurrentX;
                            otherExtraQueenZ5 = cp.CurrentZ;
                            sevenOtherQueenTook = true;
                        }
                        else if (fiveOtherQueenTook)
                        {
                            otherExtraQueenX4 = cp.CurrentX;
                            otherExtraQueenZ4 = cp.CurrentZ;
                            sixOtherQueenTook = true;
                        }
                        else if (fourOtherQueenTook)
                        {
                            otherExtraQueenX3 = cp.CurrentX;
                            otherExtraQueenZ3 = cp.CurrentZ;
                            fiveOtherQueenTook = true;
                        }
                        else if (threeOtherQueenTook)
                        {
                            otherExtraQueenX2 = cp.CurrentX;
                            otherExtraQueenZ2 = cp.CurrentZ;
                            fourOtherQueenTook = true;
                        }
                        else if (twoOtherQueenTook)
                        {
                            otherExtraQueenX1 = cp.CurrentX;
                            otherExtraQueenZ1 = cp.CurrentZ;
                            threeOtherQueenTook = true;
                        }
                        else if (oneOtherQueenTook)
                        {

                            otherExtraQueenX0 = cp.CurrentX;
                            otherExtraQueenZ0 = cp.CurrentZ;
                            twoOtherQueenTook = true;

                        }
                        else
                        {
                            otherQueenX = cp.CurrentX;
                            otherQueenZ = cp.CurrentZ;
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
                            otherPawnX7 = cp.CurrentX;
                            otherPawnZ7 = cp.CurrentZ;
                        }
                        else if (sixOtherPawnTook)
                        {
                            otherPawnX6 = cp.CurrentX;
                            otherPawnZ6 = cp.CurrentZ;
                            sevenOtherPawnTook = true;
                        }
                        else if (fiveOtherPawnTook)
                        {
                            otherPawnX5 = cp.CurrentX;
                            otherPawnZ5 = cp.CurrentZ;
                            sixOtherPawnTook = true;
                        }
                        else if (fourOtherPawnTook)
                        {
                            otherPawnX4 = cp.CurrentX;
                            otherPawnZ4 = cp.CurrentZ;
                            fiveOtherPawnTook = true;
                        }
                        else if (threeOtherPawnTook)
                        {
                            otherPawnX3 = cp.CurrentX;
                            otherPawnZ3 = cp.CurrentZ;
                            fourOtherPawnTook = true;
                        }
                        else if (twoOtherPawnTook)
                        {
                            otherPawnX2 = cp.CurrentX;
                            otherPawnZ2 = cp.CurrentZ;
                            threeOtherPawnTook = true;
                        }
                        else if (oneOtherPawnTook)
                        {
                            otherPawnX1 = cp.CurrentX;
                            otherPawnZ1 = cp.CurrentZ;
                            twoOtherPawnTook = true;
                        }
                        else
                        {
                            otherPawnX0 = cp.CurrentX;
                            otherPawnZ0 = cp.CurrentZ;
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
        KingeatenNextMove = false;
    }
    /// <summary>
    /// Observations of all the x and z position of all the pieces (including potencial queens)
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //according to vecotr size
        getPiecesPositions();

        //kings +4
        sensor.AddObservation(kingX);
        sensor.AddObservation(kingZ);
        sensor.AddObservation(otherkingX);
        sensor.AddObservation(otherkingZ);

        //rooks +8 observations
        sensor.AddObservation(rookX0);
        sensor.AddObservation(rookZ0);
        sensor.AddObservation(otherRookX0);
        sensor.AddObservation(otherRookZ0);
        sensor.AddObservation(rookX1);
        sensor.AddObservation(rookZ1);
        sensor.AddObservation(otherRookX1);
        sensor.AddObservation(otherRookZ1);

        //horses +8 observations
        sensor.AddObservation(horseX0);
        sensor.AddObservation(horseZ0);
        sensor.AddObservation(otherHorseX0);
        sensor.AddObservation(otherHorseZ0);
        sensor.AddObservation(horseX1);
        sensor.AddObservation(horseZ1);
        sensor.AddObservation(otherHorseX1);
        sensor.AddObservation(otherHorseZ1);

        //bishops +8 observations
        sensor.AddObservation(bishopX0);
        sensor.AddObservation(bishopZ0);
        sensor.AddObservation(otherBishopX0);
        sensor.AddObservation(otherBishopZ0);
        sensor.AddObservation(bishopX1);
        sensor.AddObservation(bishopZ1);
        sensor.AddObservation(otherBishopX1);
        sensor.AddObservation(otherBishopZ1);

        //queen +4 observations
        sensor.AddObservation(queenX);
        sensor.AddObservation(queenZ);
        sensor.AddObservation(otherQueenX);
        sensor.AddObservation(otherQueenZ);

        //extra queens thta may show up +32 observations
        sensor.AddObservation(extraQueenX0);
        sensor.AddObservation(extraQueenZ0);
        sensor.AddObservation(extraQueenX1);
        sensor.AddObservation(extraQueenZ1);
        sensor.AddObservation(extraQueenX2);
        sensor.AddObservation(extraQueenZ2);
        sensor.AddObservation(extraQueenX3);
        sensor.AddObservation(extraQueenZ3);
        sensor.AddObservation(extraQueenX4);
        sensor.AddObservation(extraQueenZ4);
        sensor.AddObservation(extraQueenX5);
        sensor.AddObservation(extraQueenZ5);
        sensor.AddObservation(extraQueenX6);
        sensor.AddObservation(extraQueenZ6);
        sensor.AddObservation(extraQueenX7);
        sensor.AddObservation(extraQueenZ7);
        sensor.AddObservation(otherExtraQueenX0);
        sensor.AddObservation(otherExtraQueenZ0);
        sensor.AddObservation(otherExtraQueenX1);
        sensor.AddObservation(otherExtraQueenZ1);
        sensor.AddObservation(otherExtraQueenX2);
        sensor.AddObservation(otherExtraQueenZ2);
        sensor.AddObservation(otherExtraQueenX3);
        sensor.AddObservation(otherExtraQueenZ3);
        sensor.AddObservation(otherExtraQueenX4);
        sensor.AddObservation(otherExtraQueenZ4);
        sensor.AddObservation(otherExtraQueenX5);
        sensor.AddObservation(otherExtraQueenZ5);
        sensor.AddObservation(otherExtraQueenX6);
        sensor.AddObservation(otherExtraQueenZ6);
        sensor.AddObservation(otherExtraQueenX7);
        sensor.AddObservation(otherExtraQueenZ7);

        //pawns +32 observations
        sensor.AddObservation(pawnX0);
        sensor.AddObservation(pawnZ0);
        sensor.AddObservation(otherPawnX0);
        sensor.AddObservation(otherPawnZ0);
        sensor.AddObservation(pawnX1);
        sensor.AddObservation(pawnZ1);
        sensor.AddObservation(otherPawnX1);
        sensor.AddObservation(otherPawnZ1);
        sensor.AddObservation(pawnX2);
        sensor.AddObservation(pawnZ2);
        sensor.AddObservation(otherPawnX2);
        sensor.AddObservation(otherPawnZ2);
        sensor.AddObservation(pawnX3);
        sensor.AddObservation(pawnZ3);
        sensor.AddObservation(otherPawnX3);
        sensor.AddObservation(otherPawnZ3);
        sensor.AddObservation(pawnX4);
        sensor.AddObservation(pawnZ4);
        sensor.AddObservation(otherPawnX4);
        sensor.AddObservation(otherPawnZ4);
        sensor.AddObservation(pawnX5);
        sensor.AddObservation(pawnZ5);
        sensor.AddObservation(otherPawnX5);
        sensor.AddObservation(otherPawnZ5);
        sensor.AddObservation(pawnX6);
        sensor.AddObservation(pawnZ6);
        sensor.AddObservation(otherPawnX6);
        sensor.AddObservation(otherPawnZ6);
        sensor.AddObservation(pawnX7);
        sensor.AddObservation(pawnZ7);
        sensor.AddObservation(otherPawnX7);
        sensor.AddObservation(otherPawnZ7);

        //total observations:
        //96 observations
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
        BoardManager.Instance.allowedMoves = BoardManager.Instance.chessPieces[initialX, initialZ].PossibleMove();
        BoardManager.Instance.activeChessPiece = BoardManager.Instance.chessPieces[initialX, initialZ];
        //is it possible move there
        if (toX > -1 && toX < 8 && toZ > -1 && toZ < 8)
        {
            if (BoardManager.Instance.allowedMoves[toX, toZ])
            {
                checkIfEatsPiece(toX, toZ);
                BoardManager.Instance.MovePiece(initialX, initialZ, toX, toZ);
                AddReward(validAction);
                if (KingeatenNextMove)
                {
                    EndEpisode();
                    //BoardManager.Instance.ResetGame();
                }
            }
            else
            {
                AddReward(invalidAction);
            }
        }
        else
        {
            AddReward(invalidAction);
        }
    }

    /// <summary>
    /// Behaviors functions that try to check and attribute if a piece of y type can do certain action given an action (number)
    /// </summary>
    //0 -> not move; 1-> left; 2->left+up; ...
    private void kingBehaviour(float action, int king_X, int king_Z)
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
                break;
        }
    }
    //0 -> not move; 1-> left 1unit; 2->left 2 units; ...7-> left 7 units; 8->up 1 unit; ....
    private void towerBehaviour(float action, int tower_X, int tower_Z)
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
                    break;
            }

        }
        else
        {
            AddReward(rookPieceMissing);
        }
    }
    //0 -> not move; 1-> 2left+1up; 2->2up+1left; ...
    private void knightBehaviour(float action, int knight_X, int knight_Z)
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
                    break;
            }
        }
        else
        {
            AddReward(knightPieceMissing);
        }
    }
    //0 -> not move; 1-> 1left+1up; 2->2up+2left; ...
    private void bishopBehaviour(float action, int bishop_X, int bishop_Z)
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
                    break;
            }
        }
        else
        {
            AddReward(bishopPieceMissing);
        }
    }
    //fisrt 28 are the tower movement and the rest corresponds to the bishop
    private void queenBehaviour(float action, int queen_X, int queen_Z, bool isExtra)
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
                    break;
            }
        }
        else
        {
            //if is extra, do not penalize... itherwise it would be being penalized each iteration even in the begining of the game
            if (!isExtra)
            {
                AddReward(QueenPieceMissing);
            }
        }
    }
    //0 -> not move; 1-> 1up; 2->2up+; 3-> eat left and up;....
    private void pawnBehaviour(float action, int pawn_X, int pawn_Z)
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
                break;
        }
    }

    /// <summary>
    /// it still misses the en passant movcement and castling
    /// The intial structure for the branches was:
    ///     branch 0        -> king behavior:          11  different possibilities
    ///     branch 1 & 2    -> tower behavior:         29  different possibilities
    ///     branch 3 & 4    -> horse behavior:         9   different possibilities
    ///     branch 5 & 6    -> bishop behavior:        29  different possibilities
    ///     branch 7        -> queen behavior:         57  different possibilities
    ///     branch 8-15     -> extra queen behavior:   57  different possibilities
    ///     branches 16-23  -> pawn behavior:          5   different possibilities
    ///    
    /// 2nd iteration:
    ///  branch 0 with all the possibilities
    ///     -> king behavior:          10  different possibilities
    ///     -> tower behavior:         28  different possibilities * 2
    ///     -> horse behavior:         8   different possibilities * 2
    ///     -> bishop behavior:        28  different possibilities * 2
    ///     -> queen behavior:         56  different possibilities 
    ///     -> pawn behavior:          4   different possibilities * 8
    ///     -> extra queen behavior:   56  different possibilities * 8
    /// </summary>

    public override void OnActionReceived(float[] vectorAction)
    {
        if (verifyIsLost())
        {
            EndEpisode();
            BoardManager.Instance.ResetGame();
        }
        if (BoardManager.Instance.isWhiteTurn == isWhitePlayer)
        {
            //number max of the generated action is the max on the last condition+1 ->675
            //king behavior:            10  different possibilities
            if (vectorAction[0] >= 0 && vectorAction[0] <= 9)
            {   
                if(kingX != -1 && kingZ != -1 && BoardManager.Instance.hasOnePossibleMove(kingX, kingZ))
                {
                    kingBehaviour(vectorAction[0]+1, kingX, kingZ);
                }
            }
            //tower 0 behavior:         28  different possibilities
            else if (vectorAction[0] >= 10 && vectorAction[0] <= 37)
            {
                if (rookX0 != -1 && rookZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(rookX0, rookZ0))
                {
                    towerBehaviour(vectorAction[0] - 9, rookX0, rookZ0);
                }
            }
            //tower 1 behavior:         28  different possibilities
            else if (vectorAction[0] >= 38 && vectorAction[0] <= 65)
            {
                if (rookX1 != -1 && rookZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(rookX1, rookZ1))
                {
                    towerBehaviour(vectorAction[0] - 37, rookX1, rookZ1);
                }
            }
            //horse 0 behavior:         8  different possibilities
            else if (vectorAction[0] >= 66 && vectorAction[0] <= 73)
            {
                if (horseX0 != -1 && horseZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(horseX0, horseZ0))
                {
                    knightBehaviour(vectorAction[0] - 65, horseX0, horseZ0);
                }
            }
            //horse 1 behavior:         8  different possibilities
            else if (vectorAction[0] >= 74 && vectorAction[0] <= 81)
            {
                if (horseX1 != -1 && horseZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(horseX1, horseZ1))
                {
                    knightBehaviour(vectorAction[0] - 73, horseX1, horseZ1);
                }
            }
            //bishop 0 behavior:        28  different possibilities
            else if (vectorAction[0] >= 82 && vectorAction[0] <= 109)
            {
                if (bishopX0 != -1 && bishopZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(bishopX0, bishopZ0))
                {
                    bishopBehaviour(vectorAction[0] - 84, bishopX0, bishopZ0);
                }
            }
            //bishop 1 behavior:        28  different possibilities
            else if (vectorAction[0] >= 110 && vectorAction[0] <= 137)
            {
                if (bishopX1 != -1 && bishopZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(bishopX1, bishopZ1))
                {
                    bishopBehaviour(vectorAction[0] - 109, bishopX1, bishopZ1);
                }
            }
            //queen behavior:           56  different possibilities
            else if (vectorAction[0] >= 138 && vectorAction[0] <= 193)
            {
                if (queenX != -1 && queenZ != -1 && BoardManager.Instance.hasOnePossibleMove(queenX, queenZ))
                {
                    queenBehaviour(vectorAction[0] - 137, queenX, queenZ, false);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 194 && vectorAction[0] <= 197)
            {
                if (pawnX0 != -1 && pawnZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX0, pawnZ0))
                {
                    pawnBehaviour(vectorAction[0] - 193, pawnX0, pawnZ0);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 198 && vectorAction[0] <= 201)
            {
                if (pawnX1 != -1 && pawnZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX1, pawnZ1))
                {
                    pawnBehaviour(vectorAction[0] - 197, pawnX1, pawnZ1);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 202 && vectorAction[0] <= 205)
            {
                if (pawnX2 != -1 && pawnZ2 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX2, pawnZ2))
                {
                    pawnBehaviour(vectorAction[0] - 201, pawnX2, pawnZ2);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 206 && vectorAction[0] <= 209)
            {
                if (pawnX3 != -1 && pawnZ3 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX3, pawnZ3))
                {
                    pawnBehaviour(vectorAction[0] - 205, pawnX3, pawnZ3);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 210 && vectorAction[0] <= 213)
            {
                if (pawnX4 != -1 && pawnZ4 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX4, pawnZ4))
                {
                    pawnBehaviour(vectorAction[0] - 209, pawnX4, pawnZ4);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 214 && vectorAction[0] <= 217)
            {
                if (pawnX5 != -1 && pawnZ5 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX5, pawnZ5))
                {
                    pawnBehaviour(vectorAction[0] - 213, pawnX5, pawnZ5);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 218 && vectorAction[0] <= 221)
            {
                if (pawnX6 != -1 && pawnZ6 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX6, pawnZ6))
                {
                    pawnBehaviour(vectorAction[0] - 217, pawnX6, pawnZ6);
                }
            }
            //pawn behavior:            4  different possibilities
            else if (vectorAction[0] >= 222 && vectorAction[0] <= 225)
            {
                if (pawnX7 != -1 && pawnZ7 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX7, pawnZ7))
                {
                    pawnBehaviour(vectorAction[0] - 221, pawnX7, pawnZ7);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 226 && vectorAction[0] <= 281)
            {
                if (extraQueenX0 != -1 && extraQueenZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX0, extraQueenZ0))
                {
                    queenBehaviour(vectorAction[0] - 225, extraQueenX0, extraQueenZ0, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 282 && vectorAction[0] <= 337)
            {
                if (extraQueenX1 != -1 && extraQueenZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX1, extraQueenZ1))
                {
                    queenBehaviour(vectorAction[0] - 281, extraQueenX1, extraQueenZ1, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 338 && vectorAction[0] <= 393)
            {
                if (extraQueenX2 != -1 && extraQueenZ2 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX2, extraQueenZ2))
                {
                    queenBehaviour(vectorAction[0] - 337, extraQueenX2, extraQueenZ2, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 394 && vectorAction[0] <= 449)
            {
                if (extraQueenX3 != -1 && extraQueenZ3 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX3, extraQueenZ3))
                {
                    queenBehaviour(vectorAction[0] - 393, extraQueenX3, extraQueenZ3, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 450 && vectorAction[0] <= 505)
            {
                if (extraQueenX4 != -1 && extraQueenZ4 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX4, extraQueenZ4))
                {
                    queenBehaviour(vectorAction[0] - 449, extraQueenX4, extraQueenZ4, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 506 && vectorAction[0] <= 561)
            {
                if (extraQueenX5 != -1 && extraQueenZ5 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX5, extraQueenZ5))
                {
                    queenBehaviour(vectorAction[0] - 505, extraQueenX5, extraQueenZ5, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 562 && vectorAction[0] <= 617)
            {
                if (extraQueenX6 != -1 && extraQueenZ6 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX6, extraQueenZ6))
                {
                    queenBehaviour(vectorAction[0] - 561, extraQueenX6, extraQueenZ6, true);
                }
            }
            //extra queen behavior:     56  different possibilities
            else if (vectorAction[0] >= 618 && vectorAction[0] <= 674)
            {
                if (extraQueenX7 != -1 && extraQueenZ7 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX7, extraQueenZ7))
                {
                    queenBehaviour(vectorAction[0] - 617, extraQueenX7, extraQueenZ7, true);
                }
            }
        }
        AddReward(doNothing/ MaxStep);
    }
    /// <summary>
    /// checks if certain movement will eat a piece so we can reward the agent
    /// </summary>
    public void checkIfEatsPiece(int toX, int toZ)
    {
        if (toX > -1 && toZ > -1)
        {
            ChessPiece enemyPiece = BoardManager.Instance.chessPieces[toX, toZ];
            if (enemyPiece != null)
            {
                if (enemyPiece.GetType() == typeof(KingPiece))
                {
                    AddReward(wonGame);
                    KingeatenNextMove = true;
                }
                else if (enemyPiece.GetType() == typeof(TowerPiece))
                {
                    AddReward(ateRook);
                }
                else if (enemyPiece.GetType() == typeof(HorsePiece))
                {
                    AddReward(ateKnight);
                }
                else if (enemyPiece.GetType() == typeof(BishopPiece))
                {
                    AddReward(ateBishop);
                }
                else if (enemyPiece.GetType() == typeof(QueenPiece))
                {
                    AddReward(ateQueen);
                }
                else if (enemyPiece.GetType() == typeof(PawnPiece))
                {
                    AddReward(atePawn);
                }
            }

        }
    }
    /// <summary>
    /// simple functiont to verify if the agent lost
    /// </summary>
    /// <returns></returns>
    public bool verifyIsLost()
    {
        if (!(kingX != -1 && kingZ != -1))
        {
            AddReward(lostGame);
            EndEpisode();
            return true;
        }
        return false;
    }
}
