using UnityEngine;

public class GameSessionCleaner : MonoBehaviour
{  
    void Start()
    {
        SIS_GameSession.Instance?.ClearSession();
    }
}
