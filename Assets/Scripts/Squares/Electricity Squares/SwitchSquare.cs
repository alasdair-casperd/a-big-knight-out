using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// A square containing a switch that can be toggled on or off
/// </summary>
public class SwitchSquare : Square
{
    // Graphics representing the on and off states
    // TODO: Switch to animation based transitioning when graphics are finalised
    public GameObject onGraphics;
    public GameObject offGraphics;

    public override TileType Type =>  TileType.Switch;

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change if switch is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }
    
    // Is the switch currently on?
    public bool IsOn
    {
        get
        {
            return State == 1 || IsReceivingCharge;
        }
        set
        {
            State = value ? 1 : 0;
            UpdateOutgoingCharge();
        }
    }

    // To calculate this switch charge, we just need to know if it is on
    public override bool CalculateCharge()
    {
        return IsOn;
    }

    // Toggle the switch on player land
    public override void OnPlayerLand()
    {
        IsOn = !IsOn;
        UpdateGraphics();

        // Play a click sound effect
        AudioManager.Play(AudioManager.SoundEffects.click);
    }

    // Update visuals according to the state of the switch
    public override void UpdateGraphics()
    {
        onGraphics.SetActive(IsOn);
        offGraphics.SetActive(!IsOn);
    }

    // Update the graphics when the charge changes
    public override void OnChargeChanged()
    {
        UpdateGraphics();
    }
}
