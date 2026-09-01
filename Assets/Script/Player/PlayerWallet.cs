using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int startMoney = 100;
    public int Money { get; private set; }

    private void Awake()
    {
        Money = startMoney;
    }

    public bool Spend(int amount)
    {
        if (amount <= 0) return true;
        if (Money < amount) return false;

        Money -= amount;
        Debug.Log($"[Wallet] -{amount}, 현재 금액: {Money}");
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        Debug.Log($"[Wallet] +{amount}, 현재 금액: {Money}");
    }
}
