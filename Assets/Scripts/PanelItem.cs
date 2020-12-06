using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private ItemsController _itemsController;
    private List<RectTransform> _uiElements;
    private Sprite _sprite;
    private SceneItem _sceneItem;
    private Vector3 _shift;
    public void SetItemsController(ItemsController itemsController)
    {
        _itemsController = itemsController;
    }
    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
    }

    public void SetRectsUIElements(List<RectTransform> rects)
    {
        _uiElements = rects;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var item = new GameObject(_sprite.name);

        var spriteRenderer = item.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _sprite;

        _sceneItem = item.AddComponent<SceneItem>();
        InitShift();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveToCursor();        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (OverlapExist())
        {
            Destroy(_sceneItem.gameObject);
            return;
        }

        var pos = _sceneItem.gameObject.transform.position;
        pos.z = 0;
        _sceneItem.gameObject.transform.position = pos;
        _itemsController.AddItem(_sceneItem);
    }

    private bool OverlapExist()
    {
        var colider = _sceneItem.gameObject.AddComponent<PolygonCollider2D>();
        if (_uiElements == null || _uiElements.Count <= 0) return false;

        Rect rect = new Rect(colider.bounds.center - colider.bounds.extents, colider.bounds.size);
        var min = RectTransformUtility.WorldToScreenPoint(Camera.main, rect.min);
        var max = RectTransformUtility.WorldToScreenPoint(Camera.main, rect.max);

        for (int i = 0; i < _uiElements.Count; i++)
        {
            if (
                ContainsPoint(_uiElements[i], min) ||
                ContainsPoint(_uiElements[i], max) ||
                ContainsPoint(_uiElements[i], new Vector2(min.x, max.y)) ||
                ContainsPoint(_uiElements[i], new Vector2(max.x, min.y)))
            {
                return true;
            }
        }

        return false;
    }

    private bool ContainsPoint(RectTransform rectTransform, Vector2 point)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, point, Camera.main, out var localPoint);
        return rectTransform.rect.Contains(localPoint);
    }

    private void MoveToCursor()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        _sceneItem.transform.position = position + _shift;
    }
    private void InitShift()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 1;

        _shift = transform.position - position;
    }
}
