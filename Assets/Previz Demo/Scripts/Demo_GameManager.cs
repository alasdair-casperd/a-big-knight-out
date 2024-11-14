using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Demo {
    public class GameManager : MonoBehaviour
    {
        public float animationTime = 0.1f;

        private static GameManager _instance;
        
        public static float AnimationTime {
            get {
                if (_instance == null) _instance = FindAnyObjectByType<GameManager>();
                return _instance.animationTime;
            }
        }

        private Player player;
        private Tile[] tiles;
        private PathFollower[] pathFollowers;
        private Alternator[] alternators;
        private bool levelHasMovingTiles = false;

        private int totalMovesMade = 0;
        public bool movementLocked = false;

        void Start()
        {
            //AudioManager.Play(AudioManager.BackgroundSounds.demoBacking);

            player = FindAnyObjectByType<Player>();
            tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
            pathFollowers = FindObjectsByType<PathFollower>(FindObjectsSortMode.None);
            alternators = FindObjectsByType<Alternator>(FindObjectsSortMode.None);

            // Set the player's current tile
            foreach (var tile in tiles)
            {
                Vector3 playerToTile = tile.transform.position - player.transform.position;
                playerToTile.y = 0;
                if (playerToTile.magnitude < 0.1)
                {
                    player.startingTile = tile;
                    player.currentTile = tile;
                }

                if (tile.moving)
                {
                    levelHasMovingTiles = true;
                }
            }
        }

        void Update()
        {
            // If the tab key is pressed, manually trigger a turn without movement
            if (Input.GetKeyDown("tab"))
            {
                StepLevel();
            }

            // Handle user movement on click
            if (Input.GetMouseButtonDown(0) && !movementLocked)
            {
                HandlePlayerMovementInput();
            }

            // Reset the level if the escape key is pressed
            if (Input.GetKeyDown("escape"))
            {
                Reset();
            }
        }

        // Check if a tile is a valid move for the player
        public bool IsValidMove(Tile tile)
        {
            Vector3 movementVector = tile.transform.position - player.currentTile.transform.position;
            movementVector.y = 0;
            return Math.Abs(movementVector.magnitude - Math.Sqrt(5)) < 0.1 && !tile.destroyed;
        }

        // Reset the level
        public void Reset()
        {
            if (totalMovesMade > 0)
            {
                movementLocked = false;

                AudioManager.Play(AudioManager.SoundEffects.click);
                
                totalMovesMade = 0;
                player.Reset();
                
                foreach (var tile in tiles)
                {
                    tile.Reset();
                }

                foreach (var follower in pathFollowers)
                {
                    follower.Reset();
                }

                foreach (var alternator in alternators)
                {
                    alternator.Reset();
                }
            }
        }


        // Handle a mouse click, moving the player if necessary
        private void HandlePlayerMovementInput()
        {

            // Cast a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a gameObject with a collider
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the gameObject hit is a tile
                if (hit.collider.gameObject.TryGetComponent<Tile>(out var tile))
                {

                    // Check if the move is valid
                    if (IsValidMove(tile))
                    {
                        totalMovesMade++;
                        AudioManager.Play(AudioManager.SoundEffects.thud);
                        player.Move(tile);
                        StepLevel();
                    }
                }
            }
        }

        // Trigger the actions of all entities in the level
        private void StepLevel()
        {
            foreach (var follower in pathFollowers)
            {
                follower.Step();
            }

            foreach (var alternator in alternators)
            {
                alternator.Step();
            }

            if (levelHasMovingTiles)
            {
                movementLocked = true;

                LeanTween.delayedCall(AnimationTime, () => {
                    foreach (var tile in tiles)
                    {
                        tile.Step();
                    }
                });

                LeanTween.delayedCall(2 * AnimationTime, () => {
                    movementLocked = false;
                });
            }
        }
    }
}
