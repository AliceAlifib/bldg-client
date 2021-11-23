using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;


namespace ImageUtils
{
    public class ImageController : MonoBehaviour
    {

        public string _imageUrl;
        public string _linkURL;

        public GameObject _imageDisplay;
        Texture2D _texture;


        public void SetImageURL(string imageURL) {
            //_texture = await GetRemoteTexture(_imageUrl);
            //_material.mainTexture = _texture;
            StartCoroutine(DownloadImage(_imageUrl, _imageDisplay.GetComponent<Renderer>().material));
        }


        IEnumerator DownloadImage(string mediaUrl, Material targetMaterial)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                //this.gameObject.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                _texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetMaterial.mainTexture = _texture;
        }


        void OnDestroy () => Dispose();
        public void Dispose () => Object.Destroy(_texture);// memory released, leak otherwise

    }
}