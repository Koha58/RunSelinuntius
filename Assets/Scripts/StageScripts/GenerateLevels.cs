using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����G���A��������������N���X
/// </summary>
public class GenerateLevels : MonoBehaviour
{
    [Header("���x���ݒ�")]
    [SerializeField] private GameObject[] level;              // ���x���I�u�W�F�N�g���i�[����z��
    [SerializeField] private int initialZPos = 392;           // ������z���W
    [SerializeField] private int levelSpacing = 98;           // ���x���Ԃ̋���
    [SerializeField] private float generationDelay = 20f;     // ���x�������̊Ԋu

    private int zPos;                                         // ���݂�z���W
    private bool creatingLevel = false;                       // ���x���𐶐������ǂ����̃t���O

    void Start()
    {
        // ������z���W��ݒ�
        zPos = initialZPos;
    }

    void Update()
    {
        // ���x�����������łȂ��ꍇ�A���x���������n�߂�
        if (!creatingLevel)
        {
            creatingLevel = true; // �������t���O��ݒ�
            StartCoroutine(GenerateLvl()); // �񓯊����\�b�h�����s
        }
    }

    /// <summary>
    /// �V�������x���𐶐�����R���[�`��
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // ���x���I�u�W�F�N�g�̔z�񂩂烉���_���ȍ��ڂ�I��
        int lvlNum = Random.Range(0, level.Length);

        // �V�������x���𐶐�
        Instantiate(level[lvlNum], new Vector3(0, 0, zPos), Quaternion.identity);

        // ���x���𐶐�����ʒu���X�V
        zPos += levelSpacing;

        // ���̐����܂ł̈ꎞ��~
        yield return new WaitForSeconds(generationDelay);

        // ���x���������I�������̂Ńt���O�����Z�b�g
        creatingLevel = false;
    }
}
