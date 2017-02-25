# SimpleTilemap v1.0

<img src="https://dl.dropboxusercontent.com/u/11960643/simpletilemap/simpletilemapstand.gif" alt="Import Settings" width="400">
<img src="https://dl.dropboxusercontent.com/u/11960643/simpletilemap/simpletilemap.gif" alt="Import Settings" width="400">

Developed for [@isotacticsgame](http://twitter.com/isotacticsgame)

SimpleTilemap is a totally free library for generating tilemaps in Unity at runtime. 
It provides users with a single script that, when used with a tilemap sprite, will generate a custom textured mesh with tilemap-indexed tiles, using a single GameObject. 
Its intended use is for 2D pixelart games with orthographic cameras, and has not been tested or verified with 3D assets.

## Usage

All functionality is contained in the single SimpleTilemap script, with an example of full functionality in the two included example Unity scenes and in the APIDemo.cs script.
The biggest thing to note with this library is that this script assumes certain import settings on tilemap sprties to work properly, which you can see below.

<img src="https://dl.dropboxusercontent.com/u/11960643/simpletilemap/import-settings.png" alt="Import Settings" width="400">

The big thing to note is that this script assumes that the texture is set to **Sprite** with a **one pixel per unit** PPU setting, **Point filter mode**, and no compression.
"Sprite Mode" is also set to "Single", **even for tilemaps**. I recgonize this is somewhat of a limitation, but I also know this is somewhat standard practice
for pixelart games in Unity. If anybody has any ideas/opinions around this, please let me know! 

This is also why you need to fill in the tilemap parameters on the script. For more info on the component, see below.

## Component parameters

<img src="https://dl.dropboxusercontent.com/u/11960643/simpletilemap/final-component.png" alt="Component Parameters" width="400">

The component is relatively barebones, but displays core parameters to get the script working. In order:

**Map Render Type** allows you to choose between a Standard Grid map and an Isometric map. 
The main difference here is that Isometric maps are rendered in a slightly different way than standard maps, which is reflected in the appearance of the "Tile Art Height Fraction" and "Tile Pivot Y" parameters when Isometric is selected.

**Map Size** allows you to select how wide/tall you map is. Worth noting is that in standard maps, (0,0) is the bottom left corner, and in Isometric maps it is the leftmost corner. 
The max map size is currently limited to 100 only because that runs up againsts Unity's max vert number for a single mesh. I have ideas about how to make bigger maps, but I'll wait and see if it is a thing people are interested in.

**Tile Width/Height** is the width/height of a single tile in the tilemap, in Unity units. Note that this isn't how tall the art is in a given tile, but the actual tile dimensions.
This should normally be your PPU import setting * dimension in pixels of your tile, but changing these values while maintaining the same ratio between them will result in a small or large **rendered** tilemap.

**Tile Art Height Fraction** is only relevant for Isometric maps and determines how to properly draw the map by defining what fraction of your total tile height your tile art occupies. A value of '2' would indicate that the tile base of your art is 1/2 the total height of your tile.

**Tile Pivot Y** determines where the leftmost point on a given tile in your isometric tilemap is in Y (X is assumed to be 0). This assumes that a given tile's (0,0) is in the bottom left corner, so this should be a positive value, with a maximum at your Tile Height.

**Render Spacing** controls how much space there is in Z between tiles on different "layers". This setting defaults to 1 and should be fine for most games. This is how the library sorts depth.

**Tilemap Material** this is the default material used to render the tilemap. It shouldn't be changed, but in actuality is just the Unity default Unlit/Transparent Cutout shader.

**Tilemap** is your actual tilemap sprite.

**Tile Columns/Rows** is how many columns/rows are in your provided tilemap.

## Where to go from here

I'd love to hear feedback (issues/pull requests/DMs/etc.) on this library! I can be reached on Twitter through [@kkukshtel](http://twitter.com/kkukshtel) or through my game [@isotacticsgame](http://twitter.com/isotacticsgame)). The intention is to keep the library as barebones as possible, so if you can think of features almost everyone would want, let me know, or submit a pull request.

## License

Do whatever.


