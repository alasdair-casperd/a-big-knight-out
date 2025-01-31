
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// A class used to show a button to return to the level editor when previewing
    /// a level in the level player
    /// </summary>
    public class ReturnToLevelEditorButton : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(LevelEditor.LevelToPreview != null);
            // gameObject.SetActive(false);
        }

        public void TransitionToLevelEditor()
        {
            SceneManager.LoadScene("LevelEditor");
        }
    }
}