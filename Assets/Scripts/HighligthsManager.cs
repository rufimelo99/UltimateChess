﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighligthsManager : MonoBehaviour
{
    public static HighligthsManager Instance { set; get; }
    public GameObject highlightObject;
    public GameObject OwnHighlight;
    public List<GameObject> highlights;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
    }

    // Update is called once per frame
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
                    //BoardManager.Instance.TILESIZE; ??
                    go.transform.position = new Vector3(x+ BoardManager.Instance.TILE_SIZE /2, 0.27f,z + BoardManager.Instance.TILE_SIZE / 2);
                }
            }
        }
    }
    public void ownTileHighligth(int x, int z)
    {
        GameObject go = GetHighlightObjectforOwnTile();
        go.SetActive(true);
        go.transform.position = new Vector3(x + BoardManager.Instance.TILE_SIZE / 2, 0.27f, z + BoardManager.Instance.TILE_SIZE / 2);
    }
    public void RemoveHighlights()
    {
       foreach(GameObject go in highlights)
        {
            go.SetActive(false);

        }
    }
}
