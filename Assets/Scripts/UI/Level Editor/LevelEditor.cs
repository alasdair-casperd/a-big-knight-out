
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;
using System;
using Unity.VisualScripting;
using System.Data.Common;

[RequireComponent(typeof(LevelHandler))]
public class LevelEditor : MonoBehaviour
{
    // TO REMOVE
    public static Level LevelToPreview;

    private LevelHandler LevelHandler;

    [SerializeField]
    private TextAsset startingLevelJSON;

    [SerializeField]
    private Prefabs prefabs;

    public SidebarTool SidebarTool = null;

    public TileBrowser TileBrowser;
    public EntityBrowser EntityBrowser;

    private Vector2Int targetPosition;

    private Vector3 targetWorldPosition;

    private LinkIndicator targetLink;

    private bool hasDragged;

    private GameObject linksContainer;

    private bool ShowingWiringLinks
    {
        set { if (linksContainer != null) linksContainer.SetActive(value); }
    }

    private Vector2Int? linkStart;

    private LinkIndicator linkPreview;

    private List<LinkIndicator> linkIndicators = new();

    private TileType? selectedTileType = null;
    private EntityType? selectedEntityType = null;
    private bool playerSelected = false;
    private bool movingPlatformsSelected = false;

    private void Start()
    {
        LevelHandler = GetComponent<LevelHandler>();
        LoadLevel();
    }

    /// <summary>
    /// Parse and load the provided level file (if any)
    /// </summary>
    private void LoadLevel()
    {
        Level levelToBuild;
        if (LevelToPreview != null)
        {
            levelToBuild = LevelToPreview;
        }
        else if (startingLevelJSON == null)
        {
            levelToBuild = new Level(startPosition: Vector2Int.zero);
        }
        else
        {
            try
            {
                levelToBuild = LevelFileManager.ParseLevelFromJSON(startingLevelJSON.text);
            }
            catch
            {
                Debug.LogError("Failed to parse level file");
                levelToBuild = new Level(startPosition: Vector2Int.zero);
            }
        }
        LevelHandler.LoadLevel(levelToBuild);
    }

    /// <summary>
    /// The link indicator that is being hovered, if any
    /// </summary>
    /// <returns></returns>
    private LinkIndicator HoveredLink()
    {
        foreach (var link in linkIndicators)
        {
            if (link.MouseOver) return link;
        }
        return null;
    }

    private void Update()
    {
        TriageMouseAction();
    }

    /// <summary>
    /// Determine which mouse action was performed this frame, and call the associated method
    /// </summary>
    private void TriageMouseAction()
    {
        // Check mouse it not block by a UI element
        if (MouseUtilities.MouseOverUI) return;
        
        // Check a sidebar tool is selected
        if (SidebarTool == null) return;

        // Determine tool type
        if (SidebarTool.toolType == SidebarTool.ToolType.Tile)
        {    
            if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            {
                // Read mouse position
                Vector2Int mousePosition = GridUtilities.GetMouseGridPos();

                if (Input.GetMouseButtonDown(0))
                {
                    TileMouseDown(mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (!hasDragged)
                    {
                        TileMouseClick(mousePosition);
                    }

                    TileMouseUp(mousePosition);
                }
                else
                {
                    if (targetPosition != mousePosition)
                    {
                        TileMouseDrag(mousePosition);
                    }

                    TileMouseHeld(mousePosition);
                }
            }
        }
        else if (SidebarTool.toolType == SidebarTool.ToolType.Link)
        {
            if (!Input.GetMouseButton(0)) return;

            var hoveredLink = HoveredLink();
            if (hoveredLink != null) LinkTouch(hoveredLink);
        }
    }
    
    /*
        Mouse action functions
    */

    private void TileMouseDown(Vector2Int mousePosition)
    {
        targetPosition = mousePosition;
        hasDragged = false;
        SidebarTool.OnTileMouseDown.Invoke();
    }

    private void TileMouseClick(Vector2Int mousePosition)
    {
        targetPosition = mousePosition;
        hasDragged = false;
        SidebarTool.OnTileClick.Invoke();
    }

    private void TileMouseUp(Vector2Int mousePosition)
    {
        SidebarTool.OnTileMouseUp.Invoke();
    }

    private void TileMouseDrag(Vector2Int mousePosition)
    {
        targetPosition = mousePosition;
        hasDragged = true;
        SidebarTool.OnTileDrag.Invoke();
    }

    private void TileMouseHeld(Vector2Int mousePosition)
    {
        targetWorldPosition = MouseUtilities.GetMouseWorldPos();
        targetWorldPosition.y = 0;
        SidebarTool.OnTileMouseHeld.Invoke();
    }

    private void LinkTouch(LinkIndicator link)
    {
        targetLink = link;
        SidebarTool.OnLinkTouch.Invoke();
    }
    /*
        Tool and browser action functions
    */

    // ====================
    // Sidebar tool actions
    // ====================

    public void TileToolAction()
    {
        if (selectedTileType is TileType type) LevelHandler.AddTile(targetPosition, type);
        else if (movingPlatformsSelected) LevelHandler.PlaceMovingPlatform(targetPosition, direction: 0);
    }

    public void EntityToolAction()
    {
        // TODO: Implement adding entities
        // if (selectedEntityType is EntityType type) LevelHandler.AddEntity(targetPosition, type);

        if (playerSelected) LevelHandler.PlacePlayer(targetPosition);
    }

    public void EditAction()
    {
        // TODO: Check two state
        LevelHandler.IncrementState(targetPosition);
    }

    public void EraseAction()
    {
        if (LevelHandler.level.MovingPlatforms.ContainsKey(targetPosition))
        {
            LevelHandler.DeleteMovingPlatform(targetPosition);
        }
        else if (LevelHandler.level.Entities.ContainsKey(targetPosition))
        {
            LevelHandler.DeleteEntity(targetPosition);
        }
        else if (LevelHandler.level.Tiles.ContainsKey(targetPosition))
        {
            LevelHandler.DeleteTile(targetPosition);
        }
    }

    public void RotateAction()
    {
        if (LevelHandler.level.Entities.ContainsKey(targetPosition))
        {
            LevelHandler.RotateEntity(targetPosition);
        }
        else if (LevelHandler.level.MovingPlatforms.ContainsKey(targetPosition))
        {
            LevelHandler.RotateMovingPlatform(targetPosition);
        }

    }

    // ===============
    // Browser Actions
    // ===============

    private void DeselectBrowserItems()
    {
        selectedEntityType = null;
        selectedTileType = null;
        playerSelected = false;
        movingPlatformsSelected = false;
    }

    public void SelectTileType(TileType type)
    {
        DeselectBrowserItems();
        selectedTileType = type;
    }

    public void SelectEntityType(EntityType type)
    {
        DeselectBrowserItems();
        selectedEntityType = type;
    }

    public void SelectPlayer()
    {
        DeselectBrowserItems();
        playerSelected = true;
    }

    public void SelectMovingPlatforms()
    {
        DeselectBrowserItems();
        movingPlatformsSelected = true;
    }

    public void SetTileBrowserVisibility(bool visible)
    {
        Slider slider = TileBrowser.GetComponent<Slider>();
        if (visible) slider.Show();
        else slider.Dismiss();
    }

    public void SetEntityBrowserVisibility(bool visible)
    {
        Slider slider = EntityBrowser.GetComponent<Slider>();
        if (visible) slider.Show();
        else slider.Dismiss();
    }

    // ============
    // Link Actions
    // ============

    public void EnterWiringMode()
    {
        GenerateLinkIndicators();
        ShowingWiringLinks = true;
    }

    public void ExitWiringMode()
    {
        ShowingWiringLinks = false;
    }

    public void StartDrawingLink()
    {
        linkStart = LevelHandler.level.Tiles.ContainsKey(targetPosition) ? targetPosition : null;
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

        foreach (var (position, tile) in LevelHandler.level.Tiles)
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

    public void CreateLink()
    {
        // Destroy the link preview
        if (linkPreview != null) Destroy(linkPreview.gameObject);

        // Create the link
        if (linkStart is Vector2Int startPosition) LevelHandler.CreateLink(startPosition, targetPosition);

        GenerateLinkIndicators();
    }

    // public void DeleteLink()
    // {
    //     if (targetLink != null)
    //     {
    //         if (LevelHandler.level.Tiles.ContainsKey(targetLink.Start))
    //         {
    //             LevelHandler.level.Tiles[targetLink.Start].Links.Remove(targetLink.End);
    //         }
    //     }

    //     GenerateLinkIndicators();
    // }

    // =======
    // Actions
    // =======

    public void ExportLevel()
    {
        Level level = LevelHandler.level;
        if (level.IsValidLevel)
        {
            LevelFileManager.ExportLevelAsJson(level, "ExportedLevel");
        }
    }

    public void PreviewLevel()
    {
        LevelToPreview = LevelHandler.level;
        SceneManager.LoadScene("LevelPlayer");
    }
}
