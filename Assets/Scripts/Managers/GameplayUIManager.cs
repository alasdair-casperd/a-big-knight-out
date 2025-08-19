using UnityEngine;
using System;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField]
    private UI.Slider restartPrompt;

    [SerializeField]
    private UI.Fader pauseMenu;

    [SerializeField]
    private GameObject quitToMenuButton;

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

    public void SetPauseMenu(bool visible, Level currentLevel)
    {
        if (pauseMenu != null)
        {
            if (visible) pauseMenu.Show();
            else pauseMenu.Dismiss();
        }

        // Auto-select the first item
        var menuItems = pauseMenu.gameObject.GetComponentsInChildren<Selectable>();
        if (menuItems.Length > 0)
        {
            menuItems[0].Select();
        }

        // Show/hide the quit to menu button depending on whether the current level is the menu
        quitToMenuButton.SetActive(currentLevel.Name != "Menu");
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
