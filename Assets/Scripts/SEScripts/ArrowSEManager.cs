using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弓攻撃の効果音 (SE) を管理するクラス
/// </summary>
public class ArrowSEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // 弓攻撃時に再生する効果音

    /// <summary>
    /// アニメーションイベントから呼び出されるメソッド
    /// 弓攻撃の効果音を再生する
    /// </summary>
    public void ArrowSE()
    {
        // AudioSource が設定されている場合にのみ効果音を再生
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            // AudioSource が設定されていない場合のデバッグメッセージ
            Debug.LogWarning("AudioSource が設定されていません。効果音を再生できません。");
        }
    }
}
