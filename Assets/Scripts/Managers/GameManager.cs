using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private RewardCalculator rewardCalculator;
    [SerializeField] private GameObject rewardSystem;
    [SerializeField] private GameObject combatSystem;
    private CombatSystem _combatSystem;
    private int totalStages = 0;
    private bool isLoopEnded = false;
    public event Action OnStageEnded;
    public event Action OnLoopEnded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _combatSystem = combatSystem.GetComponent<CombatSystem>();
        rewardCalculator = rewardSystem.GetComponent<RewardCalculator>();
        _combatSystem.OnStageEnded += HandleStageEnded;
    }

    private void Update()
    {
       
    }

    public void DeleteGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("all files are deleted");
    }
    private void OnDestroy()
    {
        if (_combatSystem != null)
            _combatSystem.OnStageEnded -= HandleStageEnded;
    }

    private void HandleStageEnded()
    {
        totalStages++;

        // CalculateGoldsReward handles gold
        rewardCalculator.CalculateGoldsReward();

        // RewardInWallet calls CalculateGemsReward internally and deposits result.
        // Do NOT call CalculateGemsReward separately — it resets the flags,
        // so calling it twice means the second call always sees isTheStageFinished=false and returns 0.
        rewardCalculator.RewardInWallet();

        OnStageEnded?.Invoke();

        if (totalStages >= 10)
        {
            isLoopEnded = true;
            totalStages = 0;
            OnLoopEnded?.Invoke();
        }
    }
    
}