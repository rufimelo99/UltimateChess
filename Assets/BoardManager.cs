using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float TILE_SIZE = 1.0f;

    //Tile Selected atm
    private int CurrentTileX = -1;
    private int CurrentTileZ = -1;

    //pieces in the board
    public List<GameObject> chessPieces = new List<GameObject>();   //only initialized so i dont have an error on the console (it would run either way)
    public GameObject activeChessPiece;

    private void Start()
    {
        //Spawn Black Towers
        SpawnPiece(0, GetTileCenter(0, 7));
        SpawnPiece(7, GetTileCenter(7, 7));
        //Spawn Black Horses
        SpawnPiece(1, GetTileCenter(1, 7));
        SpawnPiece(6, GetTileCenter(6, 7));
        //Spawn Black Bishops
        SpawnPiece(2, GetTileCenter(2, 7));
        SpawnPiece(5, GetTileCenter(5, 7));
        //Spawn Black Queen and King
        SpawnPiece(3, GetTileCenter(3, 7));
        SpawnPiece(4, GetTileCenter(4, 7));
        //Spawn Black Pawns
        SpawnPiece(8, GetTileCenter(0, 6));
        SpawnPiece(9, GetTileCenter(1, 6));
        SpawnPiece(10, GetTileCenter(2, 6));
        SpawnPiece(11, GetTileCenter(3, 6));
        SpawnPiece(12, GetTileCenter(4, 6));
        SpawnPiece(13, GetTileCenter(5, 6));
        SpawnPiece(14, GetTileCenter(6, 6));
        SpawnPiece(15, GetTileCenter(7, 6));

        //Spawn White Pawns
        SpawnPiece(16, GetTileCenter(0, 1));
        SpawnPiece(17, GetTileCenter(1, 1));
        SpawnPiece(18, GetTileCenter(2, 1));
        SpawnPiece(19, GetTileCenter(3, 1));
        SpawnPiece(20, GetTileCenter(4, 1));
        SpawnPiece(21, GetTileCenter(5, 1));
        SpawnPiece(22, GetTileCenter(6, 1));
        SpawnPiece(23, GetTileCenter(7, 1));

        //Spawn Black Towers
        SpawnPiece(24, GetTileCenter(0, 0));
        SpawnPiece(31, GetTileCenter(7, 0));
        //Spawn Black Horses           
        SpawnPiece(25, GetTileCenter(1, 0));
        SpawnPiece(30, GetTileCenter(6, 0));
        //Spawn Black Bishops          
        SpawnPiece(26, GetTileCenter(2, 0));
        SpawnPiece(29, GetTileCenter(5, 0));
        //Spawn Black Queen and King   
        SpawnPiece(27, GetTileCenter(3, 0));
        SpawnPiece(28, GetTileCenter(4, 0));

    }
    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        Draw();
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
            Debug.Log(hit.point);
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
    private void SpawnPiece(int index, Vector3 position) {
        GameObject go = Instantiate(chessPieces[index], position, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        activeChessPiece=go;
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
                    Vector3.forward * (CurrentTileZ + 1) + Vector3.right * (CurrentTileX + 1)
                );
        }

    }
}
