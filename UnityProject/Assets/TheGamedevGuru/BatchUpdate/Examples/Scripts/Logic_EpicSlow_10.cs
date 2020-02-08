using System.Threading;
using TheGamedevGuru;
using UnityEngine;

public class Logic_EpicSlow_10 : MonoBehaviour, IBatchUpdate
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
        Thread.Sleep(10); // A lot of calculations, trust me!
    }


}
