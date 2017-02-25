using UnityEngine;

namespace SimpleTilemap
{
	// SimpleTilemap by @kkukshtel
	// Made for @isotacticsgame and given to you with <3 for whatever
	// Use this class to generate fast runtime tilemaps
	// See example usage in APIDemo.cs
	
	public enum MapRenderType
	{
		Standard,
		Isometric
	}

	public struct MapData
	{
		public Vector3[] vertices; // All the verticies in a map
		public Vector3[] normals; // All the normals in a map
		public Vector2[] uv; // all the UV coordinates of a map's verts
		public int[] triangles; // all the triangles of a map
		public MapData(int totalVerts)
		{
			vertices = new Vector3[totalVerts];
			normals = new Vector3[totalVerts];
			uv = new Vector2[totalVerts];
			triangles = new int[totalVerts];
		}
	}


	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class SimpleTilemap : MonoBehaviour {

		// General Map Settings
		[Tooltip("Type of map to be rendered")]
		public MapRenderType mapRenderType;
		[Tooltip("Map size in the X direction"), RangeAttribute(1, 100)]
		public int mapSizeX = 10;
		[Tooltip("Map size in the Y direction"), RangeAttribute(1, 100)]
		public int mapSizeY = 10;

		// Render settings
		[Tooltip("Tile render width in Unity units")]
		public int tileWidth = 64;
		[Tooltip("Tile render height in Unity units")]
		public int tileHeight = 64;
		[Tooltip("What fraction of your total tileHeight does your art occupy. A value of '2' would mean your tile art is half as tall as your total height")]
		public int tileArtHeightFraction = 2;
		[Tooltip("Where is the leftmost corner of your tile located in Y Unity units, assuming a tile's (0,0) is the bottom left.")]
		public int tilePivotY = 16;
		[Tooltip("How much distance between tiles in Z"), RangeAttribute(1.0f, 10.0f)]
		public float renderSpacing = 2.0f;

		// Tilemap attributes
		[Tooltip("Drag tilemap material from package here")]
		public Material tilemapMaterial;
		[Tooltip("Which tilemap to use")]
		public Sprite tilemap;
		[Tooltip("Default tile index. Tilemap is indexed from top left to bottom right, starting at 0.")]
		public int defaultTilemapIndex = 0;
		[Tooltip("How many columns in the tilemap")]
		public int tileColumns = 3;
		[Tooltip("How many rows in the tilemap")]
		public int tileRows = 3;


		// Component References
		MeshFilter mf;
		MeshRenderer mr;
		MeshCollider mc;

		// Graphics Info
		MapData MapInfo;
		float tileUVWidth;
		float tileUVHeight;
		Mesh MapMesh;

		// Assign references on Awake
		void Awake()
		{
			if(tilemap == null)
			{
				Debug.LogError("Need to attach a tilemap reference to SimpleTilemap script.");
			}

			//assign mesh props
			mf = GetComponent<MeshFilter>();
			mr = GetComponent<MeshRenderer>();
			mc = GetComponent<MeshCollider>();
		}
		
		// Build the tilemap. Note that this can be called whenever
		public void BuildMap()
		{
			//create a new material from the "prefab" material so you don't destroy the prefab
			Material mat = new Material(tilemapMaterial);
			mat.mainTexture = tilemap.texture;
			mr.sharedMaterial = mat;

			//assign tile props
			MapMesh = new Mesh();
			tileUVWidth = 1.0f / tileColumns;
			tileUVHeight = 1.0f / tileRows;

			//assing map props
			int numTiles = mapSizeX * mapSizeY;
			int numVerts = numTiles * 6; // six verts per tile
			int rowVerts = mapSizeX * 6; // six verts per tile
			MapInfo = new MapData(numVerts);

			int x,y;
			for (y = 0; y < mapSizeY; y++)
			{
				for (x = 0; x < mapSizeX; x++)
				{

					// subtract the transform to normalize tile placement coords
					Vector3 tilePosition = GetWorldPositionOfTile(new Tile(x,y)) - transform.position;
					int index = ( y * rowVerts) + (x * 6); // 6 because there are 6 verts per square

					/*   
					
					Vertex Layout
						0___1,3
						| /|
					2,5|/_| 4

					*/

					//TOP triangle of square, clockwise
					MapInfo.vertices [index + 0] = new Vector3( tilePosition.x             , tilePosition.y                  , tilePosition.z);
					MapInfo.vertices [index + 1] = new Vector3( tilePosition.x + tileWidth , tilePosition.y                  , tilePosition.z);
					MapInfo.vertices [index + 2] = new Vector3( tilePosition.x             , tilePosition.y - tileHeight     , tilePosition.z);
					//assign top tri, wrap clockwise
					MapInfo.triangles[index + 0] = index + 0;
					MapInfo.triangles[index + 1] = index + 1;
					MapInfo.triangles[index + 2] = index + 2;
					//BOTTOM triangle, clockwise
					MapInfo.vertices [index + 3] = new Vector3( tilePosition.x + tileWidth   , tilePosition.y              , tilePosition.z);
					MapInfo.vertices [index + 4] = new Vector3( tilePosition.x + tileWidth   , tilePosition.y - tileHeight , tilePosition.z);
					MapInfo.vertices [index + 5] = new Vector3( tilePosition.x               , tilePosition.y - tileHeight , tilePosition.z);
					//assign bottom tri, wrap clockwise
					MapInfo.triangles[index + 3] = index + 3;
					MapInfo.triangles[index + 4] = index + 4;
					MapInfo.triangles[index + 5] = index + 5;
					//assing normals
					MapInfo.normals [index + 0] = Vector3.forward;
					MapInfo.normals [index + 1] = Vector3.forward;
					MapInfo.normals [index + 2] = Vector3.forward;
					MapInfo.normals [index + 3] = Vector3.forward;
					MapInfo.normals [index + 4] = Vector3.forward;
					MapInfo.normals [index + 5] = Vector3.forward;

					// assing tilemap sprite to tile
					//note that this currently uses a defualt value, but could be easily modified to accept a preconfigued value
					AssignSpriteToTile(new Tile(x,y), defaultTilemapIndex, false);
				}
			}
			
			//Update the tilemap
			UpdateMesh(true, true, true, true);
		}

		// Assign which tile of tilemap should be placed at which tile
		public void AssignSpriteToTile(Tile targetTile, int tilemapIndex, bool shouldUpdateMesh)
		{
			//get starting vert for the tile
			int rowVerts = mapSizeX * 6;
			int vertIndex = (targetTile.y * rowVerts) + (targetTile.x * 6);

			//convert tilemapIndex to x,y of tile, with 0,0 being the bottom left tile to make finding UV cords easier
			Tile mapIndex = new Tile(tilemapIndex%tileColumns,(tileRows-1) - (tilemapIndex/tileRows));

			//assign new UV coords based on tile index
			MapInfo.uv [vertIndex + 0] = new Vector2 (tileUVWidth                * mapIndex.x, tileUVHeight + tileUVHeight * mapIndex.y);
			MapInfo.uv [vertIndex + 1] = new Vector2 (tileUVWidth  + tileUVWidth * mapIndex.x, tileUVHeight + tileUVHeight * mapIndex.y);
			MapInfo.uv [vertIndex + 2] = new Vector2 (tileUVWidth                * mapIndex.x, tileUVHeight                * mapIndex.y);
			MapInfo.uv [vertIndex + 3] = new Vector2 (tileUVWidth  + tileUVWidth * mapIndex.x, tileUVHeight + tileUVHeight * mapIndex.y);
			MapInfo.uv [vertIndex + 4] = new Vector2 (tileUVWidth  + tileUVWidth * mapIndex.x, tileUVHeight                * mapIndex.y);
			MapInfo.uv [vertIndex + 5] = new Vector2 (tileUVWidth                * mapIndex.x, tileUVHeight                * mapIndex.y);
			if (shouldUpdateMesh) {UpdateMesh(false, false, false, true);}
		}

		// Assign the vars of the mesh map and set references to the proper components
		void UpdateMesh(bool updateVerts, bool updateTris, bool updateNormals, bool updateUVs)
		{
			if(updateVerts) {MapMesh.vertices = MapInfo.vertices;}
			if(updateTris) {MapMesh.triangles = MapInfo.triangles;}
			if(updateNormals) {MapMesh.normals = MapInfo.normals;}
			if(updateUVs) {MapMesh.uv = MapInfo.uv;}
			mf.mesh = MapMesh;
			mc.sharedMesh = MapMesh;
		}

		// Get the tile at a given world position. Z is truncated
		public Tile GetTileAtWorldPosition(Vector3 worldPosition)
		{
			Vector2 hover = worldPosition;
			Vector2 hoveredTile = new Vector2();

			if ( mapRenderType == MapRenderType.Isometric)
			{
				//assumes that the tile art streteches across the whole tile in x
				//so no need for xstep
				int tilestepY = tileHeight / (tileArtHeightFraction * 2);

				//account for map position in space
				Vector2 mapOffset = transform.position - new Vector3(0,tileHeight - tilePivotY,0);
				Vector2 cords = hover - mapOffset;

				//find the actual hovered tile
				//tilestep.y is multiplied by two to get the actual 'art height' of a tile
				hoveredTile.x = (cords.x/tileWidth) - (cords.y / (tilestepY*2));
				hoveredTile.y = (cords.y/(tilestepY*2)) + (cords.x / tileWidth);
			}

			else
			{
				// easy offset if in a standard map
				Vector3 mapOffset = transform.position - new Vector3(0,tileHeight,0);
				hoveredTile.x = (int)(hover.x - mapOffset.x) / tileWidth;
				hoveredTile.y = (int)(hover.y - mapOffset.y) / tileHeight;
			}

			//return the tile if it is within range of the map
			Tile returnTile = new Tile(Mathf.FloorToInt(hoveredTile.x), Mathf.FloorToInt(hoveredTile.y));
			if(returnTile.x >= 0 && returnTile.y >= 0 && returnTile.x < mapSizeX && returnTile.y < mapSizeY)
			{
				return returnTile;
			}

			// else return null
			return null;	
		}

		// Returns Vector3 Position of a tile in space.
		public Vector3 GetWorldPositionOfTile(Tile tile)
		{
			Vector3 position = new Vector3();
			if(mapRenderType == MapRenderType.Isometric)
			{
				position.x = tile.x * (float)tileWidth/2 + tile.y * (float)tileWidth/2;
				position.y = tile.y * (float)tileHeight/(tileArtHeightFraction * 2) - tile.x * (float)tileHeight/(tileArtHeightFraction * 2);
			}

			else
			{
				position.x = tile.x * (float)tileWidth;
				position.y = tile.y * (float)tileHeight;
			}
			position.z = (tile.y - tile.x) * renderSpacing;

			return position + transform.position;
		}
	}
}

