using UnityEngine;

/// <summary>
/// プレイヤーの移動範囲を制限するクラス
/// </summary>
public class PlayerMovementRestriction : MonoBehaviour
{
    [SerializeField] private float minX = -8f; // 移動可能なX軸の最小値
    [SerializeField] private float maxX = 8f;  // 移動可能なX軸の最大値

    private void Update()
    {
        // 現在の位置を取得
        Vector3 position = transform.position;

        // X軸の移動を制限
        position.x = Mathf.Clamp(position.x, minX, maxX);

        // 更新した位置を反映
        transform.position = position;
    }
}
