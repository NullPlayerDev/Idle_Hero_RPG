using System;
using UnityEngine;

public class RewardWallet : MonoBehaviour
{
    public static RewardWallet Instance;
    public int currentGold;
    public int currentGems;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemsChanged;

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

    // -------------------------
    // Gold
    // -------------------------
    void LoadGold()
    {
        CurrentGold = PlayerPrefs.GetInt("Gold", 0);
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public void AddGold(int gold)
    {
        currentGold += gold;
        SaveGold();
        OnGoldChanged?.Invoke(currentGold); // fire with TOTAL, not just the added amount
    }

    public bool SpendGold(int amount)
    {
        if (!CanAfford(amount)) return false;
        CurrentGold -= amount;
        SaveGold();
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }

    public bool CanAfford(int amount)
    {
        return CurrentGold >= amount;
    }

    void SaveGold()
    {
        PlayerPrefs.SetInt("Gold", CurrentGold);
        PlayerPrefs.Save();
    }

    // -------------------------
    // Gems
    // -------------------------
    void LoadGems()
    {
        currentGems = PlayerPrefs.GetInt("Gems", 0);
        OnGemsChanged?.Invoke(currentGems); // BUG FIX 1: was firing OnGoldChanged
    }

    public void AddGems(int gems)
    {
        currentGems += gems;
        SaveGems();                          // BUG FIX 2: was calling SaveGold() so gems were never saved
        OnGemsChanged?.Invoke(currentGems); // fire with TOTAL, not just the added amount
    }

    public bool SpendGems(int amount)
    {
        if (!CanAffordGems(amount)) return false;
        currentGems -= amount;
        SaveGems();
        OnGemsChanged?.Invoke(currentGems); // BUG FIX 3: was firing OnGoldChanged
        return true;
    }

    public bool CanAffordGems(int amount)
    {
        return currentGems >= amount;
    }

    void SaveGems()
    {
        PlayerPrefs.SetInt("Gems", currentGems);
        PlayerPrefs.Save();
    }

    void Start() { }
    void Update() { }
}