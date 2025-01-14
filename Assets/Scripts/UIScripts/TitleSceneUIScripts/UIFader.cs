using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Start��UI��_�ł�����N���X
/// </summary>
public class UIFader : MonoBehaviour
{
    // �t�F�[�h�Ώۂ�Image�R���|�[�l���g
    [SerializeField] private Image startImage;

    // �t�F�[�h�C���E�A�E�g�̎����i�b�P�ʁj
    [SerializeField] private float fadeDuration = 1f;

    // �X�N���v�g�J�n���Ƀt�F�[�h�������I�ɊJ�n���邩�ǂ����̃t���O
    [SerializeField] private bool startFading = true;

    // ���݂̃t�F�[�h�������Ǘ�����t���O�itrue: �t�F�[�h�C��, false: �t�F�[�h�A�E�g�j
    private bool isFadingIn = true;

    // ���݂̃A���t�@�l�i�����x�j
    private float currentAlpha = 1f;

    private void Start()
    {
        // startImage���ݒ肳��Ă��Ȃ��ꍇ�A�A�^�b�`����Ă���Image�R���|�[�l���g���擾
        if (startImage == null)
        {
            startImage = GetComponent<Image>();
        }

        // startFading��true�̏ꍇ�A�t�F�[�h�������J�n
        if (startFading)
        {
            StartCoroutine(FadeLoop());
        }
    }

    /// <summary>
    /// �t�F�[�h�������J��Ԃ��R���[�`���B
    /// �t�F�[�h�C���ƃt�F�[�h�A�E�g�����݂Ɏ��s�B
    /// </summary>
    private System.Collections.IEnumerator FadeLoop()
    {
        while (true)
        {
            // �t�F�[�h�̊J�n�ƏI���̃A���t�@�l��ݒ�
            float elapsedTime = 0f;
            float startAlpha = isFadingIn ? 0f : 1f;
            float endAlpha = isFadingIn ? 1f : 0f;

            // �w�肳�ꂽfadeDuration�̊ԂŃA���t�@�l����
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime; // �o�ߎ��Ԃ����Z
                currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration); // ���`���
                UpdateAlpha(currentAlpha); // �A���t�@�l���X�V
                yield return null; // ���̃t���[���܂őҋ@
            }

            // �t�F�[�h�̕����𔽓]
            isFadingIn = !isFadingIn;
        }
    }

    /// <summary>
    /// �摜�̃A���t�@�l���X�V����B
    /// </summary>
    /// <param name="alpha">�X�V����A���t�@�l�i0�`1�j</param>
    private void UpdateAlpha(float alpha)
    {
        if (startImage != null)
        {
            Color color = startImage.color; // ���݂̐F���擾
            color.a = alpha; // �A���t�@�l��ݒ�
            startImage.color = color; // �X�V���ꂽ�F�𔽉f
        }
    }

    /// <summary>
    /// �t�F�[�h�������J�n����B
    /// </summary>
    public void StartFading()
    {
        startFading = true;
        StartCoroutine(FadeLoop());
    }

    /// <summary>
    /// �t�F�[�h�������~����B
    /// </summary>
    public void StopFading()
    {
        startFading = false;
        StopAllCoroutines(); // �S�ẴR���[�`�����~
    }
}
