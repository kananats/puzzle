using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Board : MonoBehaviour
{
    public BattleManager battleManager;

    public List<Rune> runes;
    private Queue<Rune> inactiveRunes = new Queue<Rune>();

    void Awake()
    {
        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Idle && Input.GetKeyDown(KeyCode.UpArrow))
            .Subscribe(_ =>
            {
                Move(Direction.Up);
                CreateRune();

                battleManager.status.Value = BattleStatus.Puzzle;
            });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Idle && Input.GetKeyDown(KeyCode.DownArrow))
            .Subscribe(_ =>
            {
                Move(Direction.Down);
                CreateRune();

                battleManager.status.Value = BattleStatus.Puzzle;
            });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Idle && Input.GetKeyDown(KeyCode.LeftArrow))
            .Subscribe(_ =>
            {
                Move(Direction.Left);
                CreateRune();

                battleManager.status.Value = BattleStatus.Puzzle;
            });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Idle && Input.GetKeyDown(KeyCode.RightArrow))
            .Subscribe(_ =>
            {
                Move(Direction.Right);
                CreateRune();

                battleManager.status.Value = BattleStatus.Puzzle;
            });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Puzzle && Input.GetKeyDown(KeyCode.UpArrow))
            .Subscribe(_ =>
        {
            Move(Direction.Up);
            CreateRune();
        });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Puzzle && Input.GetKeyDown(KeyCode.DownArrow))
            .Subscribe(_ =>
        {
            Move(Direction.Down);
            CreateRune();
        });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Puzzle && Input.GetKeyDown(KeyCode.LeftArrow))
            .Subscribe(_ =>
        {
            Move(Direction.Left);
            CreateRune();
        });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Puzzle && Input.GetKeyDown(KeyCode.RightArrow))
            .Subscribe(_ =>
        {
            Move(Direction.Right);
            CreateRune();
        });

        this.UpdateAsObservable()
            .Where(_ => battleManager.status.Value == BattleStatus.Battle && runes.Count(x => x.level.Value >= 1) > 0)
            .Sample(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ =>
        {
            runes.First(x => x.level.Value >= 1).active.Value = false;
            Debug.Log("remove1");
        });
    }

    void Start()
    {
        Rune runePrefab = Resources.Load<Rune>("Rune");

        for (int i = 0; i < 25; i++)
        {
            Rune rune = Instantiate(runePrefab);

            rune.active.Where(x => x).Subscribe(_ =>
            {
                runes.Add(rune);
            });

            rune.active.Where(x => !x).Subscribe(_ =>
            {
                if (runes.Contains(rune))
                    runes.Remove(rune);

                inactiveRunes.Enqueue(rune);
            });

            rune.active.Value = false;
            rune.transform.SetParent(transform, false);
        }

        for (int i = 0; i < 25; i++)
            CreateRune();
    }

    private void CreateRune()
    {
        if (inactiveRunes.Count == 0)
            return;

        Rune rune = inactiveRunes.Dequeue();

        rune.active.Value = true;
        rune.level.Value = 1;

        List<Tuple<int, int>> points = new List<Tuple<int, int>>();
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                points.Add(new Tuple<int, int>(i, j));

        runes.ToObservable().Subscribe(x =>
        {
            points.Remove(new Tuple<int, int>(x.column.Value, x.row.Value));
        });

        Tuple<int, int> point = points[UnityEngine.Random.Range(0, points.Count)];

        rune.column.Value = point.Item1;
        rune.row.Value = point.Item2;
    }

    private void MoveUp()
    {
        List<Rune>[] columns = new List<Rune>[5];

        for (int i = 0; i < columns.Length; i++)
            columns[i] = new List<Rune>();

        runes.ToObservable()
            .Subscribe(x =>
        {
            columns[x.column.Value].Add(x);
        });

        for (int i = 0; i < columns.Length; i++)
            columns[i].Sort((x, y) => -x.row.Value.CompareTo(y.row.Value));

        for (int i = 0; i < columns.Length; i++)
        {
            Rune lastRune = null;
            int index = 4;

            columns[i].ToObservable().Subscribe(x =>
            {
                if (lastRune == null || lastRune.attribute.Value != x.attribute.Value)
                {
                    x.row.Value = index--;
                    lastRune = x;
                }

                else if (lastRune.attribute.Value == x.attribute.Value)
                {
                    lastRune.level.Value += x.level.Value;
                    x.active.Value = false;
                }
            });
        }
    }

    private void MoveDown()
    {
        List<Rune>[] columns = new List<Rune>[5];

        for (int i = 0; i < columns.Length; i++)
            columns[i] = new List<Rune>();

        runes.ToObservable()
            .Subscribe(x =>
            {
                columns[x.column.Value].Add(x);
            });

        for (int i = 0; i < columns.Length; i++)
            columns[i].Sort((x, y) => x.row.Value.CompareTo(y.row.Value));

        for (int i = 0; i < columns.Length; i++)
        {
            Rune lastRune = null;
            int index = 0;

            columns[i].ToObservable().Subscribe(x =>
            {
                if (lastRune == null || lastRune.attribute.Value != x.attribute.Value)
                {
                    x.row.Value = index++;
                    lastRune = x;
                }

                else if (lastRune.attribute.Value == x.attribute.Value)
                {
                    lastRune.level.Value += x.level.Value;
                    x.active.Value = false;
                }
            });
        }
    }

    private void MoveLeft()
    {
        List<Rune>[] rows = new List<Rune>[5];

        for (int i = 0; i < rows.Length; i++)
            rows[i] = new List<Rune>();

        runes.ToObservable()
            .Subscribe(x =>
        {
            rows[x.row.Value].Add(x);
        });

        for (int i = 0; i < rows.Length; i++)
            rows[i].Sort((x, y) => x.column.Value.CompareTo(y.column.Value));

        for (int i = 0; i < rows.Length; i++)
        {
            Rune lastRune = null;
            int index = 0;

            rows[i].ToObservable().Subscribe(x =>
            {
                if (lastRune == null || lastRune.attribute.Value != x.attribute.Value)
                {
                    x.column.Value = index++;
                    lastRune = x;
                }

                else if (lastRune.attribute.Value == x.attribute.Value)
                {
                    lastRune.level.Value += x.level.Value;
                    x.active.Value = false;
                }
            });
        }
    }

    private void MoveRight()
    {
        List<Rune>[] rows = new List<Rune>[5];

        for (int i = 0; i < rows.Length; i++)
            rows[i] = new List<Rune>();

        runes.ToObservable()
            .Subscribe(x =>
        {
            rows[x.row.Value].Add(x);
        });

        for (int i = 0; i < rows.Length; i++)
            rows[i].Sort((x, y) => -x.column.Value.CompareTo(y.column.Value));

        for (int i = 0; i < rows.Length; i++)
        {
            Rune lastRune = null;
            int index = 4;

            rows[i].ToObservable().Subscribe(x =>
            {
                if (lastRune == null || lastRune.attribute.Value != x.attribute.Value)
                {
                    x.column.Value = index--;
                    lastRune = x;
                }

                else if (lastRune.attribute.Value == x.attribute.Value)
                {
                    lastRune.level.Value += x.level.Value;
                    x.active.Value = false;
                }
            });
        }
    }

    private void Move(Direction direction)
    {
        List<Rune>[] temp = new List<Rune>[5];

        for (int i = 0; i < temp.Length; i++)
            temp[i] = new List<Rune>();

        runes.ToObservable()
            .Subscribe(x =>
        {
            temp[direction.IsUpOrDown() ? x.column.Value : x.row.Value].Add(x);
        });

        for (int i = 0; i < temp.Length; i++)
            temp[i].Sort((x, y) => (direction.IsUpOrRight() ? -1 : 1) * (direction.IsUpOrDown() ? x.row.Value.CompareTo(y.row.Value) : x.column.Value.CompareTo(y.column.Value)));

        for (int i = 0; i < temp.Length; i++)
        {
            Rune lastRune = null;
            int index = direction.IsUpOrRight() ? 4 : 0;

            temp[i].ToObservable().Subscribe(x =>
            {
                if (lastRune == null || lastRune.attribute.Value != x.attribute.Value)
                {
                    x.column.Value = direction.IsUpOrDown() ? x.column.Value : (direction.IsUpOrRight() ? index-- : index++);
                    x.row.Value = direction.IsUpOrDown() ? (direction.IsUpOrRight() ? index-- : index++) : x.row.Value;

                    lastRune = x;
                }

                else if (lastRune.attribute.Value == x.attribute.Value)
                {
                    lastRune.level.Value += x.level.Value;
                    x.active.Value = false;
                }
            });
        }
    }
}
