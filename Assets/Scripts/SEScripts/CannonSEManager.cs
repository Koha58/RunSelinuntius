using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��C�U���̌��ʉ� (SE) �ƃG�t�F�N�g���Ǘ�����N���X
/// </summary>
public class CannonSEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // ���ʉ����Đ����� AudioSource �R���|�[�l���g

    [SerializeField] private AudioClip cannonFireSound;    // ��C���ˎ��̌��ʉ��N���b�v
    [SerializeField] private AudioClip cannonImpactSound; // ��C���e���̌��ʉ��N���b�v

    [SerializeField] private GameObject impactEffectPrefab; // ���e�G�t�F�N�g�̃v���n�u

    [SerializeField] private LayerMask groundLayer; // �n�ʂ̃��C���[

    private const float FullSpatialBlend = 1.0f;// 3D�����ݒ�p�̒萔

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(cannonFireSound);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g�̃��C���[���n�ʂ̃��C���[�ƈ�v����ꍇ
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // ���e�n�_�ɉ��p�I�u�W�F�N�g�𐶐�
            GameObject soundObject = new GameObject("ImpactSound");
            soundObject.transform.position = collision.contacts[0].point; // ���e�n�_�ɔz�u
            AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
            tempAudioSource.spatialBlend = FullSpatialBlend; // 3D�����ɐݒ�
            tempAudioSource.clip = cannonImpactSound;
            tempAudioSource.Play();

            // ���e�n�_�ɃG�t�F�N�g�𐶐����A���p�I�u�W�F�N�g��e�ɂ���
            GameObject impactEffect = PlayCannonImpactEffect(collision.contacts[0].point);
            if (impactEffect != null)
            {
                // �G�t�F�N�g�����p�I�u�W�F�N�g�̎q�ɐݒ�
                impactEffect.transform.SetParent(soundObject.transform);
            }

            // �����I�������폜
            Destroy(soundObject, cannonImpactSound.length);

            // �e���̂��v�[���ɖ߂������������x�点��
            StartCoroutine(ReturnToPoolAfterDelay("Prefabs/Attack/Cannon/IronBall", this.gameObject, 1.5f));
        }
    }

    /// <summary>
    /// ��C���e���̃G�t�F�N�g���Đ����郁�\�b�h
    /// </summary>
    /// <param name="position">�G�t�F�N�g�̔����ʒu</param>
    /// <returns>�������ꂽ�G�t�F�N�g�� GameObject</returns>
    public GameObject PlayCannonImpactEffect(Vector3 position)
    {
        if (impactEffectPrefab != null)
        {
            // �G�t�F�N�g�𐶐�
            GameObject impactEffect = Instantiate(impactEffectPrefab, position, Quaternion.identity);
            return impactEffect;
        }
        else
        {
            Debug.LogWarning("impactEffectPrefab ���ݒ肳��Ă��܂���B");
            return null;
        }
    }

    /// <summary>
    /// �w��b����ɏ�Q�����v�[���ɕԋp����
    /// </summary>
    private IEnumerator ReturnToPoolAfterDelay(string prefabPath, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Null�`�F�b�N�ƕԋp
        if (ObstaclePoolManager.Instance != null && obj != null)
        {
            ObstaclePoolManager.Instance.ReturnObject(prefabPath, obj);
        }
    }

}
