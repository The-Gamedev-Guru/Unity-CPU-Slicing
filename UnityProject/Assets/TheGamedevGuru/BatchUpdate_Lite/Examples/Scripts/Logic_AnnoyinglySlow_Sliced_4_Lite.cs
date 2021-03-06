﻿using System.Threading;
using TheGamedevGuru;
using UnityEngine;

public class Logic_AnnoyinglySlow_Sliced_4_Lite : MonoBehaviour, IBatchUpdate
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
        Thread.Sleep(4);    // A lot of calculations, trust me!
    }
}
