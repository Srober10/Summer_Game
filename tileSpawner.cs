using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pair {
    public int first;
    public int second;
    public Pair(int first, int sec) {
        this.first = first;
        second = sec;
    }
    public string toString() {
        return "" + first + "," + second;
    }

    public bool EqualsCheck(Pair p) {
        if (p.first == first && p.second == second) {
            return true;
        }
        else {
            return false;
        }
    }

    public Vector3 DistanceVector() {
        // return Mathf.Sqrt(first^2 + second^2);
        return new Vector3(first, second, 1);
    }
}

public class tileSpawner : MonoBehaviour{     

    //attributes
    public int dimension =3; //dimensions of the square (x by x)
    public float tileSize = 0.9f; //not yet used, set scale in inspector to < 1 to leave gaps
    public GameObject prefabTile;
  //  GameObject[] tiles ;    
    Tile[,] tileBoard;
    List<Pair> freePoints ;
    GameObject[,] gameTiles;
    Transform holder;
    

    // Awake is called before Start and the first frame update
    void Awake()
    {
        holder = this.transform;
        intialiseFreePoints();        
        intialiseBoard();
        initialiseGameTiles();       
  
        AllocateRegions(0.2f, 0.1f, 0.3f, 0f); //params
      

    }
    //called on object instanitation via unity, after awake
    private void Start() {
        UpdateGameObjectArray(); //has to be done after all the tiles are set 
        
    }

    private void initialiseGameTiles() {
        gameTiles = new GameObject[dimension, dimension];
        //creates a background tile, where z must be greater than other tiles so that they appear infront
        var backgroundTile = Instantiate(prefabTile, tileBoard[Mathf.RoundToInt(dimension / 2), Mathf.RoundToInt(dimension / 2)].coordinate.DistanceVector() + new Vector3(0,0,0.5f), Quaternion.identity, holder);
        backgroundTile.gameObject.transform.localScale = new Vector3((1 * dimension + 0.5f), (1 * dimension + 0.5f), 1f); 
        backgroundTile.tag = "Background";
        backgroundTile.name = "Background tile";
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                //gameTiles[i, j] = prefabTile;
                //instantiates them based on coords  and puts these tiles under the tile spawner object (holder) for neatness          
                var newtile = Instantiate(prefabTile, tileBoard[i,j].coordinate.DistanceVector(), Quaternion.identity, holder);
                //spawn background (atm this is just a tile as well)                
                newtile.name = "Tile (" +i +"," +j + ")" ; //name tiles on position in inspector
                gameTiles[i, j] = newtile;
                newtile.GetComponent<tileScript>().tile = new Tile(new Pair(i, j)); //sets tile scripts tile reference 
            }
        }
    }
    //sets the gameobjects tiles to the tiles in the gamestate and then instantiates them in teh correct space in the game
    private void UpdateGameObjectArray() {        
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                
                gameTiles[i, j].GetComponent<tileScript>().tile = tileBoard[i,j];
               
            }
        }
    }
    
    // create matrix of empty board
    private void intialiseBoard() {
        //creates matrix representation
        tileBoard = new Tile[dimension,dimension];
        
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                tileBoard[i,j] = new Tile(new Pair(i,j));             
            }
        }
              
    }


    //adds (all possible) free points to the list of pairs
    private void intialiseFreePoints() {
        freePoints = new List<Pair>();
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                freePoints.Add(new Pair(i, j));
            }
        }
        //Debug.Log("free points after init: " + freePoints.Count);
    }

    private bool isNoneRegion(Pair coord) {
        if (tileBoard[coord.first, coord.second].region == Tile.Region.None) {
            return true;
        } else {
            return false;
        }
    }

    //given position of tileboard[x,y] find all immediate and valid neighbours: 
    //should return list of length between 8 and 3
    private List<Pair> findNeighbours(int x, int y) {
        
        List<Pair> neighbours = new List<Pair>();
        HashSet<Pair> hashPairs = new HashSet<Pair>();
        //Debug.Log("Finding : " + x + "," + y + " neighbours");      
          
        for (int i=-1; i<2; i++) {
            for (int j = -1; j < 2; j++) {                
                    hashPairs.Add(new Pair(x + i, y + j));                    
            }
        }

        // add unique entries from hashtable into the list 
        //Debug.Log("Expected neighbours unclamped: 9, Actual: " + hashPairs.Count);
        foreach( Pair p in hashPairs) {
            //check if values are in bounds
           if(p.first > dimension-1 || p.second > dimension -1 || p.first < 0 || p.second < 0) {
                //Debug.Log("Point  " + p.toString() + " is not within the grid");
            }
            else if( p.EqualsCheck(new Pair(x,y) )) { //not out of bounds or itself, so add this 
                //
            }
            else { //must be valid and not same
                neighbours.Add(p);
                tileBoard[x, y].adjacentTileCoords.Add(p); // add to tile objects neighbours
            }
            //otherwise this is not valid so move on
        }
      // Debug.Log("valid neighbours found:  " + neighbours.Count);
       // Debug.Log("Neighbours found:  " + neighbours); 
        return neighbours;

    }

    //pick the first point of which to allocate to a starting region
    //if starting point doesnt have any free neighbours then it retries, if freepoints >1
    private bool allocatePointToRegion(Tile.Region region, HashSet<Pair> noneNeighbours) {
        int randIndex = UnityEngine.Random.Range(0, freePoints.Count); //pick a random free point to assign to region 
        int xPos = freePoints[randIndex].first;
        int yPos = freePoints[randIndex].second;
        List<Pair> nlist = findNeighbours(xPos, yPos);
        foreach (Pair p in nlist) {
            if (isNoneRegion(p)) //add to hashset of noneneighbours only if the tile is none
                noneNeighbours.Add(p);
        }
        if (noneNeighbours.Count >= 1 || freePoints.Count <1) { // set random neighbour to city
            int rand = Random.Range(0, noneNeighbours.Count);
            Pair p = noneNeighbours.ElementAt(rand); // slow O(n) but should be ok considering relatively small dimensions                                                             
            tileBoard[p.first, p.second].region = region;
            noneNeighbours.Remove(p); //also slow O(n) 
                                      //Debug.Log(noneNeighbours.Count + " neighbours count for " + region);
            freePoints.RemoveAt(randIndex); //remove this point as it is no longer free
            tileBoard[xPos, yPos].region = region;
            return true;
        }
        else {
            //try again.. shouldnt cause infinite loop? 
            Debug.Log("attempting to find a better starting point...");
            allocatePointToRegion(region, noneNeighbours);
        }
        return false;
    }


    private HashSet<Pair> allocateNeighbourToRegion(Tile.Region region, HashSet<Pair> noneNeighbours) {
        //pick random point in neighbours list, set it to region 
        int rand = Random.Range(0, noneNeighbours.Count);
        Pair p = noneNeighbours.ElementAt(rand);
        tileBoard[p.first, p.second].region = region;
        //remove this point from neighbours and freepoints as it is no longer free
        noneNeighbours.Remove(p);
        //////////freePoints.RemoveAt(rand); issue removing from free points properly
        //find neighbours of this point and add them to the neighbours list
        List<Pair> nlist = findNeighbours(p.first, p.second);
        foreach (Pair pair in nlist) {
            if (isNoneRegion(pair)) //add to hashset of noneneighbours only if the tile is none
                noneNeighbours.Add(pair);
        }
        return noneNeighbours; //doesnt need to return as parameter object is mutable and changes within this method
    }


    //given a region and amount (counter) allocate tiles to that region
    private void AllocateTiles(Tile.Region region, int counter) {
        //pick random free tile from the list of tiles and allocate it to city
        HashSet<Pair> noneNeighbours = new HashSet<Pair>(); //avoid dupllicate points here. no way to select specific index/ random elem from hashset however :(
     
        Debug.Log("allocating" + counter + " " + region + " tiles...");
        //Debug.Log("allocate tiles: counter : " + counter + " , freePoints count: " + freePoints.Count);
        allocatePointToRegion(region, noneNeighbours);
        counter--;
        while (counter > 0 && freePoints.Count > 0) {
            allocateNeighbourToRegion(region, noneNeighbours);
            counter--;
        }
    }

    //splits regions into dimensions 
    private void AllocateRegions(float wildP, float cityP, float farmP, float noneP) {
      //  Debug.Log("dimension is.. : " + dimension);
        int totalTiles = dimension * dimension;
       // Debug.Log("total tiles : " + totalTiles);
        int wildTiles = Mathf.RoundToInt(wildP * totalTiles);
        int cityTiles = Mathf.RoundToInt(cityP * totalTiles);
        int farmTiles = Mathf.RoundToInt(farmP * totalTiles);
        int noneTiles = totalTiles - wildTiles - cityTiles - farmTiles;
        //Debug.Log("tiles of type None: " + noneTiles);
       // Debug.Log("tiles of type City: " + cityTiles);
        if (wildTiles + cityTiles + farmTiles + noneTiles != totalTiles) {
            Debug.Log("Rounding error");
        }
        //allocate city, wildnerness and farm tiles
        AllocateTiles(Tile.Region.City, cityTiles);        
        AllocateTiles(Tile.Region.Wilderness, wildTiles);       
        AllocateTiles(Tile.Region.Farmland, farmTiles);
       // Debug.Log("Allocated starting regions to tiles");
    }
}
