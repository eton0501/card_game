using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T:MonoBehaviour
{
    public static T Instance{get;private set;}//任何地方都能透過ClassName.Instance來存取
    protected virtual void Awake()//物件被建立時自動呼叫
    {
        if(Instance != null)//如果已經存在一個實例
        {
            Destroy(gameObject);//摧毀這個物件(確保全局只有一個實例)
            return;
        }
        Instance=this as T;//如果目前還沒有實例，將自己設定為唯一的實例
    }
    protected virtual void OnApplicationQuit()//應用程式關閉時自動呼叫
    {
        Instance=null;//清除Instance
        Destroy(gameObject);//摧毀物件
    }
}
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();//先執行父類別的Awake
        DontDestroyOnLoad(gameObject);//切換場景時不要摧毀這個物件
    }
}
