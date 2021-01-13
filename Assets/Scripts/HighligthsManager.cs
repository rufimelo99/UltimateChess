using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighligthsManager : MonoBehaviour
{
    public static HighligthsManager Instance { set; get; }
    /// <summary>
    /// Both Prefabs here are the tiles that change visually representing the selected piece (OwnHighlight) and the possible moves (highlightObject)
    /// </summary>
    public GameObject highlightObject;
    public GameObject OwnHighlight;
    [HideInInspector]
    public List<GameObject> highlights;
    private float offsetX = 0;
    private float offsetY = 0;
    private float offsetZ = 0;
    private BoardManager board;
    void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();

        BoardManager board = gameObject.GetComponent<BoardManager>();
        offsetX = board.offsetX;
        offsetY = board.offsetY;
        offsetZ = board.offsetZ;
    }
    /// <summary>
    /// Following 2 functions simply returns the prefabs
    /// </summary>
    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null) {
            go = Instantiate(highlightObject);
            highlights.Add(go);
        }
        return go;
    }
    private GameObject GetHighlightObjectforOwnTile()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(OwnHighlight);
            highlights.Add(go);
        }
        return go;
    }
    /// <summary>
    /// function that permits to highlight the possible moves on the board
    /// </summary>
    public void AllowedMovesHighlight(bool[,] moves) { 
        for (int x =0; x<8; x++)
        {
            for (int z = 0; z < 8; z++)
            {
                if (moves[x, z]) 
                {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    //slightly above on y to be above board tiles
                    go.transform.position = new Vector3(x + offsetX + BoardManager.Instance.TILE_SIZE/2, 0.27f+offsetY,z + offsetZ+ BoardManager.Instance.TILE_SIZE/2);
                }
            }
        }
    }
    /// <summary>
    /// function that permits to highlight the selected tile on the board
    /// </summary>
    public void ownTileHighligth(int x, int z)
    {
        GameObject go = GetHighlightObjectforOwnTile();
        go.SetActive(true);
        go.transform.position = new Vector3(x+ offsetX + BoardManager.Instance.TILE_SIZE/2, 0.27f + offsetY, z + offsetZ+ BoardManager.Instance.TILE_SIZE/2);
    }
    /// <summary>
    /// function that removes all highlights
    /// </summary>
    public void RemoveHighlights()
    {
       foreach(GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }


}
