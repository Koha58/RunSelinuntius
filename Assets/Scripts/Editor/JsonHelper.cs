using System;
using UnityEngine;

/// <summary>
/// UnityのJsonUtilityは配列をそのまま扱えないため、
/// ラッパーを使って配列形式のJSONをサポートするヘルパークラス。
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// JSON文字列から配列をデシリアライズする（JSON → T[]）
    /// </summary>
    /// <typeparam name="T">配列の要素型</typeparam>
    /// <param name="json">JSON配列形式の文字列（例: "[{...},{...}]"）</param>
    /// <returns>T型の配列として返す</returns>
    public static T[] FromJson<T>(string json)
    {
        // 配列をラップした形式に変換してから読み込む
        return JsonUtility.FromJson<Wrapper<T>>(WrapJsonArray(json)).Items;
    }

    /// <summary>
    /// 配列をJSON形式の文字列に変換する（T[] → JSON）
    /// </summary>
    /// <typeparam name="T">配列の要素型</typeparam>
    /// <param name="array">変換する配列</param>
    /// <param name="prettyPrint">整形して見やすくするか（true推奨）</param>
    /// <returns>JSON配列形式の文字列</returns>
    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        // UnityのJsonUtilityは直接配列をサポートしないため、
        // 要素を手動で並べてJSON配列文字列を構築する
        string json = "[\n";
        for (int i = 0; i < array.Length; i++)
        {
            json += JsonUtility.ToJson(array[i], prettyPrint);

            // 最後の要素でなければカンマを付ける
            if (i < array.Length - 1)
                json += ",\n";
        }
        json += "\n]";
        return json;
    }

    /// <summary>
    /// JsonUtilityで配列を読み込むために使用する内部ラッパークラス
    /// </summary>
    /// <typeparam name="T">配列の要素型</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    /// <summary>
    /// 配列形式のJSON文字列を、JsonUtilityが読み込めるようにラップする
    /// </summary>
    /// <param name="json">元のJSON配列（例: "[{...},{...}]"）</param>
    /// <returns>{"Items": [...]} の形式に変換した文字列</returns>
    private static string WrapJsonArray(string json)
    {
        return "{ \"Items\": " + json + "}";
    }
}
