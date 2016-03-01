using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BattleManager : MonoBehaviour
{
    public MasterManager masterManager;

    public Board board;
    public Player player;
    public Health health;
    public Timer timer;

    public ReactiveProperty<BattleStatus> status = new ReactiveProperty<BattleStatus>();

    void Awake()
    {
        status.Subscribe(x =>
        {
            Debug.Log("status: " + status.Value);
        });
    }

    void Start()
    {
        player.monsters[0].monsterMaster.Value = masterManager.monsterMasters[0];
        player.monsters[1].monsterMaster.Value = masterManager.monsterMasters[0];
        player.monsters[2].monsterMaster.Value = masterManager.monsterMasters[0];
        player.monsters[3].monsterMaster.Value = masterManager.monsterMasters[0];
        player.monsters[4].monsterMaster.Value = masterManager.monsterMasters[0];

        health.currentHealth.Value = health.maxHealth.Value;

        status.Value = BattleStatus.Idle;
    }
}
