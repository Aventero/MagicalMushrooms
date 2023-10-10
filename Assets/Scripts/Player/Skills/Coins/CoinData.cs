using UnityEngine;

[CreateAssetMenu(menuName = "Coin/Coin Data")]
public class CoinData : ScriptableObject
{
    public int Value = 1;
    public float Scaling = 1f;
    public Color Color = Color.green;
    public float Resistance = 0.5f;
}
