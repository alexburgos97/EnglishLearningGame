using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using SpatialSys.UnitySDK;

public class NumberTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int numberValue;
    public string numberInEnglish;
    public AudioClip numberAudio;

    private AudioSource audioSource;

    private string[] numberWords = new string[]
    {
        "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE",
        "SIX", "SEVEN", "EIGHT", "NINE", "TEN",
        "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN",
        "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN", "TWENTY"
    };

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        if (numberValue >= 0 && numberValue <= 20)
            numberInEnglish = numberWords[numberValue];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SpatialBridge.coreGUIService.DisplayToastMessage(numberInEnglish);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // No necesita hacer nada
    }

    public void PlayAudio()
    {
        if (numberAudio != null)
        {
            audioSource.clip = numberAudio;
            audioSource.Play();
        }
    }
}
