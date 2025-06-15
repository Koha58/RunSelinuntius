using System;
using UnityEngine;

/// <summary>
/// Unity��JsonUtility�͔z������̂܂܈����Ȃ����߁A
/// ���b�p�[���g���Ĕz��`����JSON���T�|�[�g����w���p�[�N���X�B
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// JSON�����񂩂�z����f�V���A���C�Y����iJSON �� T[]�j
    /// </summary>
    /// <typeparam name="T">�z��̗v�f�^</typeparam>
    /// <param name="json">JSON�z��`���̕�����i��: "[{...},{...}]"�j</param>
    /// <returns>T�^�̔z��Ƃ��ĕԂ�</returns>
    public static T[] FromJson<T>(string json)
    {
        // �z������b�v�����`���ɕϊ����Ă���ǂݍ���
        return JsonUtility.FromJson<Wrapper<T>>(WrapJsonArray(json)).Items;
    }

    /// <summary>
    /// �z���JSON�`���̕�����ɕϊ�����iT[] �� JSON�j
    /// </summary>
    /// <typeparam name="T">�z��̗v�f�^</typeparam>
    /// <param name="array">�ϊ�����z��</param>
    /// <param name="prettyPrint">���`���Č��₷�����邩�itrue�����j</param>
    /// <returns>JSON�z��`���̕�����</returns>
    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        // Unity��JsonUtility�͒��ڔz����T�|�[�g���Ȃ����߁A
        // �v�f���蓮�ŕ��ׂ�JSON�z�񕶎�����\�z����
        string json = "[\n";
        for (int i = 0; i < array.Length; i++)
        {
            json += JsonUtility.ToJson(array[i], prettyPrint);

            // �Ō�̗v�f�łȂ���΃J���}��t����
            if (i < array.Length - 1)
                json += ",\n";
        }
        json += "\n]";
        return json;
    }

    /// <summary>
    /// JsonUtility�Ŕz���ǂݍ��ނ��߂Ɏg�p����������b�p�[�N���X
    /// </summary>
    /// <typeparam name="T">�z��̗v�f�^</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    /// <summary>
    /// �z��`����JSON��������AJsonUtility���ǂݍ��߂�悤�Ƀ��b�v����
    /// </summary>
    /// <param name="json">����JSON�z��i��: "[{...},{...}]"�j</param>
    /// <returns>{"Items": [...]} �̌`���ɕϊ�����������</returns>
    private static string WrapJsonArray(string json)
    {
        return "{ \"Items\": " + json + "}";
    }
}
