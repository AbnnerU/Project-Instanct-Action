using UnityEngine;
public interface IHasBounds
{
    Vector3 GetDefaultBoundsSize();

    Vector3 GetCurrentBoundsSize();

    Vector3 GetBoundsOffSet();

    Vector3 GetDefaultBoundsOffSet();

    void MultiplyBounds(Vector3 multiplier, bool apllyToOffSet);

    void MultiplyBounds(float multiplier, bool apllyToOffSet);
}
