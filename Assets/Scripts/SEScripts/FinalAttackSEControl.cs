using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FinalAttack関連のSE管理クラス
/// </summary>
public class FinalAttackSEControl : MonoBehaviour
{
    [SerializeField] private AudioSource SESource; // FinalAttack用SE の AudioSource
    [SerializeField] private AudioClip finalAttackSE; // FinalAttack 時の SE
    [SerializeField] private AudioClip runAwaySE; // Targetに逃げられた 時の SE

    /// <summary>
    /// FinalAttack の SE を再生し、再生時間を返す
    /// </summary>
    public float PlayFinalAttackSE()
    {
        if (SESource != null && finalAttackSE != null)
        {
            SESource.PlayOneShot(finalAttackSE);
            return finalAttackSE.length; // SE の長さを返す
        }
        return 0f; // SE が設定されていない場合は 0 を返す
    }

    /// <summary>
    /// PlayrunAway の SE を再生
    /// </summary>
    public void PlayrunAwaySE()
    {
        if (SESource != null && runAwaySE != null)
        {
            SESource.PlayOneShot(runAwaySE);
        }
    }
}
