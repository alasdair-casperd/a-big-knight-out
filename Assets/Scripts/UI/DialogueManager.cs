using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// A class used to create and display dialogue pop up boxes
    /// </summary>
    public class DialogueManager: MonoBehaviour
    {
        /// <summary>
        /// The dialogue prefab to instantiate
        /// </summary>
        public GameObject dialoguePrefab;

        /// <summary>
        /// The parent to which to add the dialogues
        /// </summary>
        public Transform dialogueParent;
        
        /// <summary>
        /// Create and display dialogue with a title, body text, and up to two buttons
        /// </summary>
        /// <param name="headerData"></param>
        /// <param name="parent"></param>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        public void Create(Dialogue.HeaderData headerData, Dialogue.ButtonData? primary = null, Dialogue.ButtonData? secondary = null)
        {
            Dialogue dialogue = Instantiate(dialoguePrefab, dialogueParent).GetComponent<Dialogue>();

            Slider slider = dialogue.GetComponent<Slider>();

            dialogue.Title.text = headerData.title;
            dialogue.Body.text = headerData.body;

            dialogue.PrimaryButton.gameObject.SetActive(primary != null);

            if (primary is Dialogue.ButtonData primaryData)
            {
                dialogue.PrimaryButtonText.text = primaryData.text;
                dialogue.PrimaryButton.onClick.AddListener(primaryData.action);
            }

            if (secondary is Dialogue.ButtonData secondaryData)
            {
                dialogue.SecondaryButtonText.text = secondaryData.text;
                dialogue.SecondaryButton.onClick.AddListener(secondaryData.action);
            }
            else
            {
                dialogue.SecondaryButtonText.text = "Cancel";
            }

            slider.Show(forceEntrance: true);
        }
    }
}