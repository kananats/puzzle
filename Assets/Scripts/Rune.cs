using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Rune : MonoBehaviour
{
    public Text text;
    public Image image;

    public RectTransform rectTransform;

    public ReactiveProperty<bool> active = new ReactiveProperty<bool>();

    public ReactiveProperty<Attribute> attribute = new ReactiveProperty<Attribute>();
    public ReactiveProperty<int> level = new ReactiveProperty<int>();

    public ReactiveProperty<int> column = new ReactiveProperty<int>();
    public ReactiveProperty<int> row = new ReactiveProperty<int>();

    void Awake()
    {
        active
            .Where(x => x == true)
            .Subscribe(_ =>
        {
            attribute.Value = (Attribute)UnityEngine.Random.Range(0, 3);

            gameObject.SetActive(true);
        });

        active
            .Where(x => x == false)
            .Subscribe(_ =>
        {
            gameObject.SetActive(false);
        });

        attribute
            .Where(x => x == Attribute.Water)
            .Subscribe(x =>
        {
            image.color = Color.blue;
        });

        attribute
            .Where(x => x == Attribute.Fire)
            .Subscribe(x =>
        {
            image.color = Color.red;
        });

        attribute
            .Where(x => x == Attribute.Earth)
            .Subscribe(x =>
        {
            image.color = Color.green;
        });

        level
            .Subscribe(x =>
        {
            text.text = "" + level;
        });

        column
            .Merge(row)
            .Where(_ => active.Value)
            .Subscribe(_ =>
        {
            UpdatePosition();
        });
    }
    
    private void UpdatePosition()
    {
        rectTransform.anchorMin = new Vector2(0.2f * column.Value, 0.2f * row.Value);
        rectTransform.anchorMax = new Vector2(0.2f * column.Value + 0.2f, 0.2f * row.Value + 0.2f);
    }
}
