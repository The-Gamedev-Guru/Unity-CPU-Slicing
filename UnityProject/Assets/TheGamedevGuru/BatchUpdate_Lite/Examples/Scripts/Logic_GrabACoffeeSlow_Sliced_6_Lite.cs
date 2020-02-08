using System;
using System.Threading;
using TheGamedevGuru;
using UnityEngine;

public class Logic_GrabACoffeeSlow_Sliced_6_Lite : MonoBehaviour, IBatchUpdate
{
    private void Start()
    {
        UpdateManagerLite.Instance.RegisterSlicedUpdate(this, UpdateManagerLite.UpdateMode.BucketB);
    }

    private void OnDestroy()
    {
        UpdateManagerLite.Instance.DeregisterSlicedUpdate(this);
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
