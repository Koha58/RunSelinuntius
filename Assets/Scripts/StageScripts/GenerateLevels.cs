using System.Collections;
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
    [SerializeField] private float baseGenerationDelay = 30f; // ��{�̐����Ԋu�i���x�ɉ����ĕω��j

    [Header("���x�ݒ�")]
    [SerializeField] private PlayerMove playerMove;           // �v���C���[�̈ړ��Ǘ��N���X

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
        // �v���C���[�̑��x�ɉ����Đ����Ԋu��ύX
        float currentSpeed = playerMove.GetCurrentSpeed(); // �v���C���[�̑��x���擾
        float adjustedGenerationDelay = baseGenerationDelay / currentSpeed; // ���x�ɉ����Đ����Ԋu�𒲐�

        // ���x���I�u�W�F�N�g�̔z�񂩂烉���_���ȍ��ڂ�I��
        int lvlNum = Random.Range(0, level.Length);

        // �V�������x���𐶐�
        Instantiate(level[lvlNum], new Vector3(0, 0, zPos), Quaternion.identity);

        // ���x���𐶐�����ʒu���X�V
        zPos += levelSpacing;

        // ���̐����܂ł̈ꎞ��~�i���x�ɉ����ĊԊu���Z�k�j
        yield return new WaitForSeconds(adjustedGenerationDelay);

        // ���x���������I�������̂Ńt���O�����Z�b�g
        creatingLevel = false;
    }
}
