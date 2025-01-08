using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �v���C���[�̓����𐧌䂷��N���X
/// </summary>
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Animator _animator;        // �v���C���[�̃A�j���[�^�[
    [SerializeField] private float _forwardSpeed = 3f;  // �O�����ړ����x
    [SerializeField] private float _moveSpeed = 5f;     // ���ړ����x
    [SerializeField] private float _jumpForce = 5f;     // �W�����v��
    [SerializeField] private LayerMask _groundLayer;    // �n�ʂ̃��C���[

    private float _horizontalVelocity;                 // �������̈ړ����x
    private Rigidbody _rigidbody;                      // �v���C���[��Rigidbody
    private bool _isGrounded;                          // �n�ʂɂ��邩�ǂ���

    /// <summary>
    /// �R���|�[�l���g�̏�����
    /// </summary>
    private void Awake()
    {
        // Rigidbody�R���|�[�l���g���擾
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody ��������܂���I �v���C���[�� Rigidbody �R���|�[�l���g���A�^�b�`���Ă��������B");
        }
    }

    /// <summary>
    /// �ړ����͂��󂯎��
    /// </summary>
    /// <param name="value">���͒l (Vector2)</param>
    private void OnMove(InputValue value)
    {
        // ���X�e�B�b�N�̓��͒l���擾
        Vector2 axis = value.Get<Vector2>();

        // �������̈ړ����x��ێ�
        _horizontalVelocity = axis.x;
    }

    /// <summary>
    /// �W�����v���͂��󂯎��
    /// </summary>
    /// <param name="value">���͒l (isPressed)</param>
    private void OnJump(InputValue value)
    {
        // �W�����v�{�^����������Ă��āA���n�ʂɂ���ꍇ�̂݃W�����v�����s
        if (_isGrounded && value.isPressed)
        {
            // �A�j���[�V�������W�����v��Ԃɂ���
            if (_animator != null)
            {
                _animator.SetBool("Jump", true);
            }

            // Rigidbody �ɃW�����v�̗͂�������
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            // �W�����v���͒n�ʂɂ��Ȃ���Ԃɐݒ�
            _isGrounded = false;

            // ��莞�Ԍ�ɃW�����v�A�j���[�V�������I������
            StartCoroutine(SetJumpFalseAfterDelay(0.8f));
        }
    }

    /// <summary>
    /// ���t���[���Ăяo����鏈��
    /// �������ƑO�����Ɉړ�������
    /// </summary>
    private void Update()
    {
        // �������ƑO�����Ɉړ�
        Vector3 movement = new Vector3(_horizontalVelocity * _moveSpeed, 0, _forwardSpeed) * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// ���Ԋu�ŌĂяo����镨���v�Z
    /// �n�ʂɂ��邩�ǂ����𔻒�
    /// </summary>
    private void FixedUpdate()
    {
        // �n�ʂɂ��邩�ǂ������m�F
        _isGrounded = Physics.CheckSphere(transform.position, 0.1f, _groundLayer);
    }

    /// <summary>
    /// �n�ʃ`�F�b�N�p�̉�������
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    /// <summary>
    /// ��莞�Ԍ�ɃW�����v�A�j���[�V�������I�����A�n�ʂɂ����Ԃɖ߂�
    /// </summary>
    /// <param name="delay">�x������ (�b)</param>
    private IEnumerator SetJumpFalseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // �W�����v�A�j���[�V�������I��
        if (_animator != null)
        {
            _animator.SetBool("Jump", false);
        }

        // �n�ʂɂ����Ԃɐݒ�
        _isGrounded = true;
    }
}
