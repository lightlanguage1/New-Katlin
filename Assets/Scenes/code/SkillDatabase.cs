using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class SkillDatabase : MonoBehaviour
{
    private const string databaseFileName = "Karryn"; // 数据库文件名
    private string databasePath; // 数据库文件完整路径
    private Dictionary<int, Skill> skillCache; // 技能缓存

    private void Awake()
    {
        // 组合数据库完整路径
        databasePath = Path.Combine(Application.streamingAssetsPath, databaseFileName);

        // 输出数据库路径进行调试
        Debug.Log($"数据库路径: {databasePath}");

        // 检查数据库文件是否存在
        if (File.Exists(databasePath))
        {
            Debug.Log("数据库文件存在");
        }
        else
        {
            Debug.LogError("数据库文件不存在");
            return; // 如果文件不存在，则不进行加载
        }

        // 加载所有技能数据并缓存
        LoadAllSkills();
    }

    // 获取技能缓存
    public Dictionary<int, Skill> getSkillCache()
    {
        return skillCache;
    }

    // 加载所有技能数据的方法
    private void LoadAllSkills()
    {
        skillCache = new Dictionary<int, Skill>(); // 初始化技能缓存字典

        try
        {
            using (SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}"))
            {
                connection.Open(); // 打开数据库连接

                using (SqliteCommand command = connection.CreateCommand()) // 创建 SQLite 命令对象
                {
                    // 查询数据库中的所有表
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        Debug.Log("数据库中的表:");
                        while (reader.Read())
                        {
                            Debug.Log(reader.GetString(0)); // 输出所有表的名称
                        }
                    }

                    // 查询 Skills 表数据
                    command.CommandText = "SELECT * FROM Skills"; // 设置命令文本为从 Skills 表中选择所有数据

                    using (SqliteDataReader reader = command.ExecuteReader()) // 执行命令并返回数据读取器
                    {
                        while (reader.Read())   // 循环读取每一行数据
                        {
                            Skill skill = new Skill(); // 创建 Skill 对象
                            skill.ID = reader.GetInt32(0);  // 设置技能 ID，0 代表第一个字段，以此类推
                            skill.skillName = reader.GetString(1); // 设置技能名称
                            skill.attack = reader.GetInt32(2); // 设置技能伤害值
                            skill.addSexVal = reader.GetInt32(3); // 设置技能增加性欲值
                            skill.reduceSexVal = reader.GetInt32(4); // 设置技能减少性欲值
                            skill.cooldown = reader.GetFloat(5); // 设置技能冷却时间
                            skill.hpCost = reader.GetInt32(6); // 设置技能消耗生命值
                            skill.mpCost = reader.GetInt32(7); // 设置技能消耗魔法值
                            skill.voliCost = reader.GetInt32(8); // 设置技能消耗意志值
                            skillCache.Add(skill.ID, skill); // 将技能添加到缓存字典中
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"数据库连接失败: {ex.Message}");
        }
    }
}
