using System;
using UnityEngine;

public class RewardWallet : MonoBehaviour
{
    public static RewardWallet Instance;
    public int currentGold;
     public event Action<int> OnGoldChanged; 
     
    public int CurrentGold
    {
        get => currentGold;
        set => currentGold = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGold();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LoadGold()
    {
        CurrentGold = PlayerPrefs.GetInt("Gold", 0);
        OnGoldChanged?.Invoke(CurrentGold);
    }
    public void AddGold(int gold)
    {
        currentGold += gold;
        SaveGold();
        OnGoldChanged?.Invoke(gold);
    }
    // Spend gold
    public bool SpendGold(int amount)
    {
        if (!CanAfford(amount)) return false;

        CurrentGold -= amount;
        SaveGold();
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }

    // Check if enough gold
    public bool CanAfford(int amount)
    {
        return CurrentGold >= amount;
    }
    // Save system
    void SaveGold()
    {
        PlayerPrefs.SetInt("Gold", CurrentGold);
        PlayerPrefs.Save();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
