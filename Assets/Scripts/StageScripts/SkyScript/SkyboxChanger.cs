using System.Collections;
using UnityEngine;

/// <summary>
/// ���Ԍo�߂ŋ�̐F��ω�������N���X
/// </summary>
public class SkyboxChanger : MonoBehaviour
{
    // Skybox �؂�ւ��Ԋu�i�b�j
    private const float DEFAULT_CHANGE_INTERVAL = 20f;

    // Skybox �t�F�[�h���ԁi�b�j
    private const float DEFAULT_TRANSITION_DURATION = 5f;

    // Skybox �̃C���f�b�N�X�����l
    private const int INDEX_INCREMENT = 1;

    // 1�t���[�����Ƃɑҋ@���鎞�ԁi�b�j
    private const float FRAME_WAIT_TIME = 0f;

    // �؂�ւ��� Skybox �̃}�e���A���ꗗ�iInspector �Őݒ�j
    [SerializeField] private Material[] skyboxMaterials;

    // Skybox ��ύX����Ԋu�i�b�j
    [SerializeField] private float changeInterval = DEFAULT_CHANGE_INTERVAL;

    // Skybox �̃t�F�[�h�ɂ����鎞�ԁi�b�j
    [SerializeField] private float transitionDuration = DEFAULT_TRANSITION_DURATION;

    // ���݂� Skybox �C���f�b�N�X�i�ŏ��̃}�e���A���j
    private int currentIndex = 0;

    void Start()
    {
        // Skybox �̕ύX�������R���[�`���Ŏ��s�J�n
        StartCoroutine(ChangeSkyboxRoutine());
    }

    IEnumerator ChangeSkyboxRoutine()
    {
        while (true)
        {
            // ���� Skybox �̃C���f�b�N�X������i���[�v����悤�Ɂj
            int nextIndex = (currentIndex + INDEX_INCREMENT) % skyboxMaterials.Length;

            // ���݂� Skybox �}�e���A�����擾
            Material currentSkybox = skyboxMaterials[currentIndex];

            // ���ɓK�p���� Skybox �}�e���A�����擾
            Material nextSkybox = skyboxMaterials[nextIndex];

            // ��x�A���݂� Skybox ��ݒ�iLerp �̃x�[�X�Ƃ��Ďg�p�j
            RenderSettings.skybox = currentSkybox;

            // ���C�e�B���O�����X�V�i�ύX�𔽉f������j
            DynamicGI.UpdateEnvironment();

            // ���X�� Skybox ���t�F�[�h������
            float elapsedTime = 0f; // �o�ߎ��Ԃ�������

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime; // �o�ߎ��Ԃ��X�V
                float t = Mathf.Clamp01(elapsedTime / transitionDuration); // 0 ���� 1 �փX���[�Y�ɕω�����l

                // Lerp �� Skybox �̃}�e���A�������炩�ɕ��
                RenderSettings.skybox.Lerp(currentSkybox, nextSkybox, t);

                // ���C�e�B���O�����X�V
                DynamicGI.UpdateEnvironment();

                // ���̃t���[���܂őҋ@
                yield return new WaitForSeconds(FRAME_WAIT_TIME);
            }

            // �ŏI�I�� Skybox �Ɋm��i�덷���Ȃ������߁j
            RenderSettings.skybox = nextSkybox;

            // ���C�e�B���O�����X�V
            DynamicGI.UpdateEnvironment();

            // ���� Skybox �ɐ؂�ւ�
            currentIndex = nextIndex;

            // ���̕ύX�܂őҋ@
            yield return new WaitForSeconds(changeInterval);
        }
    }
}