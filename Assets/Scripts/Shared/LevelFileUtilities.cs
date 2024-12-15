
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// A class storing methods used to convert Level objects to and from JSON
/// </summary>
public static class LevelFileManager
{
    /*
        JSON Import Functions
    */

    /// <summary>
    /// Create a Level object from a given JSON string
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Level ParseLevelFromJSON(string json)
    {
        // Convert a Serializing_Vector2Int to a Vector2Int
        Vector2Int Vector2Int(Serializing_Vector2Int v)
        {
            return new Vector2Int(v.x, v.y);
        }

        // Convert a Serializing_Tile to a Tile (ignores position information on the former)
        Tile Tile(Serializing_Tile t)
        {
            TileType tileType = TileType.All.Where(type => type.ID == t.TileTypeID).First();
            
            return new Tile
            (
                type: tileType,
                initialState: t.InitialState,
                graphicsVariant: t.GraphicsVariant,
                links: t.Links.Select(l => Vector2Int(l)).ToList()
            );
        }
        
        // Deserialize json into a Serializing_Level object
        var l = JsonUtility.FromJson<Serializing_Level>(json);

        // Extract a list of tiles from this
        var tiles = new Dictionary<Vector2Int, Tile>();
        foreach (var t in l.Tiles)
        {
            tiles.Add(Vector2Int(t.Position), Tile(t));
        }

        // Return the corresponding level
        return new Level
        (
            name: l.Name,
            startPosition: Vector2Int(l.StartPosition),
            tiles: tiles
        );
    }

    /*
        JSON Export Functions
    */

    /// <summary>
    /// Export a Level object as JSON file
    /// </summary>
    /// <param name="level">The level to convert to JSON</param>
    /// <param name="fileName">The file name to use</param>
    public static void ExportLevelAsJson(Level level, string fileName)
    {
        string path = Application.dataPath + $"/Levels/{fileName}.json";
        File.WriteAllText(path, ExportLevelAsJSON(level));

        Debug.Log($"Level Exported to File System at '{path}'. You may need to click 'Refresh' to see it.");
    }

    /// <summary>
    /// Convert a level object to a JSON string
    /// </summary>
    /// <returns></returns>
    private static string ExportLevelAsJSON(Level level)
    {
        var serializing_Level = new Serializing_Level(level);
        return JsonUtility.ToJson(serializing_Level);
    } 

    /*
        Serializing Classes
    */

    // These classes are used when converting levels to and from JSON. When exporting as JSON, a
    // Level object is converted to a Serializing_Level object, which is then serialized. This is
    // in part to get around the issue of Dictionaries not being Serializable, but also gives us more
    // control over the format of the exported JSON. Some examples:
    //      1. We don't have to needlessly store the magnitude of every Vector2Int serialized
    //      2. We can store just the id of each tile's type rather than all of the information
    //         associated with that tileType

    [Serializable]
    private struct Serializing_Level
    {
        public string Name;
        public Serializing_Vector2Int StartPosition;
        public Serializing_Tile[] Tiles;

        public Serializing_Level(Level level)
        {
            Name = level.Name;
            StartPosition = new Serializing_Vector2Int(level.StartPosition);
            var list = new List<Serializing_Tile>();
            foreach (var (position, tile) in level.Tiles)
            {
                list.Add(new Serializing_Tile(tile, position));
            }
            Tiles = list.ToArray();
        }
    }

    [Serializable]
    private struct Serializing_Tile
    {
        public int TileTypeID;
        public Serializing_Vector2Int Position;
        public int InitialState;
        public int GraphicsVariant;
        public List<Serializing_Vector2Int> Links;

        public Serializing_Tile(Tile tile, Vector2Int position)
        {
            TileTypeID = tile.Type.ID;
            Position = new Serializing_Vector2Int(position);
            InitialState = tile.InitialState;
            GraphicsVariant = tile.GraphicsVariant;
            Links = tile.Links.Select(link => new Serializing_Vector2Int(link)).ToList();
        }
    }
    
    [Serializable]
    private struct Serializing_Vector2Int
    {
        public int x;
        public int y;

        public Serializing_Vector2Int(Vector2Int vector2Int)
        {
            x = vector2Int.x;
            y = vector2Int.y;
        }
    }
    
    
}