using UnityEngine;

/// <summary>
/// A square used to launch a specified level
/// </summary>
public class LockSquare : Square
{
    public override TileType Type => TileType.Lock;

    public override bool BlocksJump
    {
        get
        {
            return ProgressStore.TotalLevelsCompleted() < State;
        }
        protected set { }
    }

    public GameObject lockedGraphics;
    public GameObject openGraphics;

    private void Start()
    {
        UpdateGraphics();
    }

    // Passable only if the enough levels have been completed
    public override bool IsPassable
    {
        get
        {
            return ProgressStore.TotalLevelsCompleted() >= State;
        }
        protected set { }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    public override void UpdateGraphics()
    {
        lockedGraphics.SetActive(!IsPassable);
        openGraphics.SetActive(IsPassable);
    }
}
