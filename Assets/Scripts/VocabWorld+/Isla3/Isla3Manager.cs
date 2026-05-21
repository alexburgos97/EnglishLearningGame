using UnityEngine;
using SpatialSys.UnitySDK;

public class Isla3Manager : MonoBehaviour
{
    public static Isla3Manager Instance { get; private set; }

    [Header("Canvas")]
    public Transform isla3Canvas;

    [Header("Paneles")]
    public GameObject familyPanel;
    public GameObject animalPanel;
    public GameObject placePanel;

    [Header("Progreso")]
    public int totalInteractions = 9;

    private int totalCompleted = 0;
    private int totalFamily = 0;
    private int totalAnimals = 0;
    private int totalPlaces = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterFamily() { totalFamily++; }
    public void RegisterAnimal() { totalAnimals++; }
    public void RegisterPlace() { totalPlaces++; }

    public void MoveCanvasToPlayer()
    {
    if (isla3Canvas == null) return;
    Vector3 avatarPos = SpatialBridge.actorService.localActor.avatar.position;
    
    // Posicionar frente al jugador
    Vector3 dirección = avatarPos - isla3Canvas.position;
    dirección.y = 0; // Evitar inclinación vertical
    
    isla3Canvas.position = avatarPos + Vector3.up * 2f + dirección.normalized * 1.5f;
    isla3Canvas.rotation = Quaternion.LookRotation(dirección.normalized);
    }

    public void ShowFamilyPanel()
    {
        MoveCanvasToPlayer();
        familyPanel.SetActive(true);
        animalPanel.SetActive(false);
        placePanel.SetActive(false);
    }

    public void ShowAnimalPanel()
    {
        MoveCanvasToPlayer();
        animalPanel.SetActive(true);
        familyPanel.SetActive(false);
        placePanel.SetActive(false);
    }

    public void ShowPlacePanel()
    {
        MoveCanvasToPlayer();
        placePanel.SetActive(true);
        familyPanel.SetActive(false);
        animalPanel.SetActive(false);
    }

    public void HideAllPanels()
    {
        familyPanel.SetActive(false);
        animalPanel.SetActive(false);
        placePanel.SetActive(false);
    }

    public void OnFamilyComplete()
    {
        totalCompleted++;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Family word learned! " + totalCompleted + "/" + totalInteractions);
        CheckAllComplete();
    }

    public void OnAnimalComplete()
    {
        totalCompleted++;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Animal discovered! " + totalCompleted + "/" + totalInteractions);
        CheckAllComplete();
    }

    public void OnPlaceComplete()
    {
        totalCompleted++;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Place identified! " + totalCompleted + "/" + totalInteractions);
        CheckAllComplete();
    }

    private void CheckAllComplete()
    {
    if (totalCompleted >= totalInteractions)
        {
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Amazing! You explored the whole city!");

        // Desactivar bloqueador de la insignia
        GameObject bloqueador = GameObject.Find("Bloqueador_Insignia_Final");
        if (bloqueador != null)
            bloqueador.SetActive(false);

        GameProgressManager.Instance.AwardGlobalCitizenMedal();
        }
    }
}