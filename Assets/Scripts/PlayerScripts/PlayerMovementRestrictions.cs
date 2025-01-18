using UnityEngine;

/// <summary>
/// �v���C���[�̈ړ��͈͂𐧌�����N���X
/// </summary>
public class PlayerMovementRestriction : MonoBehaviour
{
    [SerializeField] private float minX = -8f; // �ړ��\��X���̍ŏ��l
    [SerializeField] private float maxX = 8f;  // �ړ��\��X���̍ő�l

    private void Update()
    {
        // ���݂̈ʒu���擾
        Vector3 position = transform.position;

        // X���̈ړ��𐧌�
        position.x = Mathf.Clamp(position.x, minX, maxX);

        // �X�V�����ʒu�𔽉f
        transform.position = position;
    }
}
