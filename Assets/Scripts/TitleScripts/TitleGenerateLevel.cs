using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �^�C�g���V�[���̃����G���A��������������N���X
/// </summary>
public class TitleGenerateLevel : MonoBehaviour
{
    [Header("���x���ݒ�")]
    [SerializeField] private GameObject[] level;              // ���x���I�u�W�F�N�g���i�[����z��
    [SerializeField] private float initialZPos = 700f;           // ������z���W
    [SerializeField] private float initialXPos = 2.0f;            // ������x���W
    [SerializeField] private float levelSpacing = 98.0f;           // ���x���Ԃ̋���
    [SerializeField] private float GenerationDelay = 30.0f; // �����Ԋu

    private float xPos;                                         // ���݂�x���W
    private float zPos;                                         // ���݂�z���W
    private bool creatingLevel = false;                       // ���x���𐶐������ǂ����̃t���O

    void Start()
    {
        // ������x���W��ݒ�
        xPos = initialXPos;
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
        Instantiate(level[lvlNum], new Vector3(xPos, 0, zPos), Quaternion.identity);

        // ���x���𐶐�����z�ʒu���X�V
        zPos += levelSpacing;

        // ���̐����܂ł̈ꎞ��~
        yield return new WaitForSeconds(GenerationDelay);

        // ���x���������I�������̂Ńt���O�����Z�b�g
        creatingLevel = false;
    }
}
