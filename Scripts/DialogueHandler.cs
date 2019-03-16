using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSONAsset = null;
    private Story story = null;

    [SerializeField] private GameObject dialoguePanel = null;
    private Text textField = null;
    private Transform dialogueChoicePanel = null;

    [SerializeField] private Button buttonPrefab = null;

    private void Awake()
    {
        dialoguePanel = Instantiate(dialoguePanel, GameObject.Find("Canvas").transform);
        textField = dialoguePanel.GetComponentInChildren<Text>();
        dialogueChoicePanel = dialoguePanel.transform.Find("DialogueChoicePanel");
        Image image = dialoguePanel.transform.Find("Image").GetComponent<Image>();
        image.sprite = GetComponent<SpriteRenderer>().sprite;
        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Starts the story.
    /// </summary>
    public void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        RefreshView();
    }

    /// <summary>
    /// Continues the story.
    /// </summary>
    private void RefreshView()
    {
        // Clear UI
        RemoveChildren();

        // Show story text
        string text = story.ContinueMaximally().Trim();
        CreateContentView(text);

        if (story.currentChoices.Count > 0)
        {
            foreach (Choice choice in story.currentChoices)
            {
                Button button = CreateChoiceButton(choice.text);
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        }
        else
        {
            dialoguePanel.SetActive(false);
            /*
            Button button = CreateChoiceButton("Start over ? xd");
            button.onClick.AddListener(delegate
            {
                StartStory();
            });
            */
        }
    }

    /// <summary>
    /// Instantiates text prefab.
    /// </summary>
    /// <param name="text">The text to be displayed.</param>
    private void CreateContentView(string text)
    {
        dialoguePanel.SetActive(true);
        textField.text = text;
    }

    /// <summary>
    /// Create a choice button.
    /// </summary>
    /// <param name="text">The text on the button.</param>
    /// <returns>The button that was instantiated.</returns>
    private Button CreateChoiceButton(string text)
    {
        Button button = Instantiate(buttonPrefab, dialogueChoicePanel.transform);
        button.GetComponentInChildren<Text>().text = text;
        button.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
        return button;
    }

    /// <summary>
    /// Selects choice.
    /// </summary>
    /// <param name="choice">The player's choice.</param>
    private void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    /// <summary>
    /// Removes story-related UI.
    /// </summary>
    private void RemoveChildren()
    {
        int count = dialogueChoicePanel.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(dialogueChoicePanel.transform.GetChild(i).gameObject);
        }

        textField.text = "";
    }

    /// <summary>
    /// Hides the dialogue panel.
    /// </summary>
    public void HidePanel()
    {
        dialoguePanel.SetActive(false);
    }
}
