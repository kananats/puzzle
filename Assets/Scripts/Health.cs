using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Health : MonoBehaviour
{
    public BattleManager battleManager;

    public Text text;

    public ReactiveProperty<int> currentHealth = new ReactiveProperty<int>();
    public ReactiveProperty<int> maxHealth = new ReactiveProperty<int>();

    void Awake()
    {
        currentHealth
            .Merge(maxHealth)
            .Subscribe(_ =>
        {
            text.text = currentHealth.Value + "/" + maxHealth.Value;
        });

        battleManager.player.monsters[0].monsterMaster
            .Merge(battleManager.player.monsters[1].monsterMaster)
            .Merge(battleManager.player.monsters[2].monsterMaster)
            .Merge(battleManager.player.monsters[3].monsterMaster)
            .Merge(battleManager.player.monsters[4].monsterMaster)
            .Subscribe(_ =>
        {
            maxHealth.Value = battleManager.player.monsters
                .Sum(x => x.monsterMaster.Value.Health);
        });

        battleManager.status
            .Where(x => x == BattleStatus.Idle || x == BattleStatus.Battle)
            .Subscribe(_ =>
        {
            text.gameObject.SetActive(true);
        });

        battleManager.status
            .Where(x => x == BattleStatus.Puzzle)
            .Subscribe(_ =>
        {
            text.gameObject.SetActive(false);
        });
    }
}
