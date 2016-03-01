using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Monster : MonoBehaviour
{
    public Player player;

    public Button button;

    public ReactiveProperty<MonsterMaster> monsterMaster = new ReactiveProperty<MonsterMaster>();

    //Attribute
    //Leader Skill
    //Active Skill
}
