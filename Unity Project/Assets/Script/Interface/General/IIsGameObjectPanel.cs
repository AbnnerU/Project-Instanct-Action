using UnityEngine;

public interface IIsGameObjectPanel : IPanelHandler
{
    void SetObjectReference(GameObject agent);
    void ClearObjectReference();
}