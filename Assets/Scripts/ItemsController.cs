using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
	private List<SceneItem> _items = new List<SceneItem>();
	private List<SceneItem> _selected = new List<SceneItem>();

	public event Action<GameObject> SettingsAvailable;
	public event Action SettingsNotAvailable;

	public void AddItem(SceneItem item)
	{
		_items.Add(item);
	}
	public void SelectItemsFromRect(Rect rect)
	{
		ClearSelected();
		for (int i = 0; i < _items.Count; i++)
		{
			var position = _items[i].transform.position;
			Vector2 screenPos = new Vector2(Camera.main.WorldToScreenPoint(position).x, Screen.height - Camera.main.WorldToScreenPoint(position).y);

			if (rect.Contains(screenPos))
			{
				if (!_items[i].IsSelected)
					_items[i].Select();
				_selected.Add(_items[i]);
			}
		}
		if (_selected.Count == 1)
		{
			SettingsAvailable?.Invoke(_selected[0].gameObject);
		}
		else
		{
			SettingsNotAvailable?.Invoke();
		}
	}

	public void SelectItem(GameObject item)
	{
		ClearSelected();
		var sceneItem = item.GetComponent<SceneItem>();
		if (_items.Contains(sceneItem))
		{
			sceneItem.Select();
			_selected.Add(sceneItem);
		}
		SettingsAvailable?.Invoke(sceneItem.gameObject);
	}
	public void ResetSelect()
	{
		ClearSelected();
		SettingsNotAvailable?.Invoke();
	}

	private void ClearSelected()
	{
		if (_selected.Count > 0)
		{
			for (int i = 0; i < _selected.Count; i++)
			{
				if (_selected[i].IsSelected)
					_selected[i].Unselect();
			}
			_selected.Clear();
		}
	}
	public void SelectedItemsMove(Vector3 delta)
	{
		if (_selected.Count > 0)
		{
			for (int i = 0; i < _selected.Count; i++)
			{
				_selected[i].transform.position += delta;
			}
		}
	}
	public bool SelectedContains(GameObject item)
	{
		var sceneItem = item.GetComponent<SceneItem>();
		if (item == null) return false;
		return _selected.Contains(sceneItem);
	}

	public void Save(Serializer serializer)
	{
		List<Item> items = new List<Item>();
		if (_items != null && _items.Count > 0)
		{
			foreach (var item in _items)
			{
				var transform = item.gameObject.transform;
				var spriteName = item.GetComponent<SpriteRenderer>().sprite.name;
				var serializeItem = new Item(transform.position, transform.rotation, transform.localScale, spriteName);
				items.Add(serializeItem);
			}
		}

		serializer.Serialize(items);
	}

	public void DeleteSelected()
	{
		DeleteSelectedItems();
	}
	public void DeleteAll()
	{
		DeleteSelectedItems();
		while (_items.Count > 0)
		{
			var item = _items[0];
			_items.RemoveAt(0);
			Destroy(item.gameObject);
		}
	}
	private void DeleteSelectedItems()
	{
		while (_selected.Count > 0)
		{
			var item = _selected[0];
			int index = _items.IndexOf(item);
			if (index >= 0) _items.RemoveAt(index);
			_selected.Remove(item);
			Destroy(item.gameObject);
		}
		SettingsNotAvailable?.Invoke();
	}
}
