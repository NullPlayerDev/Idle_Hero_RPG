using System;
using UnityEngine;

/// <summary>
/// Stores and exposes the player's gold and gems at runtime.
/// Data is written to disk through SaveSystem — NOT PlayerPrefs.
/// </summary>
public class RewardWallet : MonoBehaviour
{
    public static RewardWallet Instance { get; private set; }

    private int _gold;
    private int _gems;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemsChanged;

    // ── Properties ────────────────────────────────────────────────────────────
    public int CurrentGold
    {
        get => _gold;
        set { _gold = Mathf.Max(0, value); OnGoldChanged?.Invoke(_gold); }
    }

    public int CurrentGems
    {
        get => _gems;
        set { _gems = Mathf.Max(0, value); OnGemsChanged?.Invoke(_gems); }
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Values are set by SaveSystem.Load() via SetGold / SetGems
    }

    // ── Gold ──────────────────────────────────────────────────────────────────

    /// <summary>Hard-sets gold (called by SaveSystem on load / config override).</summary>
    public void SetGold(int amount)
    {
        _gold = Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(_gold);
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        OnGoldChanged?.Invoke(_gold);
        SaveSystem.Save();
    }

    public bool SpendGold(int amount)
    {
        if (_gold < amount) return false;
        _gold -= amount;
        OnGoldChanged?.Invoke(_gold);
        SaveSystem.Save();
        return true;
    }

    public bool CanAfford(int amount) => _gold >= amount;

    // ── Gems ──────────────────────────────────────────────────────────────────

    /// <summary>Hard-sets gems (called by SaveSystem on load / config override).</summary>
    public void SetGems(int amount)
    {
        _gems = Mathf.Max(0, amount);
        OnGemsChanged?.Invoke(_gems);
    }

    public void AddGems(int amount)
    {
        _gems += amount;
        OnGemsChanged?.Invoke(_gems);
        SaveSystem.Save();
    }

    public bool SpendGems(int amount)
    {
        if (_gems < amount) return false;
        _gems -= amount;
        OnGemsChanged?.Invoke(_gems);
        SaveSystem.Save();
        return true;
    }

    public bool CanAffordGems(int amount) => _gems >= amount;
}