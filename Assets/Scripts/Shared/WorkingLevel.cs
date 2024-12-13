using System.Collections.Generic;
using System;
using UnityEngine;

public class WorkingLevel
{

    // The player's starting position
    public Vector2Int startPos;

    // A dictionary containing all of the level's tiles and their positions
    public Dictionary<Vector2Int, TileBuildData> tiles;

    public void ExportTo(Level level)
    {
        level.startPos = startPos;
        
        List<TilePositionPair> tilePositionPairs = new();

        foreach (var (position, tile) in tiles)
        {
            var newTilePositionPair = new TilePositionPair();
            newTilePositionPair.tile = tile;
            newTilePositionPair.position = position;
            tilePositionPairs.Add(newTilePositionPair);
        }

        level.tiles = tilePositionPairs;
    }

    public WorkingLevel(Level level)
    {
        startPos = level.startPos;
        
        tiles = new();

        foreach (var tilePositionPair in level.tiles)
        {
            tiles.Add(tilePositionPair.position, tilePositionPair.tile.DeepCopy());
        }
    }
}