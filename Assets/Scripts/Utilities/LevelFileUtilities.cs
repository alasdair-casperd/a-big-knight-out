
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

        // Converts a Serializing_Entity to an Entity (ignores position information on the former)
        Entity Entity(Serializing_Entity e)
        {
            EntityType entityType = EntityType.All.Where(type => type.ID == e.EntityTypeID).First();
            return new Entity(entityType, e.InitialState, e.GraphicsVariant, e.Direction);
        }

        // Deserialize json into a Serializing_Level object
        var l = JsonUtility.FromJson<Serializing_Level>(json);

        // Extract a dictionary of tiles from the deserialized Serializing_Level
        var tiles = new Dictionary<Vector2Int, Tile>();
        foreach (var t in l.Tiles)
        {
            tiles.Add(Vector2Int(t.Position), Tile(t));
        }

        // Extract a dictionary of entities from the deserialized Serializing_Level
        var entities = new Dictionary<Vector2Int, Entity>();
        foreach (var e in l.Entities)
        {
            entities.Add(Vector2Int(e.Position), Entity(e));
        }

        // Extract moving platforms
        var movingPlatforms = new Dictionary<Vector2Int, int>();
        foreach (var mp in l.MovingPlatforms)
        {
            movingPlatforms.Add(Vector2Int(mp.Position), mp.Direction);
        }

        // Return the corresponding level
        return new Level
        (
            name: l.Name,
            startPosition: Vector2Int(l.StartPosition),
            tiles: tiles,
            entities: entities,
            movingPlatforms: movingPlatforms
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
        public Serializing_Entity[] Entities;
        public Serializing_MovingPlatform[] MovingPlatforms;

        public Serializing_Level(Level level)
        {
            Name = level.Name;
            StartPosition = new Serializing_Vector2Int(level.StartPosition);
            var tiles_list = new List<Serializing_Tile>();

            // Add tiles
            foreach (var (position, tile) in level.Tiles)
            {
                tiles_list.Add(new Serializing_Tile(tile, position));
            }
            Tiles = tiles_list.ToArray();

            // Add entities
            var entities_list = new List<Serializing_Entity>();
            foreach (var (position, entity) in level.Entities)
            {
                entities_list.Add(new Serializing_Entity(entity, position));
            }
            Entities = entities_list.ToArray();

            // Add moving platforms
            var movingPlatforms_list = new List<Serializing_MovingPlatform>();
            foreach (var (position, direction) in level.MovingPlatforms)
            {
                movingPlatforms_list.Add(new Serializing_MovingPlatform(position, direction));
            }
            MovingPlatforms = movingPlatforms_list.ToArray();
        }
    }

    [Serializable]
    private struct Serializing_Tile
    {
        public int TileTypeID;
        public int InitialState;
        public int GraphicsVariant;
        public Serializing_Vector2Int Position;
        public List<Serializing_Vector2Int> Links;

        public Serializing_Tile(Tile tile, Vector2Int position)
        {
            TileTypeID = tile.Type.ID;
            InitialState = tile.InitialState;
            GraphicsVariant = tile.GraphicsVariant;
            Position = new Serializing_Vector2Int(position);
            Links = tile.Links.Select(link => new Serializing_Vector2Int(link)).ToList();
        }
    }

    [Serializable]
    private struct Serializing_Entity
    {
        public int EntityTypeID;
        public int GraphicsVariant;
        public int InitialState;
        public int Direction;
        public Serializing_Vector2Int Position;

        public Serializing_Entity(Entity entity, Vector2Int position)
        {
            EntityTypeID = entity.Type.ID;
            GraphicsVariant = entity.GraphicsVariant;
            InitialState = entity.InitialState;
            Direction = entity.Direction;
            Position = new Serializing_Vector2Int(position);
        }
    }

    [Serializable]
    private struct Serializing_MovingPlatform
    {
        public Serializing_Vector2Int Position;
        public int Direction;

        public Serializing_MovingPlatform(Vector2Int position, int direction)
        {
            Position = new Serializing_Vector2Int(position);
            Direction = direction;
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