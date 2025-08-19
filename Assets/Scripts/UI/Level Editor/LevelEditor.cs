
using System.Collections.Generic;
using UnityEngine;
using UI;

/// <summary>
/// A class to manage the user input involved in editing a level
/// </summary>
[RequireComponent(typeof(LevelHandler))]
[RequireComponent(typeof(GameManager))]
public class LevelEditor : MonoBehaviour
{
  // The level handler
  private LevelHandler LevelHandler;

  // Level json file to load on start
  [SerializeField]
  private TextAsset startingLevelJSON;

  // Prefabs
  [SerializeField]
  private Prefabs prefabs;

  // The currently selected sidebar tool
  public SidebarTool SidebarTool = null;

  // The sidebar tool selected before previewing the level, if any
  private SidebarTool previousSidebarTool = null;

  // Sidebars
  public Slider toolsSidebar;

  // Browsers
  public TileBrowser TileBrowser;
  public EntityBrowser EntityBrowser;

  // The grid position currently targeted (usually where the mouse is hovered)
  private Vector2Int targetPosition;

  // The world position currently targeted (usually where the mouse is hovered)
  private Vector3 targetWorldPosition;

  // Is the user performing a drag action?
  private bool hasDragged;

  // Is the level currently being previewed?
  public bool previewingLevel;

  // The dialogue manager used to display dialogues
  public DialogueManager DialogueManager;

  /*
      Link editing properties
  */

  // Are link indicators being displayed?
  private bool ShowingLinks
  {
    set { if (linksContainer != null) linksContainer.SetActive(value); }
  }

  // The location at which the user started drawing a link, if any
  private Vector2Int? linkStart;

  // A gameObject to contain all link indicators
  private GameObject linksContainer;

  // The current link preview being created, if any
  private LinkIndicator linkPreview;

  // The link indicator that is being hovered, if any
  private LinkIndicator targetLink;

  // A list of all link indicators
  private List<LinkIndicator> linkIndicators = new();

  /*
      Rotation editing properties
  */

  // Are rotation indicators being displayed?
  private bool ShowingRotations
  {
    set { if (rotationsContainer != null) rotationsContainer.SetActive(value); }
  }

  // A gameObject to contain all rotation indicators
  private GameObject rotationsContainer;

  // A dictionary of all rotation indicators
  private Dictionary<Vector2Int, RotationIndicator> rotationIndicators = new();

  /*
      Browser properties
  */

  // The currently selected tile type, if any
  private TileType? selectedTileType = null;

  // The currently selected entity type, if any
  private EntityType? selectedEntityType = null;

  // Is the player browser item selected?
  private bool playerSelected = false;

  // Is the moving platforms browser item selected?
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

    if (startingLevelJSON == null)
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
    if (selectedEntityType is EntityType type) LevelHandler.PlaceEntity(targetPosition, type);
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
    else
    {
      return;
    }

    if (rotationIndicators[targetPosition] != null)
    {
      RotationIndicator targetIndicator = rotationIndicators[targetPosition];
      Quaternion currentRotation = targetIndicator.transform.rotation;
      Quaternion targetRotation = currentRotation * Quaternion.Euler(0, 90, 0);
      LeanTween.rotate(rotationIndicators[targetPosition].gameObject, targetRotation.eulerAngles, 0.05f)
          .setOnComplete(GenerateRotationIndicators);
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

  public void EnterLinksMode()
  {
    GenerateLinkIndicators();
    ShowingLinks = true;
  }

  public void ExitLinksMode()
  {
    ShowingLinks = false;
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

  // ================
  // Rotation Actions
  // ================

  public void EnterRotationMode()
  {
    GenerateRotationIndicators();
    ShowingRotations = true;
  }

  public void ExitRotationMode()
  {
    ShowingRotations = false;
  }

  private void GenerateRotationIndicators()
  {
    if (rotationsContainer == null)
    {
      rotationsContainer = new GameObject("Rotations");
      rotationsContainer.transform.parent = transform;
    }

    foreach (var (_, rotationIndicator) in rotationIndicators)
    {
      Destroy(rotationIndicator.gameObject);
    }

    rotationIndicators = new();

    foreach (var (position, entity) in LevelHandler.level.Entities)
    {
      var rotationIndicator = Instantiate(prefabs.rotationIndicator.gameObject).GetComponent<RotationIndicator>();
      rotationIndicator.transform.parent = rotationsContainer.transform;
      rotationIndicator.Configure(position, entity.Direction, entity.Type, animateRotation: false);
      rotationIndicators.Add(position, rotationIndicator);
    }

    foreach (var (position, direction) in LevelHandler.level.MovingPlatforms)
    {
      var rotationIndicator = Instantiate(prefabs.rotationIndicator.gameObject).GetComponent<RotationIndicator>();
      rotationIndicator.transform.parent = rotationsContainer.transform;
      rotationIndicator.Configure(position, direction, entityType: null, animateRotation: false);
      rotationIndicators.Add(position, rotationIndicator);
    }
  }

  // ======
  // Export
  // ======

  public void ExportLevel()
  {
    Dialogue.HeaderData headerData = new()
    {
      title = "Export Level?",
      body = "Your level will be saved as ExportedLevel.json in /Levels."
    };

    Dialogue.ButtonData primary = new()
    {
      text = "Export",
      action = () =>
      {

        Level level = LevelHandler.level;
        if (level.IsValidLevel)
        {
          LevelFileManager.ExportLevelAsJson(level, "ExportedLevel");
        }
      }
    };

    DialogueManager.Create(headerData, primary);
  }

  // =====
  // Reset
  // =====

  public void ResetLevel()
  {
    Dialogue.HeaderData headerData = new()
    {
      title = "Reset Level?",
      body = "You will lose any unsaved changes."
    };

    Dialogue.ButtonData primary = new()
    {
      text = "Reset",
      action = () =>
      {
        LevelHandler.ClearLevel();
        LevelHandler.LoadLevel(new Level(startPosition: Vector2Int.zero));
        GenerateLinkIndicators();
        GenerateRotationIndicators();
      }
    };

    DialogueManager.Create(headerData, primary);
  }

  // =======
  // Preview
  // =======

  public void StartLevelPreview()
  {
    if (previewingLevel) return;
    previewingLevel = true;

    previousSidebarTool = SidebarTool;

    if (SidebarTool != null)
    {
      SidebarTool.GetComponent<Selector>().Deselect();
      SidebarTool = null;
    }

    toolsSidebar.Dismiss();
    LevelHandler.ClearLevel();
    GetComponent<GameManager>().Initialise(LevelHandler.level);
  }

  public void EndLevelPreview()
  {
    if (!previewingLevel) return;
    previewingLevel = false;

    SidebarTool = previousSidebarTool;
    if (SidebarTool != null) SidebarTool.GetComponent<Selector>().Select();

    GetComponent<GameManager>().Clear();
    LevelHandler.RegenerateLevel();
    toolsSidebar.Show();
  }

  // ===============
  // Not implemented
  // ===============

  public void NotImplementedDialogue()
  {
    Dialogue.HeaderData headerData = new()
    {
      title = "Work in Progress",
      body = "This level editor feature has not been implemented yet."
    };

    DialogueManager.Create(headerData);
  }
}
