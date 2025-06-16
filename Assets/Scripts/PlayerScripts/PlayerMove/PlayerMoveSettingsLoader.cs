using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Resources�t�H���_����JSON�t�@�C������PlayerMoveSettings��ǂݍ��ރ��[�e�B���e�B�N���X
/// </summary>
public static class PlayerMoveSettingsLoader
{
    // JSON�t�@�C���̃p�X�iResources�t�H���_���̑��΃p�X�A�g���q�s�v�j
    private const string FileName = "Data/PlayerMoveSettings"; // Resources/Data/PlayerMoveSettings.json

    /// <summary>
    /// JSON�t�@�C����ǂݍ��݁APlayerMoveSettings��Ԃ�
    /// </summary>
    /// <returns>�ǂݍ���PlayerMoveSettings�C���X�^���X�B�ǂݍ��ݎ��s����null�B</returns>
    public static PlayerMoveSettings Load()
    {
        // Resources����TextAsset�Ƃ���JSON�t�@�C����ǂݍ���
        TextAsset jsonFile = Resources.Load<TextAsset>(FileName);
        if (jsonFile == null)
        {
            Debug.LogError($"PlayerMoveSettingsLoader: JSON�t�@�C����������܂���BResources/{FileName}.json");
            return null;
        }

        // Newtonsoft.Json���g��JSON�������PlayerMoveSettingsExport�z��Ƀf�V���A���C�Y����
        PlayerMoveSettingsExport[] exports = JsonConvert.DeserializeObject<PlayerMoveSettingsExport[]>(jsonFile.text);
        if (exports == null || exports.Length == 0)
        {
            Debug.LogError("PlayerMoveSettingsLoader: JSON�ɗL���ȃf�[�^���܂܂�Ă��܂���B");
            return null;
        }

        // �z��̐擪�v�f��PlayerMoveSettings�^�ɕϊ����ĕԂ�
        return exports[0].ToSettings();
    }
}