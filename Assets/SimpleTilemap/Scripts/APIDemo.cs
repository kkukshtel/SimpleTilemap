using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleTilemap
{
    // SimpleTilemap APIDemo by @kkukshtel
	// Made for @isotacticsgame and given to you with <3 for whatever
    // Use this class to see how to interact with the SimpleTilemap
	
    public class APIDemo : MonoBehaviour {
        public SimpleTilemap map;
        public Tile lastHovered;
        void Start()
        {
            lastHovered = new Tile(0,0);
            map.BuildMap();
        }

        void Update()
        {
            // Update a tile that is being hovered
            Tile tile = map.GetTileAtWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (tile != null && (tile.x != lastHovered.x || tile.y != lastHovered.y))
            {
                map.AssignSpriteToTile(lastHovered, 1, true);
                lastHovered = tile;
                map.AssignSpriteToTile(lastHovered, 2, true);  
            }

            // Put down a whole new sprite on the map, outside of the tilemap
            if( Input.GetKeyDown(KeyCode.Space))
            {
                putSpriteOnMap();
            }

            // Rebuild the map
            if( Input.GetKeyDown(KeyCode.R))
            {
                map.BuildMap();
            }
        }

        // Put down a whole new sprite on the map, outside of the tilemap
        void putSpriteOnMap()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.enabled = true;
            float spriteHeightInUnits = sr.sprite.texture.height * sr.sprite.pixelsPerUnit;
            Vector3 tilePos = map.GetWorldPositionOfTile(lastHovered);
            Vector3 placePos = new Vector3(tilePos.x,(tilePos.y - (spriteHeightInUnits - sr.sprite.pivot.y/spriteHeightInUnits)), tilePos.z);
            this.transform.position = placePos;
        }

    }
}