using UnityEngine;

public interface IHasViewport
{
    RectTransform Viewport { get; }
    bool IsViewOpen { get; }
    float ViewOrthographicSize { get; }
}
