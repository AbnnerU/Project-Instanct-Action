using System;

public interface IPanelHandler
{
    event Action<bool> OnPanelOpen;

    bool IsPanelOpen { get; set; }

    void OpenPanel();

    void ClosePanel();
}
