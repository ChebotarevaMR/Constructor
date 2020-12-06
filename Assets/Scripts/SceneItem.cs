using UnityEngine;

public class SceneItem : MonoBehaviour
{
	private SpriteRenderer _spriteRenderer;
	public bool IsSelected { get; private set; }
    public void Select()
	{
		if(_spriteRenderer == null)
        {
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		IsSelected = true;
		_spriteRenderer.color = Color.green;
	}

	public void Unselect()
	{
		if (_spriteRenderer == null)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		IsSelected = false;
		_spriteRenderer.color = Color.white;
	}
}
