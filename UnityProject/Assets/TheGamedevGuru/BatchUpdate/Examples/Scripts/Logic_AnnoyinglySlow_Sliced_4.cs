using System.Threading;
using TheGamedevGuru;
using UnityEngine;

public class Logic_AnnoyinglySlow_Sliced_4 : MonoBehaviour, IBatchUpdate
{
    private void Start()
    {
        UpdateManager.Instance.RegisterUpdateSliced(this, 0);
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
        Thread.Sleep(4);    // A lot of calculations, trust me!
    }
}
