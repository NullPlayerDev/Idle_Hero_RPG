using System;
using UnityEngine;

public class RewardWallet : MonoBehaviour
{
    public static RewardWallet Instance;
    public int currentGold;
    public int currentGems;
     public event Action<int> OnGoldChanged; 
     
    public int CurrentGold
    {
        get => currentGold;
        set => currentGold = value;
    }

    public int CurrentGems
    {
        get => currentGems;
        set => currentGems = value;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGold();
            LoadGems();
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
/*
 * -----Gems Part Codes are here-------
 * 
 */  
  
    void LoadGems()
    {
        CurrentGems = PlayerPrefs.GetInt("Gems", 0);
        OnGoldChanged?.Invoke(currentGems);
    }
    public void AddGems(int gems)
    {
        currentGems += gems;
        SaveGold();
        OnGoldChanged?.Invoke(gems);
    }
    // Spend gold
    public bool SpendGems(int amount)
    {
        if (!CanAffordGems(amount)) return false;

        currentGems -= amount;
        SaveGems();
        OnGoldChanged?.Invoke(currentGems);
        return true;
    }

    // Check if enough gold
    public bool CanAffordGems(int amount)
    {
        return currentGems >= amount;
    }
    
    void SaveGems()
    {
        PlayerPrefs.SetInt("Gems", currentGems);
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
