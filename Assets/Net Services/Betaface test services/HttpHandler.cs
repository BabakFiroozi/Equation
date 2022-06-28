using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;

namespace WiniGames.Server.Core
{
    public static class HttpHandler
    {

        public static async Task<string> PostAsync(string url, string json)
        {
            Debug.Log($"PostAsync body : {json}");
            
            try
            {
                using var req = UnityWebRequest.Put(url, json);
                req.method = UnityWebRequest.kHttpVerbPOST;

                string[] headerKeys = { "Content-Type"};
                string[] headerValues = { "application/json" };

                for (int i = 0; i < headerKeys.Length; i++)
                    req.SetRequestHeader(headerKeys[i], headerValues[i]);

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
            catch (Exception ex)
            {
                Debug.LogError($"Error PostAsync, {ex.Message}");
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
                    
                    string[] headerKeys = { "Content-Type"};
                    string[] headerValues = { "application/json" };
                    
                    for (int i = 0; i < headerKeys.Length; i++)
                        req.SetRequestHeader(headerKeys[i], headerValues[i]);

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
    }
    
    
    
    public class UnityWebRequestAwaiter : INotifyCompletion
    {
        UnityWebRequestAsyncOperation asyncOp;
        Action continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            this.asyncOp = asyncOp;
            asyncOp.completed += OnRequestCompleted;
        }

        public bool IsCompleted => asyncOp.isDone;

        public void GetResult() { }

        public void OnCompleted(Action continution)
        {
            this.continuation = continution;
        }

        private void OnRequestCompleted(AsyncOperation obj)
        {
            continuation();
        }
    }

    public static class ExtensionMethods
    {
        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
    
}