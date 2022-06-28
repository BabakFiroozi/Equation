using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WiniGames.Server.Core;

public class Reqqq : MonoBehaviour
{
    [SerializeField] Sprite _file;
    
    // Start is called before the first frame update
    void Start()
    {
        Func();
    }

    async void Func()
    {
        var str = await UploadImage();
        File.WriteAllText(Application.persistentDataPath + "/facedata.txt",str);

       // var str = await DownloadFaceData();
       // File.WriteAllText(Application.persistentDataPath + "/facedata22.txt",str);
    }

    async Task<string> UploadImage()
    {
        var reqData = new UploadRequest
        {
            api_key = "d45fd466-51e2-4701-8da8-04351c872236",
            file_uri = "https://s25.picofile.com/file/8451230876/Clipboard_1.png",
            original_filename = "Clipboard_1.png",
            recognize_targets = new []{"all@mynamespace"},
            detection_flags = "basicpoints,propoints,classifiers,content"
        };

        string json = JsonUtility.ToJson(reqData);
        string url = "https://www.betafaceapi.com/api/v2/media";
        var str = await HttpHandler.PostAsync(url, json);
        return str;
    }
    
    async Task<string> DownloadFaceData()
    {
        string apiKey = "d45fd466-51e2-4701-8da8-04351c872236";
        string faceId = "9e3e88cc-f707-11ec-a8cf-0cc47a6c4dbd";
        string url = $"https://www.betafaceapi.com/api/v2/face?api_key={apiKey}&face_uuid={faceId}";
        var str = await HttpHandler.GetAsync(url);
        return str;
    }
    
    [Serializable]
    public class UploadRequest
    {
        public string api_key;
        public string file_uri;
        public string detection_flags;
        public string[] recognize_targets;
        public string original_filename;
    }
}
