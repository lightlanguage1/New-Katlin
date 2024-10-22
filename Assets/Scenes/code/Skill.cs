using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill 
{
    public int ID; // 技能ID

    public string skillName; // 技能名称
    public int attack; // 技能造成的伤害
    public int addSexVal;       
    public int reduceSexVal;    //减少淫乱值
    public float cooldown; // 技能冷却时间
    public int hpCost;  //攻击消耗自身hp
    public int mpCost;  //精力值消耗
    public int voliCost;    //意志消耗
    
}
