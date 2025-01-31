
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;

public class LevelEditor : MonoBehaviour
{
    public LevelBuilder LevelBuilder;

    [SerializeField]
    private TextAsset startingLevelJSON;

    public static Level LevelToPreview;

    public Transform levelParent;

    [SerializeField]
    private Prefabs prefabs;

    [SerializeField]
    private LevelEditorTool currentTool;

    public Level level;

    private LevelEditorTool[] allTools;

    private Vector2Int targetPosition;

    private Vector3 targetWorldPosition;

    private LinkIndicator targetLink;

    private bool hasDragged;

    private GameObject linksContainer;

    private GameObject stateContainer;

    private GameObject levelStartIndicator;

    private bool ShowingLinks
    {
        set { if (linksContainer != null) linksContainer.SetActive(value); }
    }

    private bool ShowingState
    {
        set { if (stateContainer != null) stateContainer.SetActive(value); }
    }

    private Vector2Int? linkStart;

    private LinkIndicator linkPreview;

    private List<LinkIndicator> linkIndicators = new();

    private List<StateIndicator> stateIndicators = new();

    public GameObject TileToolsContainer;
    public GameObject EntityToolsContainer;

    public LevelEditorTileTool TileToolPrefab;
    public LevelEditorEntityTool EntityToolPrefab;

    private void Start()
    {
        allTools = FindObjectsByType<LevelEditorTool>(FindObjectsSortMode.InstanceID);

        if (LevelToPreview != null)
        {
            level = LevelToPreview;
        }
        else
        {
            if (startingLevelJSON == null)
            {
                level = new Level(startPosition: Vector2Int.zero);
            }
            else
            {
                try
                {
                    level = LevelFileManager.ParseLevelFromJSON(startingLevelJSON.text);
                }
                catch
                {
                    Debug.LogError("Failed to parse level file");
                    level = new Level(startPosition: Vector2Int.zero);
                }
            }
        }

        LevelBuilder.BuildLevelSquares(levelParent, level, animationDuration: -1, ignoreErrors: true);
        LevelBuilder.BuildLevelEnemies(levelParent, level, animationDuration: -1, ignoreErrors: true);

        levelStartIndicator = Instantiate(prefabs.levelStartIndicator);
        PositionStartIndicator();

        GenerateLinkIndicators();
        GenerateStateIndicators();

        CreateDynamicTools();

        ShowingLinks = false;
        ShowingState = false;
        currentTool.Select();
    }

    private void Update()
    {
        TriageMouseAction();
    }

    private void CreateDynamicTools()
    {
        foreach (var tileType in TileType.All)
        {
            var tool = Instantiate(TileToolPrefab, TileToolsContainer.transform);
            tool.levelEditor = this;
            tool.TileType = tileType;
            tool.OnSquareClick = new UnityEngine.Events.UnityEvent();
            tool.OnSquareClick.AddListener(AddTile);
        }

        foreach (var entityType in EntityType.All)
        {
            var tool = Instantiate(EntityToolPrefab, EntityToolsContainer.transform);
            tool.levelEditor = this;
            tool.EntityType = entityType;
            tool.OnSquareClick = new UnityEngine.Events.UnityEvent();
            tool.OnSquareClick.AddListener(AddEntity);
        }
    }

    private void TriageMouseAction()
    {
        if ((Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && currentTool != null && !MouseUtilities.MouseOverUI)
        {
            // Read mouse position
            Vector2Int mousePosition = GridUtilities.GetMouseGridPos();

            if (Input.GetMouseButtonDown(0))
            {
                MouseDown(mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!hasDragged)
                {
                    MouseClick(mousePosition);
                }

                MouseUp(mousePosition);
            }
            else
            {
                if (targetPosition != mousePosition)
                {
                    MouseDrag(mousePosition);
                }

                MouseHeld(mousePosition);
            }
        }

        var hoveredLink = HoveredLink();

        if (Input.GetMouseButtonDown(0) && hoveredLink != null)
        {
            targetLink = hoveredLink;
            currentTool.OnLinkMouseDown.Invoke();
        }
    }

    private void MouseDown(Vector2Int mousePosition)
    {
        targetPosition = mousePosition;
        hasDragged = false;
        currentTool.OnSquareMouseDown.Invoke();
    }

    private void MouseClick(Vector2Int mousePosition)
    {
        currentTool.OnSquareClick.Invoke();
    }

    private void MouseUp(Vector2Int mousePosition)
    {
        currentTool.OnSquareMouseUp.Invoke();
    }

    private void MouseDrag(Vector2Int mousePosition)
    {
        targetPosition = mousePosition;
        hasDragged = true;
        currentTool.OnSquareDrag.Invoke();
    }

    private void MouseHeld(Vector2Int mousePosition)
    {
        targetWorldPosition = MouseUtilities.GetMouseWorldPos();
        targetWorldPosition.y = 0;
        currentTool.OnSquareMouseHeld.Invoke();
    }


    public void SelectTool(LevelEditorTool tool)
    {
        if (currentTool != null && currentTool != tool)
        {
            currentTool.Deselect();
            currentTool.OnDeselect.Invoke();
        }

        currentTool = tool;
        tool.OnSelect.Invoke();
    }

    private void RegenerateLevel()
    {
        LevelBuilder.BuildLevelSquares(levelParent, level, animationDuration: 0.5f, ignoreErrors: true);
        LevelBuilder.BuildLevelEnemies(levelParent, level, animationDuration: 0.5f, ignoreErrors: true);
        GenerateLinkIndicators();
        GenerateStateIndicators();
    }

    private void GenerateLinkIndicators()
    {
        if (linksContainer == null)
        {
            linksContainer = new GameObject("Links");
            linksContainer.transform.parent = transform;
        }

        foreach (var linkIndicator in linkIndicators)
        {
            Destroy(linkIndicator.gameObject);
        }

        linkIndicators = new();

        foreach (var (position, tile) in level.Tiles)
        {
            foreach (var link in tile.Links)
            {
                var linkIndicator = Instantiate(prefabs.linkIndicator);
                linkIndicator.transform.parent = linksContainer.transform;
                linkIndicator.InitialiseAsInteractiveLink(position, link);
                linkIndicators.Add(linkIndicator);
            }
        }
    }

    private void GenerateStateIndicators()
    {
        if (stateContainer == null)
        {
            stateContainer = new GameObject("State Indicators");
            stateContainer.transform.parent = transform;
        }

        foreach (var stateIndicator in stateIndicators)
        {
            Destroy(stateIndicator.gameObject);
        }

        stateIndicators = new();

        foreach (var (position, tile) in level.Tiles)
        {
            if (tile.Type.IsMultiState)
            {
                var stateIndicator = Instantiate(prefabs.stateIndicator);
                stateIndicator.transform.parent = stateContainer.transform;
                stateIndicator.transform.position = GridUtilities.GridToWorldPos(position);
                stateIndicator.Number = tile.InitialState;
                stateIndicators.Add(stateIndicator);
            }
        }
    }

    private void AddTile(TileType type, Vector2Int position)
    {
        if (!level.Tiles.ContainsKey(position))
        {
            AddTile();
        }
        else if (level.Tiles[position].Type != type)
        {
            // Check if the position is the level's start location
            if (level.StartPosition == position)
            {
                // Abort if the new type is not a valid start location
                if (!type.IsValidStartPosition) return;
            }

            RemoveLinksToPosition(position);
            AddTile();
        }

        void AddTile()
        {
            level.Tiles.Remove(position);
            level.Tiles.Add(position, new(type));
            RegenerateLevel();
        }
    }

    private LinkIndicator HoveredLink()
    {
        foreach (var link in linkIndicators)
        {
            if (link.MouseOver) return link;
        }
        return null;
    }

    private void RemoveLinksToPosition(Vector2Int position)
    {
        foreach (var (_, tile) in level.Tiles)
        {
            tile.Links?.RemoveAll(link => link == position);
        }
    }

    private void PositionStartIndicator()
    {
        levelStartIndicator.transform.position = GridUtilities.GridToWorldPos(level.StartPosition);
    }

    /*
        Methods to be used as UnityEvents, assigned to LevelEditorTools via the inspector
        These methods make use of the field 'targetPosition'
    */

    // ===========
    // Eraser Tool
    // ===========

    public void EraseTile()
    {
        // Do not erase a tile that is used as the level's start position
        if (level.StartPosition == targetPosition) return;

        RemoveLinksToPosition(targetPosition);
        level.Tiles.Remove(targetPosition);
        RegenerateLevel();
    }

    // ============
    // Tile Tools
    // ============

    public void AddTile()
    {
        if (currentTool is LevelEditorTileTool tileTool)
        {
            AddTile(tileTool.TileType, targetPosition);
        }
    }

    // ============
    // Entity Tools
    // ============
    public void AddEntity()
    {
        if (currentTool is LevelEditorEntityTool entityTool)
        {
            AddEntity(entityTool.EntityType, targetPosition);
        }
    }

    private void AddEntity(EntityType type, Vector2Int position)
    {
        if (!level.Entities.ContainsKey(position))
        {
            AddEntity();
        }
        else if (level.Entities[position].Type != type)
        {
            AddEntity();
        }

        void AddEntity()
        {
            level.Entities.Remove(position);
            level.Entities.Add(position, new(type));
            RegenerateLevel();
        }
    }

    // ==========
    // Link Tools
    // ==========

    public void EnterLinkEditingMode()
    {
        ShowingLinks = true;
    }

    public void ExitLinkEditingMode()
    {
        ShowingLinks = false;
    }

    public void StartDrawingLink()
    {
        linkStart = level.Tiles.ContainsKey(targetPosition) ? targetPosition : null;
    }

    public void CreateLinkPreview()
    {
        if (linkStart is Vector2Int startPosition)
        {
            var start = GridUtilities.GridToWorldPos(startPosition);
            var end = targetWorldPosition;

            if (linkPreview == null)
            {
                linkPreview = Instantiate(prefabs.linkIndicator, Vector3.zero, Quaternion.identity);
                linkPreview.InitialiseAsPreview(start, end);
            }
            else
            {
                linkPreview.SetPositions(start, end);
            }
        }
    }

    public void CreateLink()
    {
        // Destroy the link preview
        if (linkPreview != null) Destroy(linkPreview.gameObject);

        // Check that link start was defined
        if (linkStart is Vector2Int startPosition)
        {
            // Prevent self-links
            if (targetPosition == startPosition) return;

            // Check link starts at a tile
            if (!level.Tiles.ContainsKey(startPosition)) return;

            // Prevent links to non-existant tiles
            if (!level.Tiles.ContainsKey(targetPosition)) return;

            // Read start and target tiles
            Tile startTile = level.Tiles[startPosition];
            Tile targetTile = level.Tiles[targetPosition];

            // Check that the tiles are compatible
            if (!startTile.Type.ValidLinkTargets.Contains(targetTile.Type))
            {
                Debug.Log($"Attempting to create a link from a {startTile.Type.DisplayName} tile to a {targetTile.Type.DisplayName} tile.");
                return;
            }

            // Create the link
            level.Tiles[startPosition].Links.Add(targetPosition);
            GenerateLinkIndicators();
        }
    }

    public void DeleteLink()
    {
        if (targetLink != null)
        {
            if (level.Tiles.ContainsKey(targetLink.Start))
            {
                level.Tiles[targetLink.Start].Links.Remove(targetLink.End);
            }
        }

        GenerateLinkIndicators();
    }

    // ===========
    // State Tools
    // ===========

    public void EnterStateEditingMode()
    {
        ShowingState = true;
    }

    public void ExitStateEditingMode()
    {
        ShowingState = false;
    }

    public void IncrementState()
    {
        if (level.Tiles.ContainsKey(targetPosition))
        {
            var targetTile = level.Tiles[targetPosition];
            targetTile.IncrementInitialState();
            level.Tiles[targetPosition] = targetTile;
        }

        GenerateStateIndicators();
    }

    public void DecrementState()
    {
        if (level.Tiles.ContainsKey(targetPosition))
        {
            var targetTile = level.Tiles[targetPosition];
            targetTile.DecrementInitialState();
            level.Tiles[targetPosition] = targetTile;
        }

        GenerateStateIndicators();
    }

    // ===========
    // Level Start
    // ===========

    public void SetLevelStart()
    {
        if (level.Tiles.ContainsKey(targetPosition) && level.Tiles[targetPosition].Type.IsValidStartPosition)
        {
            level.StartPosition = targetPosition;
            PositionStartIndicator();
        }
    }

    // =======
    // Actions
    // =======

    public void SaveLevel()
    {
        if (level.IsValidLevel)
        {
            LevelFileManager.ExportLevelAsJson(level, "ExportedLevel");
        }
    }

    public void PreviewLevel()
    {
        LevelToPreview = level;
        SceneManager.LoadScene("LevelPlayer");
    }
}
