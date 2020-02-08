using System.Threading;
using TheGamedevGuru;
using UnityEngine;

public class Logic_EpicSlow_Sliced_10_Lite : MonoBehaviour, IBatchUpdate
{
    private void Start()
    {
        UpdateManagerLite.Instance.RegisterSlicedUpdate(this, UpdateManagerLite.UpdateMode.BucketA);
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
        Thread.Sleep(10);    // A lot of calculations, trust me!
    }
}
