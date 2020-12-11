using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.1f;

    private int CurrentTileX = -1;
    private int CurrentTileZ = -1;

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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane"))) {
            //where did the collision happened
            Debug.Log(hit.point);
        }  
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
    }
}
