using System.Collections.Generic;
using UnityEngine;
using static Global;

public class NPC : MonoBehaviour
{
    [Header("敌人基础信息")]
    public bool playerInRange = false;  // 玩家是否在NPC的范围内

    public bool isUP = false;  // 是否从上方进入触发器
    public bool isDiscoverPlayer;  // 是否发现了玩家

    // 通过精灵渲染器改变层级，让玩家在NPC上方时被遮挡
    private SpriteRenderer playerRenderer;
    private SpriteRenderer npcRenderer;
    private bool triggerCompleted;

    [Header("敌人动画器")]
    public Animator animator;

    [Header("计时器等待时间")]
    public float waitTime;
    public float timer;
    public bool isWait;

    [Header("NPC移动")]
    public float speed;
    public float length;  // 射线长度
    public Vector3 moveDir;
    public bool isMove;

    [Header("NPC 2D刚体和检测")]
    public Rigidbody2D rb;
    public Vector2 offset;
    public Vector2 checkSize;
    public float checkDis;
    public LayerMask attackMask;

    [Header("算法")]
    public AStar astar;
    [Header("玩家的位置")]
    public Vector3 playerPos;
    public Grid grid;

    // 拥有的状态
    public Dictionary<StateType, FSM> states;
    [HideInInspector] public StateType curState;

    private void Awake()
    {
        // 初始化状态机
        states = new Dictionary<StateType, FSM>
        {
            {StateType.IdleState, new GoblinIdleState()},
            {StateType.PatrolState, new GoblinPatrolState()},
            {StateType.ChaseState, new GoblinChaseState()}
        };

        triggerCompleted = false;
    }

    private void OnEnable()
    {
        // 设置默认状态为IdleState，并注册事件处理函数
        curState = StateType.IdleState;
        isMove = false;
        moveDir = generateRandomXyDir();
        states[curState].onEnter(this);
        EventHandler.onTrigger += stopTrigger;
    }

    private void OnDisable()
    {
        // 取消事件注册
        EventHandler.onTrigger -= stopTrigger;
    }

    // 回调函数，用于停止触发器
    private void stopTrigger(bool isStop)
    {
        triggerCompleted = isStop;
    }

    private void Start()
    {
        // 初始化NPC位置
        string selfName = gameObject.name;
        Vector3 v3;
        if (Global.instance.posDic.ContainsKey(selfName))
        {
            v3 = Global.instance.posDic[selfName];
        }
        else
        {
            // 用于测试
            v3 = new Vector3(Global.instance.enemy_x, Global.instance.enemy_y, Global.instance.enemy_z);
        }
        transform.localPosition = new Vector3(v3.x, v3.y, 0);
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            // 移动NPC位置
            Vector3 v3 = new Vector3(moveDir.x * Random.Range(speed, speed * 2) * Time.deltaTime,
                moveDir.y * Random.Range(speed, speed * 2) * Time.deltaTime, 0);
            rb.MovePosition((Vector2)v3 + (Vector2)transform.position);
        }
    }

    private void Update()
    {
        // 更新当前状态的逻辑
        states[curState].logicUpdate();
    }

    // 当玩家进入碰撞体时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果进入的是玩家且触发器未完成
        if (other.CompareTag("Player") && !triggerCompleted)
        {
            EventHandler.callTriggerStopEvent(true);  // 防止多个触发器同时触发
            isDiscoverPlayer = true;
            if (isUP)
            {
                // 调整层级，使玩家在NPC上方时被遮挡
                playerRenderer = other.GetComponent<SpriteRenderer>();
                npcRenderer = GetComponent<SpriteRenderer>();
                npcRenderer.sortingOrder += playerRenderer.sortingOrder;
            }
            NPC[] enemies = FindObjectsByType<NPC>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                // 关闭其他敌人的触发器，使其休眠，停止动画播放
                enemies[i].GetComponent<Collider2D>().isTrigger = false;
                enemies[i].rb.Sleep();
                enemies[i].animator.StopPlayback();
            }
            // 设置玩家在NPC附近
            playerInRange = true;
            Global.instance.battlePrevNpcName = gameObject.name;  // 获取要战斗的敌人名字
            // 遇敌，将所有敌人的当前位置添加到字典中
            var parent = GameObject.Find("Enemies");
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (!Global.instance.posDic.ContainsKey(parent.transform.GetChild(i).name))
                {
                    Global.instance.posDic.Add(parent.transform.GetChild(i).name, parent.transform.GetChild(i).localPosition);
                }
            }
            // 进入战斗场景
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.BattleScene);
        }
    }


    // 当玩家离开碰撞体时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 如果离开的是玩家
        if (other.CompareTag("Player"))
        {
            if (isUP)
            {
                isUP = false;
                npcRenderer.sortingOrder -= playerRenderer.sortingOrder;
            }
            // 设置玩家不在NPC附近
            playerInRange = false;
        }
    }

    // 切换状态的方法
    public void switchState(StateType state)
    {
        switch (state)
        {
            case StateType.IdleState:
                {
                    states[curState].onExit();
                    curState = StateType.IdleState;
                    states[curState].onEnter(this);
                    break;
                }
            case StateType.PatrolState:
                {
                    states[curState].onExit();
                    curState = StateType.PatrolState;
                    states[curState].onEnter(this);
                    break;
                }
            case StateType.ChaseState:
                {
                    states[curState].onExit();
                    curState = StateType.ChaseState;
                    states[curState].onEnter(this);
                    break;
                }
        }
    }

    // 更新NPC的移动方向
    public void updateMoveDir()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, moveDir, length, LayerMask.GetMask("BG"));
/*#if UNITY_EDITOR
        // 在编辑器中绘制射线，用于调试
        Debug.DrawRay(transform.position, moveDir * length);
#endif*/
        if (hit2D.collider)
        {
/*#if UNITY_EDITOR
            // 如果击中碰撞体，说明前方有障碍物，调头
            Debug.Log("碰撞体被击中：" + hit2D.collider.gameObject.name);
#endif*/
            moveDir = -moveDir;
            playNextAnim(moveDir, "goblin", "Walk");
        }
        else
        {
/*#if UNITY_EDITOR
            // 没有击中碰撞体
            //Debug.LogWarning("没有击中碰撞体");
#endif*/
            isMove = true;
        }
    }

    // 播放下一个动画
    public void playNextAnim(Vector3 moveDir_, string npcName, string actionName)
    {
        if (moveDir_.x == 1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Right");
        }
        else if (moveDir_.x == -1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Left");
        }
        else if (moveDir_.y == 1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Up");
        }
        else if (moveDir_.y == -1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Down");
        }
    }

    // 生成一个随机的X，Y方向
    public Vector3 generateRandomXyDir()
    {
        int randomDirection = Random.Range(0, 4);

        switch (randomDirection)
        {
            case 0: // 上
                return Vector3.up;

            case 1: // 下
                return Vector3.down;

            case 2: // 左
                return Vector3.left;

            case 3: // 右
                return Vector3.right;
        }
        return Vector3.zero;
    }

#if UNITY_EDITOR
    // 在编辑器中绘制射线和盒子，用于调试
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, length * moveDir);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, checkSize);
        Gizmos.DrawRay((Vector2)transform.position + offset, checkDis * moveDir);
    }
#endif

    // 检测是否发现玩家
    public bool foundPlayer()
    {
        int rayCount = 10; // 射线数量
        float angle = 45f; // 视线夹角
        float viewDistance = 3f; // 视角射线长度，用于调试

        float angleIncrement = angle / (rayCount - 1); // 角度增量

        for (int i = 0; i < rayCount; i++)
        {
            // 计算当前射线的方向
            Vector3 rayDirection = Quaternion.Euler(0, 0, -angle / 2 + i * angleIncrement) * moveDir;

            // 发射射线
            RaycastHit2D hit2D = Physics2D.Raycast((Vector2)transform.position + offset, rayDirection, viewDistance, attackMask);

/*#if UNITY_EDITOR
            // 在编辑器中绘制射线，用于调试
            Debug.DrawRay((Vector2)transform.position + offset, rayDirection * viewDistance, Color.green);
#endif*/

            // 如果射中了碰撞体，返回 true，表示发现了玩家
            if (hit2D.collider != null)
            {
                playerPos = hit2D.collider.transform.localPosition;
                return true;
            }
        }

        // 如果没有任何一条射线检测到障碍物，返回 false
        return false;
    }
}
