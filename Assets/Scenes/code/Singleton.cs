using System.Threading;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>    //����Լ�������߱�����T��Singleton�е�T
{
    //���̰߳�ȫ�ķ��͵���
    //Linux epoll ����ģ�ͽ��и߲���ͨ�ż���client���� Linux���̼߳���number one �߳��������л���
    public static T instance { get; private set; }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;   //����ת�����̶�д��
        }
        else if (instance != this)  //��Ϊ�״�ʵ����ʱinstance�Ѿ�����˶�������ã���ʱthis�������ʵ��
        {
           Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}


// ����SDK 3���º� ��Ϊ��Ӵ�����Ŀһ�����Ѿ����Ƶ� java and ץ�� ͼ����ɫ������
//��֧�� ��Ҫ΢��sdk ����������߰� h cpp Դ���� pay.findMyslefMoney(int uid); ���� int 20��  int ��ɰ� json  ���͸�ǰ�� ��Ϳ��Ի�ˮ��
// ��� �㷨�ӿ촫���ٶ� bytearray java ͨ�ô������ ���㷨��Ҫѹ�� 400M 300M �������� 