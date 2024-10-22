using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//通过枚举值增加可读性
public enum ChildrenType   //子对象类型 （表示是哪个子对象）
{
    BG = 0,
    EnemyBar,
    Karryn,
    SkillEffect,
    BattleSkillPanel1,
    KarrynPropertyBar,
    Numbers,
    BattleSkillPanel2,
    VolitionPanel,
    SpiritualPanel1,
    SpiritualPanel2,
    AtkSkillPanel,
    SexSkillPanel,
    InfoPanel,
    SelectFrame,
    FrameCursor,
    PostProcess,
}
public enum PanelType   //注意和层级窗口中的子对象索引保持一致
{
    Karryn = 2,
    SkillEffect,
    BattleSkillPanel1 = 4,
    KarrynPropertyBar,
    Numbers,
    BattleSkillPanel2,
    VolitionPanel,
    SpiritualPanel1,
    SpiritualPanel2,
    SexPanel,
    AtkSkillPanel,
    InfoPanel
}

public enum panelInChildType
{
    Cursor = 0,
    Focus
}

public enum KarrynState
{
    Weapon1 = 0,
    Weapon2,
    Atk1
}

public interface Icommand
{
    void execute(); //定义接口

    //接口类内定义的函数必须在派生类中全部实现
}

//类的名字自己看着起
public class SwitchPannel2Command : Icommand
{
    public void execute()
    {
        //使用GetChild 可以获取启用or禁用的子对象
        BattleManager battleManager = BattleManager.instance;

        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject.SetActive(false);
        //停止缩放和旋转的协程,注意此时按钮还在缩放，所以为了保证按钮大小一致，要先调整他的缩放大小
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel1[0].transform.localScale = Vector3.one; //还原

        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.curPanel.SetActive(true);
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0随便取的
        
    }
}

public class switchAttackSkillPanelCommand : Icommand  //攻击面板
{
    public void execute()
    {
        //停止该协程防止继续移动选中框
        BattleManager.instance.StopCoroutine(BattleManager.instance.coroutineSeletedEnemy);
        openSkillPanel(ChildrenType.AtkSkillPanel);
    }

    public void openSkillPanel(ChildrenType type)
    {
        BattleManager battleManager = BattleManager.instance;
        GameObject frame = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
        GameObject cursor = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
        frame.SetActive(false);
        cursor.SetActive(false);
        battleManager.canvas.transform.GetChild((int)type).gameObject.SetActive(true);

        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].Play("focus");
        battleManager.isPanel = true;
        battleManager.ResetCursorAndFocus();
    }
}

public class attackButtonCommand : Icommand //攻击按钮
{
    public void execute()
    {
        BattleManager battleManager = BattleManager.instance;

        //停止缩放和旋转的协程,注意此时按钮还在缩放，所以为了保证按钮大小一致，要先调整他的缩放大小
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel2[0].transform.localScale = Vector3.one; //还原

        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.AtkSkillPanel).gameObject;
        battleManager.curPanel.SetActive(true);

        //播放  focus 动画
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;

        battleManager.animDic[foucsName].Play("focus");

        battleManager.isPanel = true;
    }
}

public class rollBackBattleSkillPanel2 : Icommand
{
    public void execute()
    {
        BattleManager battleManager = BattleManager.instance;
        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(true);

        battleManager.curPanel.SetActive(false);
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        if (battleManager.animDic.ContainsKey(foucsName))
        {
            battleManager.animDic[foucsName].StopPlayback();
        }
        //因为有多个子面板所以回退时要逐个判断
        if (battleManager.curPanel.name == "Info Panel")
        {
            battleManager.StopCoroutine(battleManager.corStartInfoPanel);
        }
        battleManager.isPanel = false;
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0随便取的
        BattleManager.instance.isInputEnable = true;
    }
}

public class rollBackBattleSkillPanel1 : Icommand
{
    public void execute()
    {
        BattleManager battleManager = BattleManager.instance;

        if (battleManager.corStartInfoPanel != null)
        {
            battleManager.StopCoroutine(battleManager.corStartInfoPanel);
            battleManager.corStartInfoPanel = null;
        }
        battleManager.curPanel.SetActive(false);
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject;
    }
}

public class switchInfoPanel : Icommand
{
    public void execute()
    {
        //更新当前面板
        var battleManager = BattleManager.instance;
        BattleManager.instance.curPanel = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject;
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject.SetActive(true);
        BattleManager.instance.corStartInfoPanel = BattleManager.instance.StartCoroutine(BattleManager.instance.startInfoPanel());
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel2[0].transform.localScale = Vector3.one; //还原
    }
}

public class switchVolitionPanel : Icommand
{
    public void execute()
    {
        var battleManager = BattleManager.instance;
        BattleManager.instance.curPanel = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject;
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject.SetActive(false);
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject.SetActive(true);

        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel1[0].transform.localScale = Vector3.one; //还原

        //播放  focus 动画
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;

        battleManager.animDic[foucsName].Play("focus");

        battleManager.isPanel = true;
    }
}

public class specialsexskillButtonCommand : Icommand//新加的面板逻辑未完成，面板内按钮交互逻辑
{
    public void execute()
    {
        // 隐藏其他面板，显示 SpiritualSkillPanel2
        BattleManager battleManager = BattleManager.instance; ;

        // 停止一些协程
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);

        // 还原按钮大小
        battleManager.mainPanel2[0].transform.localScale = Vector3.one;

        // 隐藏其他面板
        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);

        // 显示SpiritualPanel2面板
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.SpiritualPanel2).gameObject;
        battleManager.curPanel.SetActive(true);

        // 播放 "focus" 动画
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        Debug.Log(foucsName);
        // Debug.Log(battleManager.animDic);
        battleManager.animDic[foucsName].Play("focus");

        // 设置 isPanel 为 true
        battleManager.isPanel = true;
        battleManager.ResetCursorAndFocus();
    }
}


public class SexSkillPanelCommand : Icommand//新加的面板逻辑未完成，面板内按钮交互逻辑
{
    public void execute()
    {
        // 隐藏其他面板，显示 SpiritualSkillPanel2
        BattleManager battleManager = BattleManager.instance; ;

        // 停止一些协程
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);

        // 还原按钮大小
        battleManager.mainPanel2[0].transform.localScale = Vector3.one;

        // 隐藏其他面板
        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);

        // 显示SexSkillPanelCommand面板
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.SexSkillPanel).gameObject;
        battleManager.curPanel.SetActive(true);

        // 播放 "focus" 动画
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;


        battleManager.animDic[foucsName].Play("focus");

        // 设置 isPanel 为 true
        battleManager.isPanel = true;

        battleManager.ResetCursorAndFocus();


    }
}


public class ButtonCommandManager //面板交互核心，视情况进行优化
{
    private Dictionary<string, Icommand> commandDictionary;

    private List<Button> curButtonList;     //按钮的列表

    private List<GameObject> panelList;

    public BattleManager battleManager;
    public ButtonCommandManager(Button[] buttons)
    {
        // 初始化字典、列表等数据结构
        commandDictionary = new Dictionary<string, Icommand>();
        curButtonList = new List<Button>();
        panelList = new List<GameObject>();
        curButtonList.AddRange(buttons);


        if (curButtonList.Count > 0)
        {
            BattleManager battleManager = BattleManager.instance;
            Transform canvasTransform = battleManager.canvas.transform;
            battleManager.curPanel = canvasTransform.Find("Battle Skill Panel 1").gameObject;

            // 获取所有面板的类型枚举值
            var panelTypes = Enum.GetValues(typeof(PanelType)).Cast<PanelType>();
            foreach (var panelType in panelTypes)
            {
                Transform panelTransform = canvasTransform.GetChild((int)panelType);
                GameObject panelObject = panelTransform.gameObject;

                if (panelObject.name == "Pool Manager")
                {
                    continue; // 跳过特定面板
                }

                List<Transform> children = new List<Transform>();
                List<Transform> scrollViewDescChildren = new List<Transform>();

                if (panelObject.name == "Volition Panel" || panelObject.name == "Spiritual  Panel 1" || panelObject.name == "Spiritual Panel 2")
                {
                    // 针对特定面板类型处理子对象
                    Transform scrollView = panelTransform.Find("Scroll View");
                    Transform content = scrollView.Find("Viewport").Find("Content");

                    foreach (Transform child in content)
                    {
                        children.Add(child);
                    }

                    scrollViewDescChildren.AddRange(content.Cast<Transform>());
                }
                else if (panelObject.name == "Karryn")
                {
                    // 针对特定面板类型处理子对象
                    foreach (Transform child in panelTransform)
                    {
                        children.Add(child);
                    }
                }
                else
                {
                    // 针对其他面板类型处理子对象
                    foreach (Transform child in panelTransform)
                    {
                        if (!ShouldSkipChild(panelTransform, child))
                        {
                            children.Add(child);
                        }
                    }
                }
                // 将面板的子对象添加到字典
                var dict = battleManager.childrenDic;
                //var list = new List<Dictionary<string, List<Transform>>>();
                var list = new List<string>
                {
                    panelObject.name
                };
                //battleManager.childrenDic.Add(panelObject.name, children);

                foreach (var name in list)
                {
                    // 如果字典不包含该键
                    if (!dict.ContainsKey(name))
                    {
                        // 字典为空则直接添加
                        if (!dict.Any())
                        {
                            dict.Add(name, children);
                        }
                        // 字典不为空也添加
                        else
                        {
                            dict.Add(name, children);
                        }
                    }
                    else if (dict[name].Count == 0)
                    {
                        dict[name] = children;
                    }

                }

                // 如果存在滚动视图描述子对象，添加到列表中
                if (scrollViewDescChildren.Any())
                {
                    battleManager.scrollViewDescList.AddRange(scrollViewDescChildren);
                }

                panelList.Add(panelObject);
            }

            // 填充命令字典
            PopulateCommandDictionary();
        }
        else
        {
            //Debug.LogError("curButtonList 添加了空的按钮数组，请检查");
        }
    }

    // 判断是否跳过某个子对象的条件
    private bool ShouldSkipChild(Transform panelTransform, Transform child)
    {
        return child.name == panelTransform.GetChild((int)panelInChildType.Cursor).name
            || child.name == panelTransform.GetChild((int)panelInChildType.Focus).name
            || child.name == "description";
    }

    // 填充命令字典
    private void PopulateCommandDictionary()
    {
        foreach (var button in curButtonList)
        {
            Icommand icommand = getCommandObj(button.gameObject.name);
            if (icommand == null)
            {
                //Debug.Log("没有这样的按钮！,请检查按钮的种类");
            }
            commandDictionary.Add(button.gameObject.name, icommand);
        }

        foreach (var panel in panelList)
        {
            Icommand icommand = getCommandObj(panel.gameObject.name);
            if (icommand == null)
            {
                //Debug.LogWarning("没有这样的面板！,请检查按钮的种类");
            }
            commandDictionary.Add(panel.gameObject.name, icommand);
        }
    }


    private Icommand getCommandObj(string buttonName)   //buttonName叫对象名更合适偷懒就不改了~
    {
        switch (buttonName)
        {
            //名字去层级窗口找，部分用拼音命名
            case "info":
                return new switchInfoPanel();

            case "volition skill":
                return new switchVolitionPanel();

            case "switch panel2":
                return new SwitchPannel2Command();

            case "Battle Skill Panel 1":
                return new rollBackBattleSkillPanel1();

            case "Battle Skill Panel 2":
                return new rollBackBattleSkillPanel2();

            case "Atk Skill Panel":
                return new switchAttackSkillPanelCommand();

            case "atk":
               return new attackButtonCommand();

            case "special sex skill":
                return new specialsexskillButtonCommand();

            case "Sex SkillPanel":
                return new SexSkillPanelCommand();


            default:
                return null;
        }
    }

    public void ExecuteCommand(string buttonName, string panelName = "")
    {
        // 尝试执行按钮命令
        if (TryExecuteCommand(commandDictionary, buttonName))
        {
            return; // 成功执行按钮命令
        }

        // 尝试执行面板命令
        if (!string.IsNullOrEmpty(panelName) && TryExecuteCommand(commandDictionary, panelName))
        {
            
            return; // 成功执行面板命令
        }

        // 未找到匹配的按钮或面板命令
        // Debug.LogError($"未找到按钮或面板命令：ButtonName: {buttonName}, PanelName: {panelName}");
    }

    // 尝试执行命令，并返回是否成功
    private bool TryExecuteCommand(Dictionary<string, Icommand> commandDict, string commandName)
    {
        if (commandDict.ContainsKey(commandName))
        {
            Icommand command = commandDict[commandName];
            command.execute();
            return true;
        }
        return false;
    }


}