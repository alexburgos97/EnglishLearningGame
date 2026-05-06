using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SparkyDialog : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;

    private int currentStep = 0;

    private string[] dialogSteps = new string[]
    {
        "Hello! I am Sparky, your guide. Welcome to the Central Classroom of Sky Academy! This place is magical because of 'Sintaxis Lumina' – our special energy of communication and knowledge.\n",
        "But the colors are disappearing. A dark force called 'Linguistic Chaos' is attacking the academy. It feeds on confusion. The Runa of Structure (in GrammarWorld) and the Runa of Meaning (in VocabWorld+) are broken! We are in danger.",
        "Wait! My sensors detect something special in you. You are a 'Word Mender'! You have the power to fix words and restore the energy.\nYou are the hero we need!",
        "Take this Badge Passport. It is very important!",
        "Here is your mission:\n\n* First, go to GrammarWorld. Win 3 badges and 1 final insignia.\n* Then, go to VocabWorld+. Win 3 badges and 1 final insignia.\n\nYou must complete the passport to save the Academy!",
        "Are you ready Word Mender? The portal to GrammarWorld is open.\n Good luck!"
    };

    private void Start()
    {
        dialogPanel.SetActive(false);
    }

    public void OpenDialog()
    {
        currentStep = 0;
        dialogPanel.SetActive(true);
        UpdateDialog();
    }

    public void NextStep()
    {
        currentStep++;
        if (currentStep >= dialogSteps.Length)
        {
            dialogPanel.SetActive(false);
            return;
        }
        UpdateDialog();
    }

    private void UpdateDialog()
    {
        dialogText.text = dialogSteps[currentStep];
        if (currentStep == dialogSteps.Length - 1)
            nextButtonText.text = "Close";
        else
            nextButtonText.text = "Next";
    }
}