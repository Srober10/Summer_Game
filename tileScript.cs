
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class tileScript : MonoBehaviour
{
    //public tileSpawner spawner; //asssign in inspector to get dimension to work out pos not needed.. 
    [SerializeField] public Tile tile;
    public Material[] regionColours; //assign this in inspector 
    private Renderer rend;
    public int index;
    AutoCam camera;
    //register clicks

    // Start is called before the first frame update
    void Start() {
        //get the tile based on this objects position in heighracy
        //regionColour = GetComponent<Material>();
        //Debug.Log("Setting tile colour");
       // spawner = G
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AutoCam>();
        //set colour of tile based on region
        if(this.gameObject.tag == "Background") {
            rend.sharedMaterial = regionColours[4];
        }

    }

    private void OnMouseDown() {
        camera.SetTarget(this.transform); //works. 
        Debug.Log("clicked on : " + tile.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (tile != null) {
            if (tile.region == Tile.Region.City) {
                rend.sharedMaterial = regionColours[1];
            }
            else if (tile.region == Tile.Region.Farmland) {
                float randXOffset = Random.Range(0, 1);
                float randYOffset = Random.Range(0, 1);
                rend.sharedMaterial = regionColours[2];
                rend.sharedMaterial.SetTextureOffset("_BumpMap", new Vector2 ( randXOffset, randYOffset)); 
            }
            else if (tile.region == Tile.Region.Wilderness) {
                rend.sharedMaterial = regionColours[3];
            }
            else if (tile.region == Tile.Region.None) { //none or background
                if(this.gameObject.tag == "Background") { 
                    rend.sharedMaterial = regionColours[4];
                    return;
                } 
                rend.sharedMaterial = regionColours[0];
            }
            else { //null
               
            }
        }
    }
}
