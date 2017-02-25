using UnityEditor;

namespace SimpleTilemap {
	
	[CustomEditor(typeof(SimpleTilemap)), CanEditMultipleObjects]
	public class SimpleTilemapEditor : Editor
	{
        SerializedProperty _mapSizeX;
        SerializedProperty _mapSizeY;
        SerializedProperty _mapRenderType;
        SerializedProperty _tileRenderWidthInUnits;
        SerializedProperty _tileRenderHeightInUnits;
        SerializedProperty _tileArtHeightFraction;
        SerializedProperty _tilePivotY;
        SerializedProperty _renderSpacing;
        SerializedProperty _tilemap;
        SerializedProperty _tilemapMaterial;
        SerializedProperty _defaultTilemapIndex;
        SerializedProperty _tileColumns;
        SerializedProperty _tileRows;
        
        void OnEnable()
        {
            _mapSizeX                = serializedObject.FindProperty("mapSizeX");
            _mapSizeY                = serializedObject.FindProperty("mapSizeY");
            _mapRenderType           = serializedObject.FindProperty("mapRenderType");
            _tileRenderWidthInUnits  = serializedObject.FindProperty("tileWidth");
            _tileRenderHeightInUnits = serializedObject.FindProperty("tileHeight");
            _tileArtHeightFraction   = serializedObject.FindProperty("tileArtHeightFraction");
            _tilePivotY              = serializedObject.FindProperty("tilePivotY");
            _renderSpacing           = serializedObject.FindProperty("renderSpacing");
            _tilemap                 = serializedObject.FindProperty("tilemap");
            _tilemapMaterial         = serializedObject.FindProperty("tilemapMaterial");
            _defaultTilemapIndex      = serializedObject.FindProperty("defaultTilemapIndex");
            _tileColumns             = serializedObject.FindProperty("tileColumns");
            _tileRows                = serializedObject.FindProperty("tileRows");
        }

        public override void OnInspectorGUI()
	    {
			//update the object with the object variables
			serializedObject.Update ();
			
			//set the clip var as the target of this inspector
			SimpleTilemap tilemap = (SimpleTilemap)target;

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Easy way to get fast, runtime tilemaps generated in Unity.");
            EditorGUILayout.Space();
            MapDisplaySettings();
            EditorGUILayout.Space();
            MapRenderSettings(tilemap);
            EditorGUILayout.Space();
            TilemapSettings();
            // EditorGUILayout.Knob(new Vector2(2,2), 0, -1, 1, "aye", Color.blue, Color.gray, true);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        void MapDisplaySettings()
        {
            EditorGUILayout.LabelField("Map Display Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_mapRenderType);
            EditorGUILayout.PropertyField(_mapSizeX);
            EditorGUILayout.PropertyField(_mapSizeY);
        }

        void MapRenderSettings(SimpleTilemap map)
        {
            EditorGUILayout.LabelField("Tile Render Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_tileRenderWidthInUnits); 
            EditorGUILayout.PropertyField(_tileRenderHeightInUnits);

            if(map.mapRenderType == MapRenderType.Isometric)
            {
                EditorGUILayout.PropertyField(_tileArtHeightFraction);   
                EditorGUILayout.PropertyField(_tilePivotY);   
            }

            else
            {
                //Set the tile offsets to default values
                _tilePivotY.intValue = 16;
                _tileArtHeightFraction.intValue = 2;
            }

            EditorGUILayout.PropertyField(_renderSpacing);          
        }

        void TilemapSettings()
        {
            EditorGUILayout.PropertyField(_tilemapMaterial);
            EditorGUILayout.PropertyField(_tilemap);
            EditorGUILayout.PropertyField(_defaultTilemapIndex);
            EditorGUILayout.PropertyField(_tileColumns);
            EditorGUILayout.PropertyField(_tileRows);
        }

    }


}