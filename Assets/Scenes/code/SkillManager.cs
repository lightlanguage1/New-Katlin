using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    private Dictionary<int, Skill> skillCache; // 技能缓存
    [SerializeField] private List<string> skillList; // 技能列表

    private void Start()
    {
        // 获取 SkillDatabase 实例并确保其在当前脚本之前调用
        SkillDatabase skillDatabase = GetComponent<SkillDatabase>();
        if (skillDatabase == null)
        {
            Debug.LogError("SkillDatabase 组件未找到！");
            return;
        }

        skillCache = skillDatabase.getSkillCache();

        // 确认技能缓存是否成功加载
        if (skillCache == null || skillCache.Count == 0)
        {
            Debug.LogError("技能缓存为空或未初始化！");
            return;
        }

        /*Debug.Log("技能缓存加载成功，技能数量: " + skillCache.Count);*/

/*#if UNITY_EDITOR
        // 将技能列表更新为从缓存中读取的技能名称
        foreach (KeyValuePair<int, Skill> skill in skillCache)
        {
            skillList.Add(skill.Value.skillName);
            Debug.Log($"技能名称: {skill.Value.skillName}, ID: {skill.Value.ID}");
        }
#endif*/
    }

    public Dictionary<int, Skill> getSkillCache()
    {
        if (skillCache == null)
        {
            Debug.LogError("技能缓存未初始化！");
        }
        return skillCache;
    }



/*    public Skill GetSkillByID(int skillID)
    {
        if (skillCache.TryGetValue(skillID, out Skill skill))   //out 输出参数 表示该变量在函数内部被赋值
        {
            return skill;
        }
        else
        {
            Debug.LogWarning("技能ID " + skillID + " 不存在于缓存中！");
            return null;
        }
    }

    public bool UseCachedSkill()
    {
        // 使用技能
        Debug.Log("使用技能："  );
        // 在这里执行技能的逻辑

        return true; // 使用技能成功
    }*/
}