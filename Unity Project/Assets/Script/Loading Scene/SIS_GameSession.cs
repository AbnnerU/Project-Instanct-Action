using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SIS_GameSession : Singleton<SIS_GameSession>
{
    public string ID;
 
    public void ClearSession()
    {
        Debug.Log("Cleaning Session");

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

    }

    public void CleanOldTerrariumData()
    {

    }
}
