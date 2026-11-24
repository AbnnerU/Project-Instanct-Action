using System;

public interface IIdentifiableItem 
{
    public string GetItemId();
    public void SetItemId(Guid id);
    public void SetItemId(string id);
}
