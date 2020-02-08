using System;
using System.Threading;
using TheGamedevGuru;
using UnityEngine;

public class Logic_GrabACoffeeSlow_6 : MonoBehaviour, IBatchUpdate
{
    private void Start()
    {
        UpdateManager.Instance.RegisterUpdate(this);
    }

    private void OnDestroy()
    {
        UpdateManager.Instance.DeregisterUpdate(this);
    }

    public void BatchUpdate()
    {
        SlowWork();
    }
    
    private void SlowWork()
    {
        Thread.Sleep(6);    // A lot of calculations, trust me!
    }
}
