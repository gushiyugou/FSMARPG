using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XX.Tool.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        //_instance = FindObjectOfType<T>() as T; //先去场景中找有没有这个类
                        instance = FindFirstObjectByType<T>() as T;//先去场景中找有没有这个类
                        if (instance == null)//如果没有，那么我们自己创建一个Gameobject然后给他加一个T这个类型的脚本，并赋值给instance;
                        {
                            GameObject go = new GameObject(typeof(T).Name);
                            go.transform.SetParent(ARPGManager.Instance.transform,false);
                            instance = go.AddComponent<T>();
                        }
                    }
                }

                return instance;
            }
        }


        protected virtual void Awake()
        {
            if (instance == null) instance = (T)this;
        }


        private void OnApplicationQuit()//程序退出时，将instance清空
        {
            instance = null;
        }
    }

}
