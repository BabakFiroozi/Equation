using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace WiniGames.Server.Core
{
    public class HttpHandler
    {
        public static string[] HeaderNames 
        {
            get
            {
                var arr = new[] {"Content-Type", "Access-Control-Allow-Origin", "Access-Control-Allow-Headers", "Authorization"};
                return arr;
            }
        }


        public static string[] HeaderValues
        {
            get
            {
                var arr = new [] {"application/json", "*", "Authorization", $"Bearer {LoginPrefs.GetToken()}"};
                return arr;
            }
        }


        public static async Task<string> PostAsync(string url, string json)
        {
            Debug.Log($"PostAsync body : {json}");
            
            try
            {
                using (var req = UnityWebRequest.Put(url, json))
                {
                    req.method = UnityWebRequest.kHttpVerbPOST;

                    for (int i = 0; i < HeaderNames.Length; i++)
                        req.SetRequestHeader(HeaderNames[i], HeaderValues[i]);

                    Debug.Log($"HttpHandler Post Request to URL {url} - content type: {req.GetRequestHeader("Content-Type")}");
                    
                    await req.SendWebRequest();

                    if (string.IsNullOrWhiteSpace(req.error))
                    {
                        Debug.Log($"HttpHandler Post Response from URL {url}: {req.downloadHandler.text}");
                        return req.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"Error HttpPost from URL {url} - code: {req.responseCode}, error: {req.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error PostAsync, {ex.Message}");
            }
            return null;
        }
        
        
        public static async Task<string> PostMultiPartAsync(string url, string json)
        {
            Debug.Log($"PostAsync body : {json}");
            
            try
            {
                int partSize = 1000;
                int partsCount = json.Length / partSize;
                int lastPartSize = json.Length % partSize;
                
                var parts = new List<string>();

                for (int i = 0; i < partsCount; ++i)
                    parts.Add(json.Substring(i * partSize, partSize));
                parts.Add(json.Substring(partsCount * partSize, lastPartSize));

                var formData = new List<IMultipartFormSection>();
                foreach (var p in parts)
                    formData.Add(new MultipartFormDataSection(p));
                
                using (var req = UnityWebRequest.Post(url, formData))
                {
                    req.method = UnityWebRequest.kHttpVerbPOST;

                    req.SetRequestHeader("Content-Type", "multipart/form-data");
                    for (int i = 1; i < HeaderNames.Length; i++)
                        req.SetRequestHeader(HeaderNames[i], HeaderValues[i]);

                    Debug.Log($"HttpHandler PostMultiPart Request to URL {url} - content type: {req.GetRequestHeader("Content-Type")}");
                    
                    await req.SendWebRequest();

                    if (string.IsNullOrWhiteSpace(req.error))
                    {
                        Debug.Log($"HttpHandler Post Response from URL {url}: {req.downloadHandler.text}");
                        return req.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"Error HttpPost from URL {url} - code: {req.responseCode}, error: {req.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error PostMultiPartAsync, {ex.Message}");
            }
            return null;
        }
        
        public static async Task<string> PostFormAsync(string url, string json)
        {
            Debug.Log($"PostAsync body : {json}");
            
            try
            {
                var form = new WWWForm();
                form.AddField("data", json);
                
                using (var req = UnityWebRequest.Post(url, form))
                {
                    req.method = UnityWebRequest.kHttpVerbPOST;

                    req.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                    for (int i = 1; i < HeaderNames.Length; i++)
                        req.SetRequestHeader(HeaderNames[i], HeaderValues[i]);

                    Debug.Log($"HttpHandler PostForm Request to URL {url} - content type: {req.GetRequestHeader("Content-Type")}");
                    
                    await req.SendWebRequest();

                    if (string.IsNullOrWhiteSpace(req.error))
                    {
                        Debug.Log($"HttpHandler Post Response from URL {url}: {req.downloadHandler.text}");
                        return req.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"Error HttpPost from URL {url} - code: {req.responseCode}, error: {req.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error PostFormAsync, {ex.Message}");
            }
            return null;
        }
        
        
        public static async Task<string> PostStreamAsync(string url, string json)
        {
            Debug.Log($"PostAsync body : {json}");
            
            try
            {
                var myData = Encoding.UTF8.GetBytes(json);
                
                using (var req = UnityWebRequest.Put(url, myData))
                {
                    req.method = UnityWebRequest.kHttpVerbPOST;

                    req.SetRequestHeader("Content-Type", "application/octet-stream");
                    for (int i = 1; i < HeaderNames.Length; i++)
                        req.SetRequestHeader(HeaderNames[i], HeaderValues[i]);

                    Debug.Log($"HttpHandler PostStream Request to URL {url} - content type: {req.GetRequestHeader("Content-Type")}");
                    
                    await req.SendWebRequest();

                    if (string.IsNullOrWhiteSpace(req.error))
                    {
                        Debug.Log($"HttpHandler PostStream Response from URL {url}: {req.downloadHandler.text}");
                        return req.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"Error HttpPost from URL {url} - code: {req.responseCode}, error: {req.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error PostStreamAsync, {ex.Message}");
            }
            return null;
        }
        

        public static async Task<string> GetAsync(string url)
        {
            try
            {
                using (var req = UnityWebRequest.Get(url))
                {
                    req.method = UnityWebRequest.kHttpVerbGET;
                    
                    for (int i = 0; i < HeaderNames.Length; i++)
                        req.SetRequestHeader(HeaderNames[i], HeaderValues[i]);

                    Debug.Log($"HttpHandler Get Request to URL {url} - content type: {req.GetRequestHeader("Content-Type")}");
                    
                    await req.SendWebRequest();

                    if (string.IsNullOrWhiteSpace(req.error))
                    {
                        Debug.Log($"HttpHandler Get Response from URL {url}: {req.downloadHandler.text}");
                        return req.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"Error HttpGet from URL {url} - code: {req.responseCode}, error: {req.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error GetAsync, {ex.Message}");
            }
            return null;
        }
        
        public static async Task<string> PutAsync(string url, string json)
        {
            try
            {
                using (var req = UnityWebRequest.Put(url, json))
                {
                    req.method = UnityWebRequest.kHttpVerbPUT;
                    
                    for (int i = 0; i < HeaderNames.Length; i++)
                        req.SetRequestHeader(HeaderNames[i], HeaderValues[i]);
                    
                    Debug.Log($"HttpHandler Put Request to URL {url} - content type: {req.GetRequestHeader("Content-Type")}");
                    
                    await req.SendWebRequest();

                    if (string.IsNullOrWhiteSpace(req.error))
                    {
                        Debug.Log($"HttpHandler Put Response from URL {url}: {req.downloadHandler.text}");
                        return req.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError($"Error HttpPut from URL {url} - code: {req.responseCode}, error: {req.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error PutAsync, {ex.Message}");
            }
            return null;
        }
        
        
        // //for test
        // public static async Task<string> PostAsync2(string url, string json)
        // {
        //     Debug.Log($"PostAsync body : {json}");
        //
        //     try
        //     {
        //         var http = new HttpClient();
        //         {
        //             http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Bearer {LoginPrefs.GetToken()}");
        //             
        //             for (int i = 1; i < HeaderNames.Length - 1; i++)
        //                 http.DefaultRequestHeaders.Add(HeaderNames[i], HeaderValues[i]);
        //
        //             var content = new StringContent(json, Encoding.UTF8, "application/json");
        //
        //             var response = await http.PostAsync(url, content);
        //
        //             if (response.IsSuccessStatusCode)
        //                 return response.Content.ReadAsStringAsync().Result;
        //
        //             Debug.LogError($"Error HttpClient - Post. RequestMessage: {response.RequestMessage}, StatusCode: {response.StatusCode}");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError($"Error PostAsync, {ex}");
        //     }
        //
        //     return null;
        // }
    }
}