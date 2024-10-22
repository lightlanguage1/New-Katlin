using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// ö�ٶ����˼�������
public enum SkillType
{
    daji = 1,            // �����
    xuli,                // ������
    mishujiashi,         // ������ʻ
    kanpojiashi,         // ���Ƽ�ʻ
    roubangguancha,       // ���۲�
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

// �����˼�������ӿ�
public interface ISkillCommand
{
    void Execute(); // ִ������ķ���
}

// ������������ʵ���� ISkillCommand �ӿ�
public class XuliSkillCommand : ISkillCommand
{
    public void Execute()
    {
        // �������ǹ�������������������

        var battleManager = BattleManager.instance;
        var karryn = battleManager.canvas.transform.GetChild((int)ChildrenType.Karryn).gameObject;

        // ���ż�����Ч����
        battleManager.canvas.transform.GetChild((int)ChildrenType.SkillEffect).gameObject.SetActive(true);
        Animator animator = battleManager.canvas.transform.GetChild((int)ChildrenType.SkillEffect).GetComponent<Animator>();
        animator.Play("xuli");

        var player = PlayerManager.instance;
        player.atk += 50 * player.level; // ���ӹ�����
        // �޸� UI ��ʾ�Ĺ�����
        GameObject infoPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject;
        GameObject targetObj = infoPanel.transform.GetChild(0).GetChild(0).GetChild(14).gameObject;
        //TextMeshProUGUI textPro = targetObj.GetComponent<TextMeshProUGUI>();
        //textPro.text = player.atk.ToString(); // ���¹������ı�
        // �л����յ�״̬ͼ
        battleManager.childrenDic["Karryn"][(int)battleManager.playerCurState].gameObject.SetActive(false);
        battleManager.playerCurState = KarrynState.Weapon2;
        battleManager.childrenDic["Karryn"][(int)battleManager.playerCurState].gameObject.SetActive(true);
        // �رվ�����壬���ü�������
        battleManager.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject.SetActive(false);
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].StopPlayback();
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.curPanel.SetActive(true);
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0���ȡ��
        battleManager.isPanel = false;
        PlayerManager.instance.subtractVolition(Global.instance.xuli);    //50
        battleManager.isInputEnable = true;
    }
}

// �����������ʵ���� ISkillCommand �ӿ�
public class dajiButtonCommand : ISkillCommand
{
    public void Execute()
    {
        this.closeSkillPannel(ref BattleManager.instance.curPanel);
        showSelectFrame();
    }

    // ��ʾѡ�п�
    public void showSelectFrame()
    {
        GameObject frame = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
        GameObject cursor = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
        frame.SetActive(true);
        cursor.SetActive(true);
        // ��ֹͣ Fixed Update �ڵ� checkUserMoveAction()��Ȼ��������Э��ִ��ѡ�е��˹�������
        BattleManager.instance.isPanel = false;
        BattleManager.instance.isInputEnable = true;
        BattleManager.instance.coroutineSeletedEnemy = BattleManager.instance.
        StartCoroutine(BattleManager.instance.beginSelectEnemyAtk(BattleManager.instance.prefabs, SkillType.daji));
    }

    // �رռ������
    public void closeSkillPannel(ref GameObject curPanel_)
    {
        BattleManager battleManager = BattleManager.instance;
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].StopPlayback();
        curPanel_.SetActive(false);
    }
}

// ���������������
public class SkillCommandManager
{
    private Dictionary<string, ISkillCommand> skillCommandDictionary; // ���������ֵ�
    private List<Button> buttons; // ��ť�б�

    // ���캯������ʼ�����������ֵ�Ͱ�ť�б�
    public SkillCommandManager(Button[] buttons)
    {
        skillCommandDictionary = new Dictionary<string, ISkillCommand>();
        this.buttons = new List<Button>(buttons);

        // ��ʼ������似�������ֵ�
        PopulateSkillCommandDictionary();
    }

    // ��似�������ֵ�
    private void PopulateSkillCommandDictionary()
    {
        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
        {
            string buttonName = skillType.ToString().ToLower();
            ISkillCommand skillCommand = CreateSkillCommand(skillType);
            if (skillCommand != null)
            {
                Debug.Log($"��Ӱ�ť: {buttonName}");
                skillCommandDictionary.Add(buttonName, skillCommand);
            }
            else
            {
                Debug.LogWarning($"δ�ҵ����� {skillType} ��Ӧ������");
            }
        }
    }

    // ���ݰ�ť���ƻ�ȡ��������
    public ISkillCommand GetSkillCommand(string buttonName)
    {
        string formattedButtonName = buttonName.ToLower(); // ������İ�ť����ת��ΪСд
        if (skillCommandDictionary.ContainsKey(formattedButtonName))
        {
            return skillCommandDictionary[formattedButtonName];
        }
        else
        {
            // ���������Ϣ����ʾ����İ�ť����
            Debug.Log($"����İ�ť����: {buttonName}");

            // ������������ֵ��е����м����Լ���Ƿ�������
            Debug.Log("��ǰ���������ֵ��ڵİ�ť����");
            foreach (var key in skillCommandDictionary.Keys)
            {
                Debug.Log(key);
            }

            // ���������Ϣ����ʾδ�ҵ���ť��Ӧ�ļ�������
            Debug.LogWarning($"δ�ҵ���ť {buttonName} ��Ӧ�ļ�������");
            return null;
        }
    }


    // ִ��ָ����ť���ƶ�Ӧ�ļ�������
    public bool ExecuteSkill(string buttonName)
    {
        // ������İ�ť����ת��ΪСд
        string formattedButtonName = buttonName.ToLower();

        // ���� GetSkillCommand ����������ת����İ�ť����
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

    // ���ݼ������ͻ�ȡ��������
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

