using XX.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPGManager : SingletonDontDestroy<ARPGManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
