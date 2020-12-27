using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class ChessAgent : Agent
{

    public List<ChessPiece> OwnPieces { set; get; }

    public bool isWhitePlayer;
    
    //Reward values
    const float invalidAction = -1.0f;
    const float validAction = 0.1f;
    const float wonGame = 100.0f;
    const float lostGame = -100.0f;
    const float ateRook = 5.0f;
    const float ateKnight = 5.0f;
    const float ateBishop = 5.0f;
    const float ateQueen = 20.0f;
    const float atePawn = 1.0f;
    const float rookPieceMissing = -0.05f;
    const float knightPieceMissing = -0.05f;
    const float bishopPieceMissing = -0.05f;
    const float pawnPieceMissing = -0.001f;
    const float QueenPieceMissing = -0.2f;

    //Kings Positions
    int kingX;
    int kingZ;
    int otherkingX;
    int otherkingZ;

    //Towers Positions
    int rookX0;
    int rookZ0;
    int otherRookX0;
    int otherRookZ0;
    int rookX1;
    int rookZ1;
    int otherRookX1;
    int otherRookZ1;


    //Horses/Knights Positions
    int horseX0;
    int horseZ0;
    int otherHorseX0;
    int otherHorseZ0;
    int horseX1;
    int horseZ1;
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


    //need to add eventualPieces: an extra queen ( or two just to be sure)
    int extraQueenX0;
    int extraQueenZ0;
    int otherExtraQueenX0;
    int otherExtraQueenZ0;
    int extraQueenX1;
    int extraQueenZ1;
    int otherExtraQueenX1;
    int otherExtraQueenZ1;

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
        otherRookX0 = -1;
        otherRookZ0 = -1;
        rookX1 = -1;
        rookZ1 = -1;
        otherRookX1 = -1;
        otherRookZ1 = -1;
        bool oneOwnTowerTook = false;
        bool oneOtherTowerTook = false;

        //initialize knights positions
        horseX0 = -1;
        horseZ0 = -1;
        otherHorseX0 = -1;
        otherHorseZ0 = -1;
        horseX1 = -1;
        horseZ1 = -1;
        otherHorseX1 = -1;
        otherHorseZ1 = -1;
        bool oneOwnHorseTook = false;
        bool oneOtherHorseTook = false;


        //initialize bishops positions
        bishopX0 = -1;
        bishopZ0 = -1;
        otherBishopX0 = -1;
        otherBishopZ0 = -1;
        bishopX1 = -1;
        bishopZ1 = -1;
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
        extraQueenZ0 =-1;
        otherExtraQueenX0 = -1;
        otherExtraQueenZ0=-1;
        extraQueenX1 = -1;
        extraQueenZ1 =-1;
        otherExtraQueenX1 = -1;
        otherExtraQueenZ1 =-1;
        bool oneOwnQueenTook = false;
        bool twoOwnQueenTook = false;
        bool oneOtherQueenTook = false;
        bool twoOtherQueenTook = false;


        //pawns   -.-
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
                        if (twoOwnQueenTook)
                        {
                            extraQueenX1 = cp.CurrentX;
                            extraQueenZ1 = cp.CurrentZ;

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
                        if (twoOtherQueenTook)
                        {
                            otherExtraQueenX1 = cp.CurrentX;
                            otherExtraQueenZ1 = cp.CurrentZ;

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

    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);
    }
    public override void OnEpisodeBegin()
    {
        //Debug.Log("Begining of a new episode");
    }

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

        //extra queens thta may show up +8 observations
        sensor.AddObservation(extraQueenX0);
        sensor.AddObservation(extraQueenZ0);
        sensor.AddObservation(otherExtraQueenX0);
        sensor.AddObservation(otherExtraQueenZ0);
        sensor.AddObservation(extraQueenX1);
        sensor.AddObservation(extraQueenZ1);
        sensor.AddObservation(otherExtraQueenX1);
        sensor.AddObservation(otherExtraQueenZ1);

        //pawns +32observations
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
        //72 observations
    }

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
            }
            else
            {
                AddReward(invalidAction);
                //RequestDecision();
            }

        }
        else
        {
            AddReward(invalidAction);
           //RequestDecision();
        }
    }

    //behaviours
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
                //TODO 
                //implement
                AddReward(-1.0f);
                break;
            case 10:
                //castling right 
                //TODO 
                //implement
                AddReward(-1.0f);
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

    //fisrt 29 are the tower movement and the rest corresponds to the bishop
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
    /// The structure for the branches is:
    ///     branch 0        -> king behaviour:          11  different possibilities
    ///     branch 1 & 2    -> tower behaviour:         29  different possibilities
    ///     branch 3 & 4    -> horse behaviour:         9   different possibilities
    ///     branch 5 & 6    -> bishop behaviour:        29  different possibilities
    ///     branch 7        -> queen behaviour:         57  different possibilities
    ///     branch 8 & 9    -> extra queen behaviour:   57  different possibilities
    ///     branches 10-17  -> pawn behaviour:          5   different possibilities
    /// </summary>

    public override void OnActionReceived(float[] vectorAction)
    {
        if (BoardManager.Instance.isWhiteTurn == isWhitePlayer)
        {
            // Perform actions based on a vector of numbers
            
            //movement of each pawn
            if (vectorAction[10] != 0 && pawnX0 != -1 && pawnZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX0, pawnZ0))
            {
                pawnBehaviour(vectorAction[10], pawnX0, pawnZ0);
            }
            else if (vectorAction[11] != 0 && pawnX1 != -1 && pawnZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX1, pawnZ1))
            {
                pawnBehaviour(vectorAction[11], pawnX1, pawnZ1);
            }
            else if (vectorAction[12] != 0 && pawnX2 != -1 && pawnZ2 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX2, pawnZ2))
            {
                pawnBehaviour(vectorAction[12], pawnX2, pawnZ2);
            }
            else if (vectorAction[13] != 0 && pawnX3 != -1 && pawnZ3 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX3, pawnZ3))
            {
                pawnBehaviour(vectorAction[13], pawnX3, pawnZ3);
            }
            else if (vectorAction[14] != 0 && pawnX4 != -1 && pawnZ4 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX4, pawnZ4))
            {
                pawnBehaviour(vectorAction[14], pawnX4, pawnZ4);
            }
            else if (vectorAction[15] != 0 && pawnX5 != -1 && pawnZ5 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX5, pawnZ5))
            {
                pawnBehaviour(vectorAction[15], pawnX5, pawnZ5);
            }
            else if (vectorAction[16] != 0 && pawnX6 != -1 && pawnZ6 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX6, pawnZ6))
            {
                pawnBehaviour(vectorAction[16], pawnX6, pawnZ6);
            }
            else if (vectorAction[17] != 0 && pawnX7 != -1 && pawnZ7 != -1 && BoardManager.Instance.hasOnePossibleMove(pawnX7, pawnZ7))
            {
                pawnBehaviour(vectorAction[17], pawnX7, pawnZ7);
            }
            
            //vectorAction[1] can reflect the movement of the Rook 0
            else if (vectorAction[1] != 0 && rookX0 != -1 && rookZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(rookX0, rookZ0))
            {
                towerBehaviour(vectorAction[1], rookX0, rookZ0);
            }
            //vectorAction[2] can reflect the movement of the Rook 1
            else if (vectorAction[2] != 0 && rookX1 != -1 && rookZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(rookX1, rookZ1))
            {
                towerBehaviour(vectorAction[2], rookX1, rookZ1);
            }
            //vectorAction[3] can reflect the movement of the hrose/knight 0
            else if (vectorAction[3] != 0 && horseX0 != -1 && horseZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(horseX0, horseZ0))
            {
                knightBehaviour(vectorAction[3], horseX0, horseZ0);
            }
            //vectorAction[4] can reflect the movement of the hrose/knight 1
            else if (vectorAction[4] != 0 && horseX1 != -1 && horseZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(horseX1, horseZ1))
            {
                knightBehaviour(vectorAction[4], horseX1, horseZ1);
            }
            //vectorAction[5] can reflect the movement of the bishop 0
            else if (vectorAction[5] != 0 && bishopX0 != -1 && bishopZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(bishopX0, bishopZ0))
            {
                bishopBehaviour(vectorAction[5], bishopX0, bishopZ0);
            }
            //vectorAction[6] can reflect the movement of the bishop 1
            else if (vectorAction[6] != 0 && bishopX1 != -1 && bishopZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(bishopX1, bishopZ1))
            {
                bishopBehaviour(vectorAction[6], bishopX1, bishopZ1);
            }
            //vectorAction[7] can reflect the movement of the queeen
            else if (vectorAction[7] != 0 && queenX != -1 && queenZ != -1 && BoardManager.Instance.hasOnePossibleMove(queenX, queenZ))
            {
                queenBehaviour(vectorAction[7], queenX, queenZ, false);
            }
            //vectorAction[8] can reflect the movement of the extra queeen
            else if (vectorAction[8] != 0 && extraQueenX0 != -1 && extraQueenZ0 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX0, extraQueenZ0))
            {
                queenBehaviour(vectorAction[8], extraQueenX0, extraQueenZ0, true);
            }
            //vectorAction[9] can reflect the movement of the extra extra queeen
            else if (vectorAction[9] != 0 && extraQueenX1 != -1 && extraQueenZ1 != -1 && BoardManager.Instance.hasOnePossibleMove(extraQueenX1, extraQueenZ1))
            {
                queenBehaviour(vectorAction[9], extraQueenX1, extraQueenZ1, true);
            }//vectorAction[0] can reflect the movement of the King
            else if (vectorAction[0] != 0 && kingX != -1 && kingZ != -1 && BoardManager.Instance.hasOnePossibleMove(kingX, kingZ))
            {
                kingBehaviour(vectorAction[0], kingX, kingZ);
            }
        }

    }

    public void AgentLost()
    {
        AddReward(lostGame);
        EndEpisode();
    }

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
    
    
    //no logic behind this-> PURE RANDOM
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
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (possibleMov[i, j])
                        {
                            mov.Add(new int[] { i, j });
                        }
                    }

                }
                //there is a possible movement for the piece
                if (mov.Count > 0)
                {
                    int indexMovement = Random.Range(0, mov.Count);

                    int toX = mov[indexMovement][0];
                    int toZ = mov[indexMovement][1];
                    BoardManager.Instance.MovePiece(x, z, toX, toZ);



                    actionExecuted = true;
                }
                break;

            }

        }
    }


}
