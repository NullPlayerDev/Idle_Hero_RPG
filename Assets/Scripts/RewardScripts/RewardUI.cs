using System;
using TMPro;
using UnityEngine;

public class RewardUI  : MonoBehaviour
{
   [SerializeField]private TextMeshProUGUI goldText;
   [SerializeField]private TextMeshProUGUI gemText;
   private void Start()
   {
     
   }

   private void Update()
   {
      goldText.text = $"Total gold: {RewardWallet.Instance.CurrentGold.ToString()}";
   }
}
