using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Q���̃I�u�W�F�N�g�v�[�����Ǘ�����N���X�iSingleton�j
/// �����R�X�g�팸�E�ė��p�ɂ���ăp�t�H�[�}���X�����コ����
/// </summary>
public class ObstaclePoolManager : MonoBehaviour
{
    /// <summary>
    /// Singleton�C���X�^���X�i���N���X����A�N�Z�X�\�j
    /// </summary>
    public static ObstaclePoolManager Instance;

    /// <summary>
    /// �v�[���Ǘ��p�f�B�N�V���i���i�L�[:Prefab�p�X�A�l:�I�u�W�F�N�g�̃L���[�j
    /// </summary>
    private Dictionary<string, Queue<GameObject>> pool = new();

    /// <summary>
    /// �C���X�^���X�̏������iSingleton�̃Z�b�g�j
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// �w���Prefab�p�X�ɑΉ�����I�u�W�F�N�g�v�[��������������
    /// </summary>
    /// <param name="prefabPath">Resources.Load�ȂǂɎg��Prefab�̃p�X</param>
    /// <param name="prefab">��������Prefab</param>
    /// <param name="initialSize">�����������Ă�����</param>
    public void InitializePool(string prefabPath, GameObject prefab, int initialSize)
    {
        // �܂��v�[�������݂��Ȃ��ꍇ�̂ݏ�����
        if (!pool.ContainsKey(prefabPath))
        {
            pool[prefabPath] = new Queue<GameObject>();

            // �w�萔�����I�u�W�F�N�g�𐶐��E��A�N�e�B�u�����ăv�[���Ɋi�[
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool[prefabPath].Enqueue(obj);
            }
        }
    }

    /// <summary>
    /// �v�[������I�u�W�F�N�g��1�擾����i����Ȃ��ꍇ�͐V�K�����j
    /// </summary>
    /// <param name="prefabPath">�I�u�W�F�N�g�̎�ނ������L�[</param>
    /// <param name="prefab">�K�v�ɉ����Ďg��Prefab</param>
    /// <returns>�g�p�\��GameObject</returns>
    public GameObject GetObject(string prefabPath, GameObject prefab)
    {
        // �v�[�����Ȃ��A�܂��͋�Ȃ�V�K����
        if (!pool.ContainsKey(prefabPath) || pool[prefabPath].Count == 0)
        {
            GameObject newObj = Instantiate(prefab);
            return newObj;
        }

        // �v�[������擾���A�A�N�e�B�u�����ĕԂ�
        GameObject obj = pool[prefabPath].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// �g�p�ς݃I�u�W�F�N�g���v�[���ɕԋp����i��A�N�e�B�u���j
    /// </summary>
    /// <param name="prefabPath">�I�u�W�F�N�g�̎�ނ������L�[</param>
    /// <param name="obj">�ԋp����GameObject</param>
    public void ReturnObject(string prefabPath, GameObject obj)
    {
        // �I�u�W�F�N�g�����łɔj������Ă��Ȃ����m�F
        if (obj == null)
        {
            Debug.LogWarning($"ReturnObject called but obj is already destroyed or null. prefabPath={prefabPath}");
            return;
        }

        // �f�o�b�O���O�F�ԋp�������m�F�p�ɏo��
        Debug.Log($"ReturnObject: returning {obj.name} for prefabPath={prefabPath}");

        // ��A�N�e�B�u�����ăv�[���ɖ߂�
        obj.SetActive(false);
        pool[prefabPath].Enqueue(obj);
    }
}
