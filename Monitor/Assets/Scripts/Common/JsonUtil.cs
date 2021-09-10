using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUtil
{
    static JsonSerializerSettings settings = new JsonSerializerSettings();

    public static T String2Json<T>(string jsonString)
    {
        try
        {
            T json = JsonConvert.DeserializeObject<T>(jsonString);
            return json;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><JsonUtil.String2Json>Error: {0}", ex.Message);
            return default(T);
        }
    }

    public static string Json2String(object jsonObject)
    {
        try
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
            string jsonStr = JsonConvert.SerializeObject(jsonObject, settings);
            return jsonStr;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><JsonUtil.Json2String>Error: {0}", ex.Message);
            return string.Empty;
        }
    }

    public static JsonData JsonStr2JsonData(string jsonString)
    {
        try
        {
            JsonData data = JsonMapper.ToObject(jsonString);
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><JsonUtil.JsonStr2JsonData>Error: {0}", ex.Message);
            return null;
        }
    }

    public static string Dictionary2String(Dictionary<string, string> dictionary)
    {
        if (dictionary == null) return string.Empty;
        try
        {
            JsonData data = new JsonData();
            foreach (KeyValuePair<string, string> info in dictionary)
            {
                data[info.Key] = info.Value;
            }
            string json = data.ToJson();
            return json;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><JsonUtil.Dictionary2String>Error: {0}", ex.Message);
            return string.Empty;
        }
    }
}
