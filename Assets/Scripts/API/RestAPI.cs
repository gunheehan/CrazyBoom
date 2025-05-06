using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class RestAPI
{
    //Header DataSetting
    private static void SetHeader(HttpClient client, Dictionary<string, string> headers)
    {
        foreach (var item in headers)
        {
            client.DefaultRequestHeaders.Add(item.Key, item.Value);
        }
    }
    
    // Dictionary Serialize Json
    private static string ToJson(Dictionary<string, string> body)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");

        foreach (var item in body)
        {
            sb.AppendFormat("\"{0}\":\"{1}\",", item.Key, item.Value);
        }

        if (sb.Length > 1)
        {
            sb.Length--;
        }

        sb.Append("}");

        return sb.ToString();
    }

    public static async Task<string> GetAsync(string url, Dictionary<string, string> headers = null)
    {
        using (var client = new HttpClient())
        {
            try
            {
                if (headers != null)
                    SetHeader(client, headers);

                var response = await client.GetAsync(url);

                // 요청 실패시 실패 내용 반환
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {errorContent}";
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    public static async Task<string> PostAsync(string url, Dictionary<string, string> headers = null,
        Dictionary<string, string> body = null)
    {
        using (var client = new HttpClient())
        {
            try
            {
                if (headers != null)
                    SetHeader(client, headers);

                HttpContent content = null;
                if (body != null)
                {
                    var jsonBody = ToJson(body);
                    content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }
                else
                {
                    content = new StringContent(string.Empty);
                }

                var response = await client.PostAsync(url, content);
                
                // 요청 실패시 실패 내용 반환
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {errorContent}";
                }
                
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    public static async Task<string> DeleteAsync(string url, Dictionary<string, string> headers = null)
    {
        using (var client = new HttpClient())
        {
            try
            {
                if (headers != null)
                    SetHeader(client, headers);

                var response = await client.DeleteAsync(url);
                
                // 요청 실패시 실패 내용 반환
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {errorContent}";
                }
                
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
    
    public static async Task<string> PutAsync(string url, Dictionary<string, string> headers = null,
        Dictionary<string, string> body = null)
    {
        using (var client = new HttpClient())
        {
            try
            {
                if (headers != null)
                    SetHeader(client, headers);

                HttpContent content = null;
                if (body != null)
                {
                    var jsonBody = ToJson(body);
                    content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }
                else
                {
                    content = new StringContent(string.Empty);
                }

                var response = await client.PutAsync(url, content);
                
                // 요청 실패시 실패 내용 반환
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {errorContent}";
                }
                
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
    
    public static T ParseJson<T>(string json) where T : new()
    {
        var obj = new T();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        json = json.Trim().Trim('{', '}');
        var pairs = json.Split(',').Select(p => p.Split(':')).Where(p => p.Length == 2);

        var dict = new Dictionary<string, string>();
        foreach (var pair in pairs)
        {
            var key = pair[0].Trim().Trim('"');
            var value = pair[1].Trim().Trim('"');
            dict[key] = value;
        }

        foreach (var prop in properties)
        {
            if (dict.TryGetValue(prop.Name, out var valueStr))
            {
                object converted = null;

                if (prop.PropertyType == typeof(int))
                    converted = int.Parse(valueStr);
                else if (prop.PropertyType == typeof(float))
                    converted = float.Parse(valueStr);
                else if (prop.PropertyType == typeof(bool))
                    converted = bool.Parse(valueStr);
                else if (prop.PropertyType == typeof(DateTime))
                    converted = DateTime.Parse(valueStr);
                else if (prop.PropertyType == typeof(string))
                    converted = valueStr;
                
                if (converted != null)
                    prop.SetValue(obj, converted);
            }
        }

        return obj;
    }

}