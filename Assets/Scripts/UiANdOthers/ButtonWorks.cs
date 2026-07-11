using UnityEngine;
using UnityEngine.UI;

public class ButtonWorks : MonoBehaviour
{
    [SerializeField] private Button resourceButton;
    [SerializeField] private GameObject resourcePanel;
    [SerializeField] private GameObject playerSelectionPanel;
    [SerializeField] private GameObject[] offPanelList;
    [SerializeField] private GameObject[] openPanelList;
    
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResourcePanelActive()
    {
        resourcePanel.SetActive(false);
        playerSelectionPanel.SetActive(true);
    }

    public void MenuPanelActive()
    {
        gameManager.MainMenuPanel.SetActive(false);
        gameManager.GameMapPanel.SetActive(true);
        gameManager.PlayerSelectionCanvas.SetActive(true);
        
    }

    public void Exit()
    {
        Debug.Log("Application exited");
        Application.Quit();
    }

    public void PanelOpen()
    {
        Debug.Log("PanelOpen Called");
        foreach (var VARIABLE in offPanelList)
        {
            VARIABLE.SetActive(false);
        }

        foreach (var VARIABLE in openPanelList)
        {
            VARIABLE.SetActive(true);
        }
    }
}
