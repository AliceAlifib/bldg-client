using System;
using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using Utils;


[CreateAssetMenu(fileName = "Config", menuName = "General Config", order = 1)]
public class GlobalConfig : ScriptableObjectSingleton<GlobalConfig>
{   

    //public string bldgServer = "https://api.w2m.site";
    public string bldgServer = "https://localhost:4443";
    public string residentsBasePath = "/v1/residents";
	public string bldgsBasePath = "/v1/bldgs";
	public string roadsBasePath = "/v1/roads";


    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
        //bldgServer = "http://localhost:4443";
    }

}