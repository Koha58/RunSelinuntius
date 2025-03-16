using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FinalAttackUI(�N���A���邽�߂ɕK�v�ȍU���pUI)�Ǘ��N���X
/// </summary>
public class FinalAttackUIControl : MonoBehaviour
{
    [SerializeField] private Image pressUI;  // ���� UI
    [SerializeField] private Image keyboardUI;  // �L�[�{�[�h�p UI
    [SerializeField] private Image controllerUI; // �R���g���[���[�p UI
    [SerializeField] private Color blinkColor = Color.gray; // �_�Ŏ��̐F
    [SerializeField] private float blinkInterval = 0.5f; // �_�ŊԊu�i�b�j
    [SerializeField] private PlayerMove playerMove; // PlayerMove �̎Q��

    private bool isBlinking = false;   // ���ݓ_�Œ����ǂ���

    private void Awake()
    {
        // �����I�u�W�F�N�g�ɃA�^�b�`���ꂽ PlayerMove ���擾
        playerMove = GetComponent<PlayerMove>();
        pressUI.enabled = false;
    }

    void Update()
    {
        if (playerMove == null || InputDeviceManager.Instance == null) return;

        // ���݂̓��̓f�o�C�X���擾�itrue: �R���g���[���[, false: �L�[�{�[�h�j
        bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();

        // �v���C���[�� FinalAttack �\�����擾
        bool isNearTarget = playerMove.IsFinalAttackPossible();

        // UI �̕\�� / ��\��
        keyboardUI.enabled = isNearTarget && !isUsingGamepad;
        controllerUI.enabled = isNearTarget && isUsingGamepad;

        // �_�ŏ������J�n�E��~
        if (isNearTarget && !isBlinking)
        {
            StartCoroutine(BlinkUI());
        }
        else if (!isNearTarget && isBlinking)
        {
            StopCoroutine(BlinkUI());
            ResetUIColor();
        }
    }

    /// <summary>
    /// UI ��_�ł�����R���[�`��
    /// </summary>
    private IEnumerator BlinkUI()
    {
        isBlinking = true;
        pressUI.enabled = true;
        while (true)
        {
            // ���݂� UI ���擾
            bool isUsingGamepad = InputDeviceManager.Instance.IsUsingGamepad();
            Image activeUI = isUsingGamepad ? controllerUI : keyboardUI;
            Image uiImage = activeUI.GetComponent<Image>();

            if (uiImage != null)
            {
                // �F��؂�ւ���
                uiImage.color = (uiImage.color == Color.white) ? blinkColor : Color.white;
            }

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// UI �̐F�����ɖ߂�
    /// </summary>
    private void ResetUIColor()
    {
        keyboardUI.GetComponent<Image>().color = Color.white;
        controllerUI.GetComponent<Image>().color = Color.white;
        pressUI.enabled = false;
        isBlinking = false;
    }
}