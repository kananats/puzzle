using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Timer : MonoBehaviour
{
    public BattleManager battleManager;

    public Text text;

    public ReactiveProperty<float> CurrentTime = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxTime = new ReactiveProperty<float>();

    void Awake()
    {
        CurrentTime
            .Subscribe(_ =>
        {
            text.text = "" + CurrentTime.Value.ToString("0.0");
        });

        CurrentTime
            .Where(x => battleManager.status.Value == BattleStatus.Puzzle && x <= 0)
            .Subscribe(_ =>
        {
            battleManager.status.Value = BattleStatus.Battle;
        });

        battleManager.status
            .Where(x => x == BattleStatus.Idle)
            .Subscribe(_ =>
        {
            text.gameObject.SetActive(false);

            CurrentTime.Value = MaxTime.Value;
        });

        battleManager.status
            .Where(x => x == BattleStatus.Puzzle)
            .Subscribe(_ =>
        {
            CurrentTime.Value = MaxTime.Value;

            text.gameObject.SetActive(true);
        });

        battleManager.status
            .Where(x => x == BattleStatus.Battle)
            .Subscribe(_ =>
        {
            gameObject.SetActive(false);
        });

        this.UpdateAsObservable()
            
            .Where(_ => battleManager.status.Value == BattleStatus.Puzzle)
            .Sample(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
        {
            CurrentTime.Value -= 0.1f;
        });
    }

    void Start()
    {
        MaxTime.Value = 5.0f;
    }
}
