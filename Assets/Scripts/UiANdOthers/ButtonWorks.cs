using UnityEngine;
using UnityEngine.UI;

public class ButtonWorks : MonoBehaviour
{
    [SerializeField] private Button resourceButton;
    [SerializeField] private GameObject resourcePanel;
    [SerializeField] private GameObject playerSelectionPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
