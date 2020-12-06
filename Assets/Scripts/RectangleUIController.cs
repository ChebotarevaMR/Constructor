using UnityEngine;

public class RectangleUIController : MonoBehaviour
{
    private Rect _rect;
    private bool _isDraw;
    public void DrawRectangle(Rect rect)
    {
        _rect = rect;
        _isDraw = true;
    }

    public void CancelDraw()
    {
        _isDraw = false;
    }

    private void OnGUI()
    {
        if (!_isDraw) return;
        GUI.Box(_rect, "");
    }
}
