using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 枚举定义了技能类型
public enum SkillType
{
    daji = 1,            // 打击技
    xuli,                // 蓄力技
    mishujiashi,         // 密术驾驶
    kanpojiashi,         // 看破驾驶
    roubangguancha,       // 肉榜观察
    zhanj,
    ciji,
    sunpi,
    liuxingji,
    tuibuciji,
    qingti,
    toubudaji,
    baodanfeiti,
    wanbupikan,
    kanpoliaoyu,
    liaoyu,
    fanshoujiashi,
    fanshouliaoyu,
    fanshouliyi,
    fanjijiashi,
    fanjiliaoyu,
    fanjiliyi,
    tiaoxin,
    shaoshounongzhi,
    zhenliyifu,
    zhuanzu,
    yazhikouyu,
    yazhiruyu,
    yazhiqianyu,
    yazhibanyu,
    yazhitunyu,
    kouyuwangseng,
    qiangyuwangseng,
    ruyuwangseng,
    tunyuwangseng,
    bangyuwangseng,
    guyoujieji,
    zhiyusixu,
    xinglimangdian,
    shunvdezhitai,
    kuailedezhishi1,
    kuailedezhishi3,
    kuailedezhishi5,
    kuailedezhishi10,
    fusuzhifen,
    touxian,
    roubanguangcha,
    qinwen,
    aifurouban,
    shoujiao,
    koujiao,
    rujiao,
    xinjiao,
    ganjiao,
    zujiao
}

// 定义了技能命令接口
public interface ISkillCommand
{
    void Execute(); // 执行命令的方法
}

// 蓄力技命令类实现了 ISkillCommand 接口
public class XuliSkillCommand : ISkillCommand
{
    public void Execute()
    {
        // 增加主角攻击力，播放蓄力动画

        var battleManager = BattleManager.instance;
        var karryn = battleManager.canvas.transform.GetChild((int)ChildrenType.Karryn).gameObject;

        // 播放技能特效动画
        battleManager.canvas.transform.GetChild((int)ChildrenType.SkillEffect).gameObject.SetActive(true);
        Animator animator = battleManager.canvas.transform.GetChild((int)ChildrenType.SkillEffect).GetComponent<Animator>();
        animator.Play("xuli");

        var player = PlayerManager.instance;
        player.atk += 50 * player.level; // 增加攻击力
        // 修改 UI 显示的攻击力
        GameObject infoPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject;
        GameObject targetObj = infoPanel.transform.GetChild(0).GetChild(0).GetChild(14).gameObject;
        //TextMeshProUGUI textPro = targetObj.GetComponent<TextMeshProUGUI>();
        //textPro.text = player.atk.ToString(); // 更新攻击力文本
        // 切换卡琳的状态图
        battleManager.childrenDic["Karryn"][(int)battleManager.playerCurState].gameObject.SetActive(false);
        battleManager.playerCurState = KarrynState.Weapon2;
        battleManager.childrenDic["Karryn"][(int)battleManager.playerCurState].gameObject.SetActive(true);
        // 关闭精神面板，启用技能轮盘
        battleManager.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject.SetActive(false);
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].StopPlayback();
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.curPanel.SetActive(true);
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0随便取的
        battleManager.isPanel = false;
        PlayerManager.instance.subtractVolition(Global.instance.xuli);    //50
        battleManager.isInputEnable = true;
    }
}

// 打击技命令类实现了 ISkillCommand 接口
public class dajiButtonCommand : ISkillCommand
{
    public void Execute()
    {
        this.closeSkillPannel(ref BattleManager.instance.curPanel);
        showSelectFrame();
    }

    // 显示选中框
    public void showSelectFrame()
    {
        GameObject frame = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
        GameObject cursor = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
        frame.SetActive(true);
        cursor.SetActive(true);
        // 先停止 Fixed Update 内的 checkUserMoveAction()，然后再启动协程执行选中敌人攻击函数
        BattleManager.instance.isPanel = false;
        BattleManager.instance.isInputEnable = true;
        BattleManager.instance.coroutineSeletedEnemy = BattleManager.instance.
        StartCoroutine(BattleManager.instance.beginSelectEnemyAtk(BattleManager.instance.prefabs, SkillType.daji));
    }

    // 关闭技能面板
    public void closeSkillPannel(ref GameObject curPanel_)
    {
        BattleManager battleManager = BattleManager.instance;
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].StopPlayback();
        curPanel_.SetActive(false);
    }
}

// 技能命令管理器类
public class SkillCommandManager
{
    private Dictionary<string, ISkillCommand> skillCommandDictionary; // 技能命令字典
    private List<Button> buttons; // 按钮列表

    // 构造函数，初始化技能命令字典和按钮列表
    public SkillCommandManager(Button[] buttons)
    {
        skillCommandDictionary = new Dictionary<string, ISkillCommand>();
        this.buttons = new List<Button>(buttons);

        // 初始化并填充技能命令字典
        PopulateSkillCommandDictionary();
    }

    // 填充技能命令字典
    private void PopulateSkillCommandDictionary()
    {
        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
        {
            string buttonName = skillType.ToString().ToLower();
            ISkillCommand skillCommand = CreateSkillCommand(skillType);
            if (skillCommand != null)
            {
                Debug.Log($"添加按钮: {buttonName}");
                skillCommandDictionary.Add(buttonName, skillCommand);
            }
            else
            {
                Debug.LogWarning($"未找到技能 {skillType} 对应的命令");
            }
        }
    }

    // 根据按钮名称获取技能命令
    public ISkillCommand GetSkillCommand(string buttonName)
    {
        string formattedButtonName = buttonName.ToLower(); // 将传入的按钮名称转换为小写
        if (skillCommandDictionary.ContainsKey(formattedButtonName))
        {
            return skillCommandDictionary[formattedButtonName];
        }
        else
        {
            // 输出调试信息，显示传入的按钮名称
            Debug.Log($"传入的按钮名称: {buttonName}");

            // 输出技能命令字典中的所有键，以检查是否有问题
            Debug.Log("当前技能命令字典内的按钮键：");
            foreach (var key in skillCommandDictionary.Keys)
            {
                Debug.Log(key);
            }

            // 输出调试信息，显示未找到按钮对应的技能命令
            Debug.LogWarning($"未找到按钮 {buttonName} 对应的技能命令");
            return null;
        }
    }


    // 执行指定按钮名称对应的技能命令
    public bool ExecuteSkill(string buttonName)
    {
        // 将传入的按钮名称转换为小写
        string formattedButtonName = buttonName.ToLower();

        // 调用 GetSkillCommand 方法，传入转换后的按钮名称
        ISkillCommand skillCommand = GetSkillCommand(formattedButtonName);
        if (skillCommand != null)
        {
            skillCommand.Execute();
            return true;
        }
        else
        {
            return false;
        }
    }

    // 根据技能类型获取技能命令
    private ISkillCommand CreateSkillCommand(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.xuli:
                return new XuliSkillCommand();
            case SkillType.daji:
                return new dajiButtonCommand();
            default:
                return null;
        }
    }

}

