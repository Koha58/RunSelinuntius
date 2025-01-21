using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大砲攻撃の効果音 (SE) を管理するクラス
/// </summary>
public class CannonSEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // 効果音を再生する AudioSource コンポーネント

    [SerializeField] private AudioClip cannonFireSound;    // 大砲発射時の効果音クリップ
    [SerializeField] private AudioClip cannonImpactSound; // 大砲着弾時の効果音クリップ

    [SerializeField] private LayerMask groundLayer; // 地面のレイヤー

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(cannonFireSound);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトのレイヤーが地面のレイヤーと一致する場合
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            PlayCannonImpactSound();
        }
    }

    /// <summary>
    /// 大砲発射時の効果音を再生するメソッド
    /// </summary>
    public void PlayCannonFireSound()
    {
        if (audioSource != null && cannonFireSound != null)
        {
            audioSource.PlayOneShot(cannonFireSound);
        }
        else
        {
            Debug.LogWarning("AudioSource または cannonFireSound が設定されていません。");
        }
    }

    /// <summary>
    /// 大砲着弾時の効果音を再生するメソッド
    /// </summary>
    public void PlayCannonImpactSound()
    {
        if (audioSource != null && cannonImpactSound != null)
        {
            audioSource.PlayOneShot(cannonImpactSound);
        }
        else
        {
            Debug.LogWarning("AudioSource または cannonImpactSound が設定されていません。");
        }
    }
}
