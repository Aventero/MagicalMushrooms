using UnityEngine;

public class CoinVacuum : PlayerSkill
{
    private Coin[] coins;

    private void Start()
    {
        coins = FindObjectsOfType<Coin>();
    }

    public override void ShowPreview() 
    {
        IsActivated = true;
    }
    
    public override void HidePreview() 
    {
        IsActivated = false;
    }

    public override bool Execute() 
    {
        foreach (Coin coin in coins)
        {
            coin.ActivateVacuum();
        }

        return true; 
    }
}
