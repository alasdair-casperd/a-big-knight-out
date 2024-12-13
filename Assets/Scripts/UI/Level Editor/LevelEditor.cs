
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class LevelEditor: MonoBehaviour
    {
        public LevelBuilder LevelBuilder;

        public Level startingLevel;

        public Transform levelParent;

        [SerializeField]
        private Prefabs prefabs;

        private WorkingLevel workingLevel;

        [SerializeField]
        private LevelEditorTool currentTool;

        private LevelEditorTool[] allTools;

        private Vector2Int targetPosition;
        
        private Vector3 targetWorldPosition;

        private LinkIndicator targetLink;

        private bool hasDragged;

        private GameObject linksContainer;

        private GameObject stateContainer;

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
            
            workingLevel = new WorkingLevel(startingLevel);
            LevelBuilder.BuildLevel(levelParent, workingLevel);
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
            LevelBuilder.BuildLevel(levelParent, workingLevel, null, 0.5f);
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

            foreach (var (position, tile) in workingLevel.tiles)
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

            foreach (var (position, tile) in workingLevel.tiles)
            {
                var stateIndicator = Instantiate(prefabs.stateIndicator);
                stateIndicator.transform.parent = stateContainer.transform;
                stateIndicator.transform.position = GridUtilities.GridToWorldPos(position);
                stateIndicator.Number = tile.initialState;
                stateIndicators.Add(stateIndicator);
            }
        }

        private void AddTile(TileType type, Vector2Int position)
        {    
            if (!workingLevel.tiles.ContainsKey(position))
            {
                AddTile();
            }
            else if (workingLevel.tiles[position].type != type)
            {
                RemoveLinksToPosition(position);
                AddTile();
            }

            void AddTile()
            {
                workingLevel.tiles[position] = new(type);
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
            foreach (var (_, tile) in workingLevel.tiles)
            {
                tile.links?.RemoveAll(link => link == position);
            }
        }

        private void AddToState(Vector2Int position, int amount)
        {
            if (workingLevel.tiles.ContainsKey(position))
            {
                var targetTile = workingLevel.tiles[position];
                targetTile.initialState += amount;
                workingLevel.tiles[position] = targetTile;
            }

            GenerateStateIndicators();
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
            workingLevel.tiles.Remove(targetPosition);
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
            linkStart = workingLevel.tiles.ContainsKey(targetPosition) ? targetPosition : null;
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
            if (linkPreview != null) Destroy(linkPreview.gameObject);

            if (linkStart is Vector2Int startPosition)
            {
                if (targetPosition != startPosition && workingLevel.tiles.ContainsKey(targetPosition))
                {
                    workingLevel.tiles[startPosition].links.Add(targetPosition);
                    GenerateLinkIndicators();
                }
            }
        }

        public void DeleteLink()
        {
            if (targetLink != null)
            {
                if (workingLevel.tiles.ContainsKey(targetLink.Start))
                {
                    workingLevel.tiles[targetLink.Start].links.Remove(targetLink.End);
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
            AddToState(targetPosition, 1);
        }

        public void DecrementState()
        {
            AddToState(targetPosition, -1);
        }

        // ===========
        // Load & Save
        // ===========

        public void SaveLevel()
        {
            workingLevel.ExportTo(startingLevel);
        }
    }
}