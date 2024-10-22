using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class GameTimeManager : MonoBehaviour
{
    public bool inMainScene = true;
    public float time = 0;
    public float timer = 0;

    public TextMeshProUGUI timeText;
    public Image hourHand;
    public Image minuteHand;

    [Header("时间模块")]
    // 光照效果持续时间
    public float duration;

    [SerializeField] private List<LightControl> sceneLights; // 保存场景中的所有灯光
    private LightShift lightShift = LightShift.Forenoon;
    public static GameTimeManager instance;
  



    // 添加天气色调
    /* public Color morningColor = Color.white;
     public Color nightColor = Color.blue;
     public Color forenoonColor = Color.blue;
     public Color noonColor = Color.blue;
     public Color afternoonColor = Color.blue;
     public Color midnightColor = Color.blue;*/

    // 添加 setGameVariable 相关
    public int year = 1;
    public int month = 1;
    public int day = 1;
    public int hour = 0;
    public int minute = 0;
    public int timeZone = 0;
    public int weatherType = 0;
    public int totalTime = 0;
    public int totalDay = 1;

    // 添加 Game_Chronus 相关
    private Game_Chronus chronus;

    private void Start()
    {
        // 初始化 Game_Chronus
        inMainScene = true;
        duration = 20f;
        // 初始化 sceneLights 列表
        sceneLights = new List<LightControl>();
        instance = this;  // 使用当前实例赋值给 instance
        UpdateLights(LightShift.Forenoon);
    }


    private int currentPhase = 0;  // 用于追踪当前时间段

    private void Update()
    {
        if (inMainScene)
        {
            time += Time.deltaTime / 1;

            // 更新 setGameVariable
            UpdateGameVariables();

            if (time >= 4320)
            {
                if (timer >= 1)
                {
                    timer = 1;
                    timeText.text = "凌晨";
                  
                    // 重置计时器和天数
                    ResetTimerAndDay();

                    return;
                }

                timer++;

                time -= 4320;
            }
            UpdateOtherTexts(currentPhase);
            // 设置计时器和时间段
            SetTimerAndPhase();
            // 更新时钟 UI
            UpdateClockUI();

        }
    }

    private void UpdateLights(LightShift lightShift)
    {
        // 清空之前保存的灯光控件列表
        sceneLights.Clear();
        // 查找场景中所有的灯光控件并添加到列表中
        sceneLights.AddRange(FindObjectsByType<LightControl>(FindObjectsSortMode.None));
        // 遍历所有灯光控件并根据新的灯光转换类型进行灯光变化
        foreach (var light in sceneLights)
        {
            light.ChangeLight(lightShift);
        }
    }

    // 设置计时器和时间段

    private void SetTimerAndPhase()
    {
        if (time >= 720 && time < 1440)
        {
            timer = 1;
            currentPhase = 1;
            UpdateLights(LightShift.Forenoon);
        }
        else if (time >= 1440 && time < 2160)
        {
            timer = 2;
            currentPhase = 2;
            UpdateLights(LightShift.Morning);
        }
        else if (time >= 2160 && time < 2880)
        {
            timer = 3;
            currentPhase = 3;
            UpdateLights(LightShift.Noon);
        }
        else if (time >= 2880 && time < 3600)
        {
            timer = 4;
            currentPhase = 4;
            UpdateLights(LightShift.Afternoon);
        }
        else if (time >= 3600 && time < 4320)
        {
            timer = 5;
            currentPhase = 5;
            UpdateLights(LightShift.Night);
        }
        else if (time >= 4320)
        {
            timer = 6;
            currentPhase = 6;
            UpdateLights(LightShift.Midnight);
            // 天数加一，重置计时器
            IncrementDay();
            ResetTimerAndDay();
        }
    }

    private void UpdateOtherTexts(int currentPhase)
    {
        switch (currentPhase)
        {
            case 1:
                timeText.text = "凌晨";
                break;
            case 2:
                timeText.text = "上午";
                break;
            case 3:
                timeText.text = "中午";
                break;
            case 4:
                timeText.text = "下午";
                break;
            case 5:
                timeText.text = "晚上";
                break; 
            case 6:
                timeText.text = "午夜";
                break;
            default:
                //默认的更新逻辑，可以是 night 或其他
                timeText.text = "凌晨";
                break;
        }
        //Debug.Log("Text updated: " + timeText.text); // 添加这行进行调试
    }

    // 重置计时器和天数
    private void ResetTimerAndDay()
    {
        timer = 0;
        currentPhase = 0;
    }

    // 天数加一
    private void IncrementDay()
    {
        totalDay++;
    }


    // 更新 setGameVariable
    void UpdateGameVariables()
    {
        // 确保 Game_Chronus 对象不为空
        if (chronus == null)
        {
            return;
        }
        year = chronus.getYear();
        month = chronus.getMonth();
        day = chronus.getDay();
        hour = chronus.getHour();
        minute = chronus.getMinute();
        timeZone = chronus.getTimeZone();
        weatherType = chronus.getWeatherTypeId();
        totalTime = chronus.getTotalTime();
        totalDay = chronus.getTotalDay();
    }
    public class Game_Chronus
    {
        private DateTime _nowDate;
        private int _dayMeter;
        private int _timeMeter;
        private int _frameCount;
        private bool _disablePadding;

        private List<int> _daysOfMonth;
        private string[] _weekNames;
        private string[] _monthNames;

        private static (int start, int end, int timeId)[] timeZone;

        private int _weatherType;

        /*public Game_Chronus()
        {
            // 在构造函数中初始化需要的变量和列表
            _daysOfMonth = new List<int> { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            _weekNames = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            _monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            timeZone = new (int, int, int)[]
  {
    // 格式：(开始小时, 结束小时, 时间ID)
    (0, 6, 1),    // 凌晨 0 点到早上 6 点，时间ID 1
    (6, 12, 2),   // 早上 6 点到中午 12 点，时间ID 2
    (12, 18, 3),  // 中午 12 点到下午 6 点，时间ID 3
    (18, 24, 4)   // 下午 6 点到晚上 12 点，时间ID 4
  };


            // 初始化其他变量
            _nowDate = DateTime.Now;
            _dayMeter = 0;
            _timeMeter = 0;
            _frameCount = 0;
            _disablePadding = false;
            _weatherType = 0;
        }*/

        public int getYear()
        {
            return _nowDate.Year;
        }

        public int getMonth()
        {
            return _nowDate.Month;
        }

        public int getDay()
        {
            return _nowDate.Day;
        }

        public int getHour()
        {
            return _nowDate.Hour;
        }

        public int getMinute()
        {
            return _nowDate.Minute;
        }

        public int getTimeZone()
        {
            int timeId = 0;
            foreach (var zoneInfo in timeZone)
            {
                if (isHourInRange(zoneInfo.start, zoneInfo.end)) timeId = zoneInfo.timeId;
            }
            return timeId;
        }

        public int getWeatherTypeId()
        {
            return _weatherType;
        }

        public int getTotalTime()
        {
            return (int)(_nowDate - DateTime.Now).TotalMinutes;
        }

        public int getTotalDay()
        {
            return (int)Math.Floor((double)getTotalTime() / (24 * 60)) + 1;
        }

        private bool isHourInRange(int min, int max)
        {
            var hour = getHour();
            return hour >= min && hour <= max;
        }
    }

    // 切换天气色调
/*    void ChangeWeatherColor(LightShift shift)
    {
        switch (shift)
        {
            case LightShift.Morning:
                Camera.main.backgroundColor = morningColor;
                break;
            case LightShift.Forenoon:
                Camera.main.backgroundColor = forenoonColor;
                break;
            case LightShift.Noon:
                Camera.main.backgroundColor = noonColor;
                break;
            case LightShift.Afternoon:
                Camera.main.backgroundColor = afternoonColor;
                break;
            case LightShift.Night:
                Camera.main.backgroundColor = nightColor;
                break;
            case LightShift.Midnight:
                Camera.main.backgroundColor = midnightColor;
                break;
            default:
                // 默认使用 nightColor 或者其他处理逻辑
                Camera.main.backgroundColor = nightColor;
                break;
        }
    }*/



    // 时钟 UI 更新
    private void UpdateClockUI()
    {
        // 计算时针和分针的旋转角度
        float hourRotation = -(time / 4320f) * 360f;  // 360 秒表示一小时
        float minuteRotation = -(time % 4320f / 60f) * 360f;  // 60 秒表示一分钟

        // 设置时钟的旋转
        hourHand.rectTransform.rotation = Quaternion.Euler(0f, 0f, hourRotation);
        minuteHand.rectTransform.rotation = Quaternion.Euler(0f, 0f, minuteRotation);
    }

}