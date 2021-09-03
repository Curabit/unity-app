using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

// public class downloader: MonoBehaviour {
//         void Start() {
//             StartCoroutine(DownloadVideo());
//         }

//         IEnumerator DownloadVideo() {
//             UnityWebRequest www = UnityWebRequest.Get("https://r6---sn-gwpa-cvhk.googlevideo.com/videoplayback?expire=1626176801&ei=wSjtYJL0B6GM8gSA7po4&ip=173.195.15.234&id=o-AD0bt3MNH18syK5eE95nNZpS4v7_7QgHVngBnUViFTNf&itag=18&source=youtube&requiressl=yes&vprv=1&mime=video%2Fmp4&ns=tuvD-Gh_BLO3jdWKWN2WZNYG&gir=yes&clen=9887388&ratebypass=yes&dur=170.225&lmt=1578949922256198&fexp=24001373,24007246&c=WEB&txp=5531432&n=fMjBwba2VGG3MoIPEQ&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cratebypass%2Cdur%2Clmt&sig=AOq0QJ8wRQIgDshkUA1TbXR1YajHuJItWQs05wfPhLLmL6--cbR0P6ACIQDrQLimOfJZzlHIX3Xqo0RD9-OYHjdp9n-Cq07uXfIG3g%3D%3D&redirect_counter=1&rm=sn-ab5yd7d&req_id=d8676807d24ba3ee&cms_redirect=yes&ipbypass=yes&mh=RV&mip=49.36.121.122&mm=31&mn=sn-gwpa-cvhk&ms=au&mt=1626163968&mv=m&mvi=6&pl=25&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AG3C_xAwRAIgDwXqKhSIfA51iDj2J4uZQY_PJxDEUhcBtM8BFcFp4ewCIHYqsoeFNpqUZ9utREVIeit1YejdyszXEEl4Gj2e4sSS");
//             yield return www.SendWebRequest();

//             if(www.isNetworkError || www.isHttpError) {
//                 Debug.Log(www.error);
//             } else {
//                 File.WriteAllBytes("C:\\Users\\Aman Sariya\\Documents\\Curabit\\Software\\sad_unity-master\\video.mp4", www.downloadHandler.data);
//             }
//         }
//     }





public class downloader : MonoBehaviour {

    void Start () {
        StartCoroutine(DownloadFile());
    }

    IEnumerator DownloadFile() {
        UnityWebRequest uwr = UnityWebRequest.Get("https://r6---sn-gwpa-cvhk.googlevideo.com/videoplayback?expire=1626176801&ei=wSjtYJL0B6GM8gSA7po4&ip=173.195.15.234&id=o-AD0bt3MNH18syK5eE95nNZpS4v7_7QgHVngBnUViFTNf&itag=18&source=youtube&requiressl=yes&vprv=1&mime=video%2Fmp4&ns=tuvD-Gh_BLO3jdWKWN2WZNYG&gir=yes&clen=9887388&ratebypass=yes&dur=170.225&lmt=1578949922256198&fexp=24001373,24007246&c=WEB&txp=5531432&n=fMjBwba2VGG3MoIPEQ&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cratebypass%2Cdur%2Clmt&sig=AOq0QJ8wRQIgDshkUA1TbXR1YajHuJItWQs05wfPhLLmL6--cbR0P6ACIQDrQLimOfJZzlHIX3Xqo0RD9-OYHjdp9n-Cq07uXfIG3g%3D%3D&redirect_counter=1&rm=sn-ab5yd7d&req_id=d8676807d24ba3ee&cms_redirect=yes&ipbypass=yes&mh=RV&mip=49.36.121.122&mm=31&mn=sn-gwpa-cvhk&ms=au&mt=1626163968&mv=m&mvi=6&pl=25&lsparams=ipbypass,mh,mip,mm,mn,ms,mv,mvi,pl&lsig=AG3C_xAwRAIgDwXqKhSIfA51iDj2J4uZQY_PJxDEUhcBtM8BFcFp4ewCIHYqsoeFNpqUZ9utREVIeit1YejdyszXEEl4Gj2e4sSS");
        string path = Path.Combine("C:\\Users\\Aman Sariya\\Documents\\Curabit\\Software\\sad_unity-master", "video.mp4"); //Application.persistentDataPath
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
        Debug.Log("Progress1: " + uwr.downloadProgress);
        //Debug.Log("Progress2: " + uwr.downloadHandler.GetProgress());
        if (uwr.result == UnityWebRequest.Result.ConnectionError|| uwr.result == UnityWebRequest.Result.ProtocolError)
            Debug.LogError(uwr.error);
        else
            Debug.Log("File successfully downloaded and saved to " + path);
    }
}





// public class downloader : MonoBehaviour
// {
//     // Start is called before the first frame update
//     DownloadHandlerFile DHF;
//     void Start()
//     {
//         Debug.Log("Hello World!");
//         DHF = new DownloadHandlerFile("C:\\Users\\Aman Sariya\\Documents\\Curabit\\Software\\sad_unity-master\\Assets\\Beach.mp4");
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         DHF.returnProgress();
//     }
// }

// public class DownloadHandlerFile : DownloadHandlerScript
// {

// 	public int contentLength { get { return _received>_contentLength ? _received : _contentLength; } }

// 	private int _contentLength;
// 	private int _received;
// 	private FileStream _stream;

// 	public DownloadHandlerFile(string localFilePath, int bufferSize = 4096, FileShare fileShare = FileShare.ReadWrite) : base(new byte[bufferSize])
// 	{
// 		string directory = Path.GetDirectoryName(localFilePath);
// 		if(!Directory.Exists(directory)) Directory.CreateDirectory(directory);

// 		_contentLength = -1;
// 		_received = 0;
// 		_stream = new FileStream(localFilePath,FileMode.OpenOrCreate, FileAccess.Write, fileShare, bufferSize);
// 	}

// 	protected override float GetProgress ()
// 	{
// 		return contentLength<=0 ? 0 : Mathf.Clamp01((float)_received/(float)contentLength);
// 	}

// 	protected override void ReceiveContentLength (int contentLength)
// 	{
// 		_contentLength = contentLength;
// 	}
		 
// 	protected override bool ReceiveData (byte[] data, int dataLength)
// 	{
// 		if(data==null || data.Length==0) return false;

// 		_received += dataLength;
// 		_stream.Write(data,0,dataLength);

// 		return true;
// 	}

// 	protected override void CompleteContent ()
// 	{
// 		CloseStream();
// 	}

// 	public new void Dispose()
// 	{
// 		CloseStream();
// 		base.Dispose();
// 	}

// 	private void CloseStream()
// 	{
// 		if(_stream!=null){
// 			_stream.Dispose();
// 			_stream = null;
// 		}
// 	}

//     public void returnProgress()
//     {
//         Debug.Log("1 " + _contentLength);
//         Debug.Log("2 " + _received);
//         Debug.Log("3 " + GetProgress());
//     }

// }