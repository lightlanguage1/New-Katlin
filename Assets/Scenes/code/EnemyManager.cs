using System.Collections;
using UnityEngine;

public class EnemyManager : Character
{
    [SerializeField] private int enemyIndex;    //判断是哪个敌人
    public int EnemyIndex => enemyIndex;

    private void Awake()
    {
        //这里的名字和预制体的保持一致，去看预制体吧
        if (gameObject.name == "Enemy1 guard")
        {
            enemyIndex = (int)Global.enemyName.Guard;
        }
        else if (gameObject.name == "Enemy2 goblin")
        {
            enemyIndex = (int)Global.enemyName.Goblin;
        }
        else if (gameObject.name == "Enemy3 homeless")
        {
            enemyIndex = (int)Global.enemyName.Homeless;
        }
        else if (gameObject.name == "Enemy4 lizardman")
        {
            enemyIndex = (int)Global.enemyName.Lizardman;
        }
        else if (gameObject.name == "Enemy5 nerd")
        {
            enemyIndex = (int)Global.enemyName.Nerd;
        }
        else if (gameObject.name == "Enemy6 orc")
        {
            enemyIndex = (int)Global.enemyName.Orc;
        }
        else if (gameObject.name == "Enemy7 prisoner")
        {
            enemyIndex = (int)Global.enemyName.Prisoner;
        }
        else if (gameObject.name == "Enemy8 rogue")
        {
            enemyIndex = (int)Global.enemyName.Rogue;
        }
        else if (gameObject.name == "Enemy9 slime")
        {
            enemyIndex = (int)Global.enemyName.Slime;
        }
        else if (gameObject.name == "Enemy10 thug")
        {
            enemyIndex = (int)Global.enemyName.Thug;
        }
        else if (gameObject.name == "Enemy11 wolf")
        {
            enemyIndex = (int)Global.enemyName.Wolf;
        }
        else if (gameObject.name == "Enemy12 yeti")
        {
            enemyIndex = (int)Global.enemyName.Yeti;
        }
        else if (gameObject.name == "Enemy BOOS1L")
        {
            enemyIndex = (int)Global.enemyName.Boss1L;
        }
    }

    public void initialize(float hp_, float mp_, float maxhp_, float maxmp_,float atk_,float def_,float sex_)
    {
        hp = hp_;
        mp = mp_;
        maxhp = maxhp_;
        maxmp = maxmp_;
        atk = atk_;
        def = def_;
        sex = sex_;
        initHpAndMp(hp, mp, maxhp, maxmp);
    }

    public override void takeDamage(float damage)
    {
        base.takeDamage(damage);
        
    }

    public float getAtk()
    {
        return atk;
    }

    public float getSexVal()
    {
        return sex;
    }
    
}