using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tile{
    
    public enum Region { None, Wilderness, City, Farmland };
    public enum Building { None, Resource, Housing, Capital, Barracks};
   // public List<Tile> adjacentTiles = new List<Tile>();
    public List<Pair> adjacentTileCoords = new List<Pair>();
    bool occupied;
    public Region region;
    Building building;
    public Pair coordinate;

    override
    public String ToString() {
        return ("Tile " + "(" + coordinate.first + "," + coordinate.second + ")");
    }
    public Tile() {
        region = Region.None;
        building = Building.None;
        coordinate = null;
    }
    public Tile(Pair coords) {
        region = Region.None;
        building = Building.None;
        coordinate = coords;
    }
    public Tile(Region region, Building building, List<Pair> adjTilesCoords) {
        this.region = region;
        this.building = building;
        adjacentTileCoords = adjTilesCoords;
    }
    public Tile(Region region, Building building) {
        this.region = region;
        this.building = building;
        //adjacentTiles is empty
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
