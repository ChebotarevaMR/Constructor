using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public ItemsController ItemsController;
    public RectangleUIController RectangleUIController;

    private Rect _rect;
    private Vector3 _startPos, _endPos, _oldPos;
    private bool _isDown, _isMove, _isClickEmpty, _isClickSelected;

    void Update()
    {
        if (_isDown && !_isMove && _startPos != Input.mousePosition)
        {
            _isMove = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _isDown = true;
            _startPos = Input.mousePosition;
            _isClickEmpty = CliskOverEmpty();
            if (!_isClickEmpty)
            {
                _isClickSelected = CliskOverSelected(out var item);
                _oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if(!_isClickSelected && item != null)
                {
                    ItemsController.SelectItem(item);
                    _isClickSelected = true;
                }

                // TODO Reser or not reset selection if click on UI?
                /*if (!_isClickSelected)
                {                    
                    ItemsController.ResetSelected();
                }*/
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_isClickEmpty && _isMove)
            {
                RectangleUIController.CancelDraw();
                ItemsController.SelectItemsFromRect(_rect);
            }
            if(_isClickEmpty && !_isMove)
            {
                ItemsController.ResetSelect();
            }

            _isDown = false;
            _isMove = false;
            _isClickEmpty = false;
            _isClickSelected = false;
        }
        if (_isClickEmpty && _isMove)
        {
            _endPos = Input.mousePosition;
            _rect = GetRect();
            RectangleUIController.DrawRectangle(_rect);
            ItemsController.SelectItemsFromRect(_rect);
        }
        if (_isClickSelected && _isMove)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ItemsController.SelectedItemsMove(pos - _oldPos);
            _oldPos = pos;
        }
    }

    private Rect GetRect()
    {
        return new Rect(Mathf.Min(_endPos.x, _startPos.x),
                            Screen.height - Mathf.Max(_endPos.y, _startPos.y),
                            Mathf.Max(_endPos.x, _startPos.x) - Mathf.Min(_endPos.x, _startPos.x),
                            Mathf.Max(_endPos.y, _startPos.y) - Mathf.Min(_endPos.y, _startPos.y)
                            );
    }

    private bool CliskOverEmpty()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return false;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics2D.Raycast(ray.origin, ray.direction);

        return hits.collider == null;
    }

    private bool CliskOverSelected(out GameObject item)
    {
        item = null;
        if (!EventSystem.current.IsPointerOverGameObject()) return false;
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics2D.Raycast(ray.origin, ray.direction);

        if (hits.collider == null) return false;

        item = hits.collider.gameObject;
        return ItemsController.SelectedContains(hits.collider.gameObject);
    }
}
