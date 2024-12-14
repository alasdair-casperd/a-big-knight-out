
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelEditor: MonoBehaviour
    {
        public LevelBuilder LevelBuilder;

        [SerializeField]
        private TextAsset startingLevelFile;

        public static Level LastEditedLevel;

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
            set{ if (linksContainer != null) linksContainer.SetActive(value); }
        }

        private bool ShowingState
        {
            set{ if (stateContainer != null) stateContainer.SetActive(value); }
        }

        private Vector2Int? linkStart;

        private LinkIndicator linkPreview;

        private List<LinkIndicator> linkIndicators = new();

        private List<StateIndicator> stateIndicators = new();

        private void Start()
        {
            allTools = FindObjectsByType<LevelEditorTool>(FindObjectsSortMode.InstanceID);

            if (startingLevelFile == null)
            {
                level = new();
            }   
            else
            {
                try
                {
                    level = LevelFileUtilities.Parse(startingLevelFile.text);
                }
                catch
                {
                    Debug.LogError("Failed to parse level file");
                    level = new();
                }
            }

            LevelBuilder.BuildLevel(levelParent, level);
            LastEditedLevel = level;

            levelStartIndicator = Instantiate(prefabs.levelStartIndicator);
            PositionStartIndicator();

            GenerateLinkIndicators();
            GenerateStateIndicators();

            ShowingLinks = false;
            ShowingState = false;
            currentTool.Select();
        }

        private void Update()
        {
            TriageMouseAction();
        }

        private void TriageMouseAction()
        {
            if ((Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && currentTool != null && !Utilities.MouseOverUI)
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

        public void DeselectTool()
        {
            Debug.Log("testing");
        }

        private void RegenerateLevel()
        {
            LevelBuilder.BuildLevel(levelParent, level, null, 0.5f);
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

            foreach (var (position, tile) in level.tiles)
            {
                foreach (var link in tile.links)
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

            foreach (var (position, tile) in level.tiles)
            {
                if (tile.type.IsMultiState)
                {
                    var stateIndicator = Instantiate(prefabs.stateIndicator);
                    stateIndicator.transform.parent = stateContainer.transform;
                    stateIndicator.transform.position = GridUtilities.GridToWorldPos(position);
                    stateIndicator.Number = tile.initialState;
                    stateIndicators.Add(stateIndicator);
                }
            }
        }

        private void AddTile(TileType type, Vector2Int position)
        {    
            if (!level.tiles.ContainsKey(position))
            {
                AddTile();
            }
            else if (level.tiles[position].type != type)
            {
                RemoveLinksToPosition(position);
                AddTile();
            }

            void AddTile()
            {
                level.tiles.Remove(position);
                level.tiles.Add(position, new(type));
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
            foreach (var (_, tile) in level.tiles)
            {
                tile.links?.RemoveAll(link => link == position);
            }
        }

        private void PositionStartIndicator()
        {
            levelStartIndicator.transform.position = GridUtilities.GridToWorldPos(level.startPosition);
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
            level.tiles.Remove(targetPosition);
            RegenerateLevel();
        }

        // ============
        // Square Tools
        // ============

        public void AddFloorTile()
        {
            AddTile(TileType.Floor, targetPosition);
        }

        public void AddPortal()
        {
            AddTile(TileType.Portal, targetPosition);
        }

        public void AddMovingPlatform()
        {
            AddTile(TileType.MovingPlatform, targetPosition);
        }

        public void AddSpikes()
        {
            AddTile(TileType.Spikes, targetPosition);
        }

        public void AddFallingFloor()
        {
            AddTile(TileType.FallingFloor, targetPosition);
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
            linkStart = level.tiles.ContainsKey(targetPosition) ? targetPosition : null;
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
                if (!level.tiles.ContainsKey(startPosition)) return;

                // Prevent links to non-existant tiles
                if (!level.tiles.ContainsKey(targetPosition)) return;
                
                // Read start and target tiles
                TileBuildData startTile = level.tiles[startPosition];
                TileBuildData targetTile = level.tiles[targetPosition];

                // Check that the tiles are compatible
                if (!startTile.type.ValidLinkTargets.Contains(targetTile.type))
                {
                    Debug.Log($"Attempting to create a link from a {startTile.type.DisplayName} tile to a {targetTile.type.DisplayName} tile");
                    return;
                }

                // Create the link
                level.tiles[startPosition].links.Add(targetPosition);
                GenerateLinkIndicators();
            }
        }

        public void DeleteLink()
        {
            if (targetLink != null)
            {
                if (level.tiles.ContainsKey(targetLink.Start))
                {
                    level.tiles[targetLink.Start].links.Remove(targetLink.End);
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
            if (level.tiles.ContainsKey(targetPosition))
            {
                var targetTile = level.tiles[targetPosition];
                targetTile.IncrementInitialState();
                level.tiles[targetPosition] = targetTile;
            }

            GenerateStateIndicators();
        }

        public void DecrementState()
        {
            if (level.tiles.ContainsKey(targetPosition))
            {
                var targetTile = level.tiles[targetPosition];
                targetTile.DecrementInitialState();
                level.tiles[targetPosition] = targetTile;
            }

            GenerateStateIndicators();
        }

        // ===========
        // Level Start
        // ===========

        public void SetLevelStart()
        {
            if (level.tiles.ContainsKey(targetPosition))
            {
                level.startPosition = targetPosition;
                PositionStartIndicator();
            }
        }

        // =======
        // Actions
        // =======

        public void SaveLevel()
        {
            string path = Application.dataPath + "/Levels/ExportedLevel.txt";
            File.WriteAllText(path, LevelFileUtilities.Export(level));
        }

        public void SafeAndPreviewLevel()
        {
            SaveLevel();
            LastEditedLevel = level;
            
            SceneManager.LoadScene("LevelPlayer");
        }
    }
}