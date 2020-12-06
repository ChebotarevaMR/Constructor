using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject ScrollView;
    public ItemsController ItemsController;
    public List<RectTransform> RectsUIElements;
    public GameObject Settings;
    public Slider Scale;
    public Text ScaleText;
    public Slider Rotation;
    public Text RotationText;

    private const float MIN_SCALE = 0;
    private const float MAX_SCALE = 4;
    private const float MIN_ROTATION = 0;
    private const float MAX_ROTATION = 360;
    private const string PATH_SPRITES = "Texture";

    private List<Sprite> _sprites;
    private Serializer _serializer;
    private GameObject _selectObject;
    public void OnShowHideClick()
    {
        ScrollView.SetActive(!ScrollView.activeSelf);
    }

    public void OnSaveClick()
    {
        ItemsController.Save(_serializer);
    }

    public void OnDeleteClick()
    {
        ItemsController.DeleteSelected();
    }

    public void OnDeleteAllClick()
    {
        ItemsController.DeleteAll();
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnValueChangedScale()
    {
        if (_selectObject == null)
        {
            Settings.gameObject.SetActive(false);
            return;
        }
        var value = Scale.value;
        _selectObject.transform.localScale = new Vector3(value, value, 1);
        UpdateScaleText(value);
    }

    public void OnValueChangedRotation()
    {
        if (_selectObject == null)
        {
            Settings.gameObject.SetActive(false);
            return;
        }
        var value = Rotation.value;
        _selectObject.transform.rotation = Quaternion.Euler(0, 0, value);
        UpdateRotationText(value);
    }

    private void Start()
    {
        ScrollView.SetActive(false);
        Settings.gameObject.SetActive(false);

        ItemsController.SettingsAvailable += OnSettingsAvailable;
        ItemsController.SettingsNotAvailable += OnSettingsNotAvailable;

        Rotation.minValue = MIN_ROTATION;
        Rotation.maxValue = MAX_ROTATION;
        Scale.minValue = MIN_SCALE;
        Scale.maxValue = MAX_SCALE;

        LoadSprites();
        CreatePanelItems();
        LoadSceneItems();
    }
    private void OnSettingsNotAvailable()
    {
        _selectObject = null;
        Settings.gameObject.SetActive(false);
    }

    private void OnSettingsAvailable(GameObject obj)
    {
        _selectObject = obj;

        float rotationValue = obj.transform.rotation.eulerAngles.z;
        Rotation.value = rotationValue;
        UpdateRotationText(rotationValue);

        float scaleValue = obj.transform.localScale.x;
        Scale.value = scaleValue;
        UpdateScaleText(scaleValue);

        Settings.gameObject.SetActive(true);
    }

    private void UpdateRotationText(float value)
    {
        RotationText.text = $"Rotation (Euler) {value}";
    }

    private void UpdateScaleText(float value)
    {
        ScaleText.text = $"Scale {value}";
    }
    private void LoadSprites()
    {
        _sprites = Resources.LoadAll<Sprite>(PATH_SPRITES).ToList();
    }

    private void CreatePanelItems()
    {
        var scrollRect = ScrollView.GetComponent<ScrollRect>();
        var content = scrollRect.content;
        foreach (var sprite in _sprites)
        {
            GameObject cell = new GameObject();
            var image = cell.AddComponent<Image>();
            image.sprite = sprite;

            cell.transform.SetParent(content.transform, false);

            var width = sprite.rect.width;
            var height = sprite.rect.height;

            float scaleX = 1, scaleY = height / width;

            if (scaleY > 1)
            {
                scaleX /= scaleY;
                scaleY = 1;
            }

            cell.transform.localScale = new Vector3(scaleX, scaleY, 1);
            var panelItem = cell.AddComponent<PanelItem>();
            panelItem.SetItemsController(ItemsController);
            panelItem.SetSprite(sprite);
            panelItem.SetRectsUIElements(RectsUIElements);
        }
    }

    private void LoadSceneItems()
    {
        _serializer = new Serializer();
        var items = _serializer.Deserialize();

        if (items != null && items.Count > 0)
        {
            foreach (var item in items)
            {
                var sprite = _sprites.Find(x => x.name == item.SpriteName);
                if (sprite != null)
                {
                    var obj = new GameObject(sprite.name);

                    var spriteRenderer = obj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;

                    obj.AddComponent<PolygonCollider2D>();
                    var sceneItem = obj.AddComponent<SceneItem>();
                    obj.transform.position = new Vector3(item.Position.X, item.Position.Y, item.Position.Z);
                    obj.transform.rotation = Quaternion.Euler(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                    obj.transform.localScale = new Vector3(item.Scale.X, item.Scale.Y, item.Scale.Z);

                    ItemsController.AddItem(sceneItem);
                }
            }
        }
    }   

}
