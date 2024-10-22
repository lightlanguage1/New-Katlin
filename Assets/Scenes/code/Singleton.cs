using System.Threading;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>    //泛型约束，告诉编译器T是Singleton中的T
{
    //非线程安全的泛型单例
    //Linux epoll 网络模型进行高并发通信监听client连接 Linux多线程技术number one 线程上下文切换快
    public static T instance { get; private set; }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;   //必须转换，固定写法
        }
        else if (instance != this)  //因为首次实例化时instance已经获得了对象的引用，此时this是子类的实例
        {
           Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}


// 接入SDK 3个月后 因为你接触的项目一定是已经完善的 java and 抓包 图标蓝色鲨鱼鳍
//做支付 需要微信sdk 软件开发工具包 h cpp 源代码 pay.findMyslefMoney(int uid); 返回 int 20亿  int 打成包 json  发送给前端 你就可以划水了
// 打包 算法加快传输速度 bytearray java 通用打包工具 用算法是要压缩 400M 300M 哈夫曼树 