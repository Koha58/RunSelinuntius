using UnityEngine;

/// <summary>
/// FinalAttack の可能範囲内にいるときだけ BGM を変更するクラス
/// </summary>
public class FinalAttackBGMChanger : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource; // BGM の AudioSource
    [SerializeField] private AudioClip finalAttackBGM; // FinalAttack 時の BGM
    [SerializeField] private AudioClip normalBGM; // 通常時の BGM
    [SerializeField] private PlayerMove playerMove; // PlayerMove の参照

    private bool isInFinalAttackRange = false; // FinalAttack 範囲内かどうか

    private void Awake()
    {
        // 同じオブジェクトにアタッチされた PlayerMove を取得
        playerMove = GetComponent<PlayerMove>();
        isInFinalAttackRange = false;
    }

    private void Update()
    {
        if (playerMove == null || bgmSource == null) return;

        // プレイヤーが FinalAttack 可能かを取得
        bool isNearTarget = playerMove.IsFinalAttackPossible();

        // 状態が変わった場合のみ BGM を切り替える
        if (isNearTarget && !isInFinalAttackRange)
        {
            ChangeBGM(finalAttackBGM);
            isInFinalAttackRange = true;
        }
        else if (!isNearTarget && isInFinalAttackRange)
        {
            ChangeBGM(normalBGM);
            isInFinalAttackRange = false;
        }
    }

    /// <summary>
    /// BGM を変更する
    /// </summary>
    private void ChangeBGM(AudioClip newClip)
    {
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();
    }
}
