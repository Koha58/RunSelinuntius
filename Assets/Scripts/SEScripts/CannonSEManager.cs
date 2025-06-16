using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大砲攻撃の効果音 (SE) とエフェクトを管理するクラス
/// </summary>
public class CannonSEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // 効果音を再生する AudioSource コンポーネント

    [SerializeField] private AudioClip cannonFireSound;    // 大砲発射時の効果音クリップ
    [SerializeField] private AudioClip cannonImpactSound; // 大砲着弾時の効果音クリップ

    [SerializeField] private GameObject impactEffectPrefab; // 着弾エフェクトのプレハブ

    [SerializeField] private LayerMask groundLayer; // 地面のレイヤー

    private const float FullSpatialBlend = 1.0f;// 3D音響設定用の定数

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
            // 着弾地点に音用オブジェクトを生成
            GameObject soundObject = new GameObject("ImpactSound");
            soundObject.transform.position = collision.contacts[0].point; // 着弾地点に配置
            AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
            tempAudioSource.spatialBlend = FullSpatialBlend; // 3D音響に設定
            tempAudioSource.clip = cannonImpactSound;
            tempAudioSource.Play();

            // 着弾地点にエフェクトを生成し、音用オブジェクトを親にする
            GameObject impactEffect = PlayCannonImpactEffect(collision.contacts[0].point);
            if (impactEffect != null)
            {
                // エフェクトを音用オブジェクトの子に設定
                impactEffect.transform.SetParent(soundObject.transform);
            }

            // 音が終わったら削除
            Destroy(soundObject, cannonImpactSound.length);

            // 弾自体をプールに戻す処理を少し遅らせる
            StartCoroutine(ReturnToPoolAfterDelay("Prefabs/Attack/Cannon/IronBall", this.gameObject, 1.5f));
        }
    }

    /// <summary>
    /// 大砲着弾時のエフェクトを再生するメソッド
    /// </summary>
    /// <param name="position">エフェクトの発生位置</param>
    /// <returns>生成されたエフェクトの GameObject</returns>
    public GameObject PlayCannonImpactEffect(Vector3 position)
    {
        if (impactEffectPrefab != null)
        {
            // エフェクトを生成
            GameObject impactEffect = Instantiate(impactEffectPrefab, position, Quaternion.identity);
            return impactEffect;
        }
        else
        {
            Debug.LogWarning("impactEffectPrefab が設定されていません。");
            return null;
        }
    }

    /// <summary>
    /// 指定秒数後に障害物をプールに返却する
    /// </summary>
    private IEnumerator ReturnToPoolAfterDelay(string prefabPath, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Nullチェックと返却
        if (ObstaclePoolManager.Instance != null && obj != null)
        {
            ObstaclePoolManager.Instance.ReturnObject(prefabPath, obj);
        }
    }

}
