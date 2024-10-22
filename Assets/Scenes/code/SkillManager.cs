using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    private Dictionary<int, Skill> skillCache; // ���ܻ���
    [SerializeField] private List<string> skillList; // �����б�

    private void Start()
    {
        // ��ȡ SkillDatabase ʵ����ȷ�����ڵ�ǰ�ű�֮ǰ����
        SkillDatabase skillDatabase = GetComponent<SkillDatabase>();
        if (skillDatabase == null)
        {
            Debug.LogError("SkillDatabase ���δ�ҵ���");
            return;
        }

        skillCache = skillDatabase.getSkillCache();

        // ȷ�ϼ��ܻ����Ƿ�ɹ�����
        if (skillCache == null || skillCache.Count == 0)
        {
            Debug.LogError("���ܻ���Ϊ�ջ�δ��ʼ����");
            return;
        }

        /*Debug.Log("���ܻ�����سɹ�����������: " + skillCache.Count);*/

/*#if UNITY_EDITOR
        // �������б����Ϊ�ӻ����ж�ȡ�ļ�������
        foreach (KeyValuePair<int, Skill> skill in skillCache)
        {
            skillList.Add(skill.Value.skillName);
            Debug.Log($"��������: {skill.Value.skillName}, ID: {skill.Value.ID}");
        }
#endif*/
    }

    public Dictionary<int, Skill> getSkillCache()
    {
        if (skillCache == null)
        {
            Debug.LogError("���ܻ���δ��ʼ����");
        }
        return skillCache;
    }



/*    public Skill GetSkillByID(int skillID)
    {
        if (skillCache.TryGetValue(skillID, out Skill skill))   //out ������� ��ʾ�ñ����ں����ڲ�����ֵ
        {
            return skill;
        }
        else
        {
            Debug.LogWarning("����ID " + skillID + " �������ڻ����У�");
            return null;
        }
    }

    public bool UseCachedSkill()
    {
        // ʹ�ü���
        Debug.Log("ʹ�ü��ܣ�"  );
        // ������ִ�м��ܵ��߼�

        return true; // ʹ�ü��ܳɹ�
    }*/
}