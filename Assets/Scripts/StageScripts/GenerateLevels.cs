using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����G���A��������������N���X
/// </summary>
public class GenerateLevels : MonoBehaviour
{
    // ���x���I�u�W�F�N�g���i�[����z��
    [SerializeField] private GameObject[] level;
    // ��������郌�x���̐V����z���W
    [SerializeField] private int zPos = 98;
    // ���x���𐶐������ǂ�����\���t���O
    [SerializeField] private bool creatingLevel = false;
    // �����_���ȃ��x����I�����邽�߂̐���
    [SerializeField] private int lvlNum;

    void Start()
    {
        // ��������������������ꏊ
    }

    void Update()
    {
        // ���x�����������łȂ��ꍇ�A���x���������n�߂�
        if (!creatingLevel)
        {
            creatingLevel = true; // �������t���O���I���ς݂ɐݒ�
            StartCoroutine(GenerateLvl()); // �񓯊����\�b�h�����s
        }
    }

    /// <summary>
    /// �V�������x���𐶐�����R���`�����\�b�h
    /// </summary>
    IEnumerator GenerateLvl()
    {
        // ���x���I�u�W�F�N�g�̔z�񂩂烉���_���ȍ��ڂ�I��
        lvlNum = Random.Range(0, 4); // ���x���I�u�W�F�N�g�̃C���f�b�N�X(0����4�̊�)

        // �V�������x���𐶐�
        Instantiate(level[lvlNum], new Vector3(0, 0, zPos), Quaternion.identity);

        // ���x���𐶐�����ʒu���X�V
        zPos += 98;

        // ���̐����܂ł̈ꎞ��~
        yield return new WaitForSeconds(20);

        // ���x���������I�������̂Ńt���O�����Z�b�g
        creatingLevel = false;
    }
}
