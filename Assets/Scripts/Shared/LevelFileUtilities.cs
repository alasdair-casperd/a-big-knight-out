
using System;
using System.Linq;
using UnityEngine;

public static class LevelFileUtilities
{
    public static string Export(Level level)
    {
        string StringFor(Vector2Int vector2Int)
        {
            return $"({vector2Int.x},{vector2Int.y})";
        }

        var output = "";
        output += $"{level.Name}\n";
        output += $"Start = {StringFor(level.startPosition)}\n";
        output += "\n";

        foreach (var (position, tile) in level.tiles)
        {
            output += $"{StringFor(position)} {tile.type.ID} ({tile.type.DisplayName})\n";
            output += $"\tInitialState = {tile.initialState}\n";
            output += $"\tGraphicsVariant = {tile.graphicsVariant}\n";
            output += $"\tLinks = [";

            var i = 0;
            foreach (var link in tile.links)
            {
                if (i > 0) output += ", ";
                i += 1;
                output += StringFor(link);
            }
            output += "]\n";
            output += "\n";
        }

        return output;
    }

    public static Level Parse(string input)
    {
        // Helper function to parse Vector2Int from a string like "(x,y)"
        Vector2Int ParseVector2Int(string str)
        {
            var trimmed = str.Trim('(', ')');
            var parts = trimmed.Split(',');
            return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        var lines = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var level = new Level();
        int index = 0;

        // Parse level name
        level.Name = lines[index];
        index++;

        // Parse start position
        var startPosStr = lines[index].Split('=')[1].Trim();
        level.startPosition = ParseVector2Int(startPosStr);
        index++;

        // Parse tiles
        level.tiles = new();
        while (index < lines.Length)
        {
            // Parse tile position and type
            var firstLine = lines[index];
            var parts = firstLine.Split(' ');
            var position = ParseVector2Int(parts[0]);
            var tileTypeID = int.Parse(parts[1]);
            index++;

            // Parse tile properties
            var initialState = int.Parse(lines[index].Split('=')[1].Trim());
            index++;
            var graphicsVariant = int.Parse(lines[index].Split('=')[1].Trim());
            index++;

            // Parse links
            var linksLine = lines[index].Split('=')[1].Trim('[', ']', ' ');
            var linkStrings = linksLine.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            var links = linkStrings.Select(ParseVector2Int).ToList();
            index++;

            // Add parsed tile to the level
            var tile = new TileBuildData
            {
                type = TileType.All.First(t => t.ID == tileTypeID),
                initialState = initialState,
                graphicsVariant = graphicsVariant,
                links = links
            };
            level.tiles.Add(position, tile);
        }

        return level;
    }
}