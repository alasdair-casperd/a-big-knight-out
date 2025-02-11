using UnityEngine;
using System;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField]
    private UI.Slider restartPrompt;

    [SerializeField]
    private UI.Fader transitionFader;

    /// <summary>
    /// Show or hide the restart prompt
    /// </summary>
    public void SetRestartPrompt(bool visible)
    {
        if (restartPrompt != null)
        {
            if (visible) restartPrompt.Show();
            else restartPrompt.Dismiss();
        }
    }

    /// <summary>
    /// Fade out to black, perform an action, then fade back in
    /// </summary>
    /// <param name="action"></param>
    public void FadeThroughAction(Action action)
    {
        transitionFader.Show(onComplete: () =>
        {
            action();
            transitionFader.Dismiss();
        });
    }    

    /// <summary>
    /// Fade out to black then perform an action
    /// </summary>
    /// <param name="action"></param>
    public void FadeBeforeAction(Action action)
    {
        transitionFader.Show(onComplete: () =>
        {
            action();
        });
    }    
}
