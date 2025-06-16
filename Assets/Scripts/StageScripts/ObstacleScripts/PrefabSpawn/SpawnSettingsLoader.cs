using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// JSON�t�@�C������Prefab�̐����ݒ��ǂݍ��ރR���|�[�l���g
/// </summary>
public class SpawnSettingsLoader : MonoBehaviour
{
    /// <summary>
    /// �ݒ���L�q����JSON�t�@�C���iTextAsset�Ƃ���Inspector����w��j
    /// </summary>
    [SerializeField] private TextAsset jsonFile;

    /// <summary>
    /// �ǂݍ���Prefab�����ݒ�̃��X�g
    /// </summary>
    public List<PrefabSpawnSetting> spawnSettings;

    /// <summary>
    /// �Q�[���J�n�O�iAwake�j��JSON��ǂݍ��݁A�ݒ胊�X�g�Ɋi�[����
    /// </summary>
    private void Awake()
    {
        // JSON�t�@�C���̓��e��List<PrefabSpawnSetting>�ɕϊ��i�f�V���A���C�Y�j
        spawnSettings = JsonConvert.DeserializeObject<List<PrefabSpawnSetting>>(jsonFile.text);

        // �m�F�p���O�F�ŏ��̗v�f�����݂���ꍇ�͖��O��\��
        if (spawnSettings != null && spawnSettings.Count > 0)
        {
            Debug.Log($"1�Ԗڂ�Prefab: {spawnSettings[0].name}");
        }
    }
}