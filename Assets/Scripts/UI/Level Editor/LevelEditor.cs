
using System.Collections.Generic;
using System.Linq;
using Demo;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class LevelEditor: MonoBehaviour
    {
        public LevelBuilder LevelBuilder;

        public Level startingLevel;

        public Transform levelParent;

        private Level level;

        [SerializeField]
        private LevelEditorTool currentTool;

        private LevelEditorTool[] allTools;

        private Vector2Int targetPosition;

        private bool hasDragged;

        private void Start()
        {
            allTools = FindObjectsByType<LevelEditorTool>(FindObjectsSortMode.InstanceID);
            currentTool.Select();
            level = startingLevel;
            LevelBuilder.BuildLevel(levelParent, level);
        }

        private void Update()
        {
            if (currentTool != null && !Utilities.MouseOverUI)
            {
                // Read mouse position
                Vector2Int mousePosition = GridUtilities.GetMouseGridPos();

                if (Input.GetMouseButtonDown(0))
                {
                    targetPosition = mousePosition;
                    hasDragged = false;
                    currentTool.OnSquareMouseDown.Invoke();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (!hasDragged)
                    {
                        currentTool.OnSquareClick.Invoke();
                    }
                }
                else if (Input.GetMouseButton(0) && targetPosition != mousePosition)
                {
                    targetPosition = mousePosition;
                    hasDragged = true;
                    currentTool.OnSquareDrag.Invoke();
                }
            }
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
        }

        /*
            Methods to be used as UnityEvents, assigned to LevelEditorTools via the inspector
            These methods make use of the field 'targetPosition'
        */

        public void EraseTile()
        {
            level.tiles.RemoveAll(pair => pair.position == targetPosition);
            RegenerateLevel();
        }

        public void AddFloorTile()
        {
            AddTile(TileType.Floor);
        }

        public void AddPortal()
        {
            AddTile(TileType.Portal);
        }

        public void AddMovingPlatform()
        {
            AddTile(TileType.MovingPlatform);
        }

        private void AddTile(TileType type)
        {    
            // Remove any existing tiles at this position
            level.tiles.RemoveAll(pair => pair.position == targetPosition);

            TilePositionPair newTilePositionPair = new TilePositionPair();
            newTilePositionPair.position = targetPosition;
            newTilePositionPair.tile = new TileBuildData(type);
            
            level.tiles.Add(newTilePositionPair);
            RegenerateLevel();
        }
    }

}