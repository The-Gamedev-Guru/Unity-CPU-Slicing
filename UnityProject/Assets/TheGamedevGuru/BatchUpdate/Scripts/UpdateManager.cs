#if !DEBUG
#define PERFORMANCE_MODE
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace TheGamedevGuru
{
public interface IBatchUpdate
{
    void BatchUpdate();
}

public interface IBatchLateUpdate
{
    void BatchLateUpdate();
}

public interface IBatchFixedUpdate
{
    void BatchFixedUpdate();
}

public class UpdateManager : MonoBehaviour
{
    private const int BucketCount = 2;
    public static UpdateManager Instance { get; private set; }
    
#if PERFORMANCE_MODE
        private readonly HashSet<IBatchUpdate>[] _slicedUpdateBehavioursBuckets = new HashSet<IBatchUpdate>[BucketCount + 1];
        private readonly HashSet<IBatchLateUpdate>[] _slicedLateUpdateBehavioursBuckets = new HashSet<IBatchLateUpdate>[BucketCount + 1];
        private readonly HashSet<IBatchFixedUpdate>[] _slicedFixedUpdateBehavioursBuckets = new HashSet<IBatchFixedUpdate>[BucketCount + 1];
#else
    private readonly Dictionary<Type, HashSet<IBatchUpdate>>[] _slicedUpdateBehavioursBuckets = new Dictionary<Type, HashSet<IBatchUpdate>>[BucketCount + 1];
    private readonly Dictionary<Type, HashSet<IBatchLateUpdate>>[] _slicedLateUpdateBehavioursBuckets = new Dictionary<Type, HashSet<IBatchLateUpdate>>[BucketCount + 1];
    private readonly Dictionary<Type, HashSet<IBatchFixedUpdate>>[] _slicedFixedUpdateBehavioursBuckets = new Dictionary<Type, HashSet<IBatchFixedUpdate>>[BucketCount + 1];
#endif
    
    private int _currentUpdateAndLateUpdateBucket;
    private int _currentFixedUpdateBucket;

    #region Update
    public void RegisterUpdate(IBatchUpdate batchUpdateBehaviour)
    {
        RegisterUpdate_Internal(batchUpdateBehaviour, BucketCount);
    }
    
    public void RegisterUpdateSliced(IBatchUpdate batchUpdateBehaviour, int bucketNumber)
    {
        if (IsValidBucketNumber(bucketNumber))
        {
            RegisterUpdate_Internal(batchUpdateBehaviour, bucketNumber);
        }
    }
    
    public void DeregisterUpdate(IBatchUpdate batchUpdateBehaviour)
    {
        DeregisterUpdate_Internal(batchUpdateBehaviour);
    }
    #endregion
    
    #region LateUpdate
    public void RegisterLateUpdate(IBatchLateUpdate batchLateUpdateBehaviour)
    {
        RegisterBatchLateUpdate_Internal(batchLateUpdateBehaviour, BucketCount);
    }

    public void RegisterLateUpdateSliced(IBatchLateUpdate batchLateUpdateBehaviour, int bucketNumber)
    {
        if (IsValidBucketNumber(bucketNumber))
        {
            RegisterBatchLateUpdate_Internal(batchLateUpdateBehaviour, bucketNumber);
        }
    }
    
    public void DeregisterLateUpdate(IBatchLateUpdate batchLateUpdateBehaviour)
    {
        DeregisterBatchLateUpdate_Internal(batchLateUpdateBehaviour);
    }
    #endregion
    
    #region FixedUpdate
    public void RegisterFixedUpdate(IBatchFixedUpdate batchFixedUpdateBehaviour)
    {
        RegisterBatchFixedUpdate_Internal(batchFixedUpdateBehaviour, BucketCount);
    }
    
    public void RegisterFixedUpdateSliced(IBatchFixedUpdate batchFixedUpdateBehaviour, int bucketNumber)
    {
        if (IsValidBucketNumber(bucketNumber))
        {
            RegisterBatchFixedUpdate_Internal(batchFixedUpdateBehaviour, BucketCount);
        }
    }
    
    public void DeregisterFixedUpdate(IBatchFixedUpdate batchFixedUpdateBehaviour)
    {
        DeregisterBatchFixedUpdate_Internal(batchFixedUpdateBehaviour);
    }
    #endregion

    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        for (var i = 0; i < BucketCount + 1; i++)
        {
#if PERFORMANCE_MODE
            _slicedUpdateBehavioursBuckets[i] = new HashSet<IBatchUpdate>();
            _slicedLateUpdateBehavioursBuckets[i] = new HashSet<IBatchLateUpdate>();
            _slicedFixedUpdateBehavioursBuckets[i] = new HashSet<IBatchFixedUpdate>();
#else
            _slicedUpdateBehavioursBuckets[i] = new Dictionary<Type, HashSet<IBatchUpdate>>();
            _slicedLateUpdateBehavioursBuckets[i] = new Dictionary<Type, HashSet<IBatchLateUpdate>>();
            _slicedFixedUpdateBehavioursBuckets[i] = new Dictionary<Type, HashSet<IBatchFixedUpdate>>();
#endif
        }
    }

    private void Update()
    {
#if PERFORMANCE_MODE
        foreach (var batchUpdateBehaviour in _slicedUpdateBehavioursBuckets[_currentUpdateAndLateUpdateBucket]) batchUpdateBehaviour.BatchUpdate();
        foreach (var batchUpdateBehaviour in _slicedUpdateBehavioursBuckets[BucketCount]) batchUpdateBehaviour.BatchUpdate();
#else
        foreach (var keyValue in _slicedUpdateBehavioursBuckets[_currentUpdateAndLateUpdateBucket])
        {
            Profiler.BeginSample(keyValue.Key.Name);
            foreach (var behaviour in keyValue.Value)
            {
                behaviour.BatchUpdate();
            }
            Profiler.EndSample();
        }
        foreach (var keyValue in _slicedUpdateBehavioursBuckets[BucketCount])
        {
            Profiler.BeginSample(keyValue.Key.Name);
            foreach (var behaviour in keyValue.Value)
            {
                behaviour.BatchUpdate();
            }
            Profiler.EndSample();
        }
#endif
    }
    
    private void LateUpdate()
    {
#if PERFORMANCE_MODE
        foreach (var batchLateUpdateBehaviour in _slicedLateUpdateBehavioursBuckets[_currentUpdateAndLateUpdateBucket]) batchLateUpdateBehaviour.BatchLateUpdate();
        foreach (var batchLateUpdateBehaviour in _slicedLateUpdateBehavioursBuckets[BucketCount]) batchLateUpdateBehaviour.BatchLateUpdate();
#else
        foreach (var keyValue in _slicedLateUpdateBehavioursBuckets[_currentUpdateAndLateUpdateBucket])
        {
            Profiler.BeginSample(keyValue.Key.Name);
            foreach (var behaviour in keyValue.Value)
            {
                behaviour.BatchLateUpdate();
            }
            Profiler.EndSample();
        }
        foreach (var keyValue in _slicedLateUpdateBehavioursBuckets[BucketCount])
        {
            Profiler.BeginSample(keyValue.Key.Name);
            foreach (var behaviour in keyValue.Value)
            {
                behaviour.BatchLateUpdate();
            }
            Profiler.EndSample();
        }
#endif
        _currentUpdateAndLateUpdateBucket = (_currentUpdateAndLateUpdateBucket + 1) % BucketCount;
    }

    private void FixedUpdate()
    {
#if PERFORMANCE_MODE
        foreach (var batchFixedUpdateBehaviour in _slicedFixedUpdateBehavioursBuckets[_currentFixedUpdateBucket]) batchFixedUpdateBehaviour.BatchFixedUpdate();
        foreach (var batchFixedUpdateBehaviour in _slicedFixedUpdateBehavioursBuckets[BucketCount]) batchFixedUpdateBehaviour.BatchFixedUpdate();
#else
        foreach (var keyValue in _slicedFixedUpdateBehavioursBuckets[_currentFixedUpdateBucket])
        {
            Profiler.BeginSample(keyValue.Key.Name);
            foreach (var behaviour in keyValue.Value)
            {
                behaviour.BatchFixedUpdate();
            }
            Profiler.EndSample();
        }
        foreach (var keyValue in _slicedFixedUpdateBehavioursBuckets[BucketCount])
        {
            Profiler.BeginSample(keyValue.Key.Name);
            foreach (var behaviour in keyValue.Value)
            {
                behaviour.BatchFixedUpdate();
            }
            Profiler.EndSample();
        }
#endif
        
        _currentFixedUpdateBucket = (_currentFixedUpdateBucket + 1) % BucketCount;
    }

    private static bool IsValidBucketNumber(int bucketNumber)
    {
        var isValidBucketNumber = bucketNumber >= 0 && bucketNumber < BucketCount;
        Assert.IsTrue(isValidBucketNumber, $"Bucket must be between 0 and {BucketCount - 1}");
        return isValidBucketNumber;
    }
    
    private void RegisterUpdate_Internal(IBatchUpdate batchUpdateBehaviour, int bucketNumber)
    {
#if PERFORMANCE_MODE
        _slicedUpdateBehavioursBuckets[bucketNumber].Add(batchUpdateBehaviour);
#else
        var type = batchUpdateBehaviour.GetType();
        var targetBucket = _slicedUpdateBehavioursBuckets[bucketNumber];
        if (targetBucket.ContainsKey(type) == false)
        {
            targetBucket[type] = new HashSet<IBatchUpdate>();
        }
        targetBucket[type].Add(batchUpdateBehaviour);
#endif
    }
    
    private void RegisterBatchLateUpdate_Internal(IBatchLateUpdate batchLateUpdateBehaviour, int bucketNumber)
    {
#if PERFORMANCE_MODE
        _slicedLateUpdateBehavioursBuckets[bucketNumber].Add(batchLateUpdateBehaviour);
#else
        var type = batchLateUpdateBehaviour.GetType();
        var targetBucket = _slicedLateUpdateBehavioursBuckets[bucketNumber];
        if (targetBucket.ContainsKey(type) == false)
        {
            targetBucket[type] = new HashSet<IBatchLateUpdate>();
        }
        targetBucket[type].Add(batchLateUpdateBehaviour);
#endif
    }
    
    private void RegisterBatchFixedUpdate_Internal(IBatchFixedUpdate batchFixedUpdateBehaviour, int bucketNumber)
    {
#if PERFORMANCE_MODE
        _slicedFixedUpdateBehavioursBuckets[bucketNumber].Add(batchFixedUpdateBehaviour);
#else
        var type = batchFixedUpdateBehaviour.GetType();
        var targetBucket = _slicedFixedUpdateBehavioursBuckets[bucketNumber];
        if (targetBucket.ContainsKey(type) == false)
        {
            targetBucket[type] = new HashSet<IBatchFixedUpdate>();
        }
        targetBucket[type].Add(batchFixedUpdateBehaviour);
#endif
    }
    
    private void DeregisterUpdate_Internal(IBatchUpdate batchUpdateBehaviour)
    {
#if PERFORMANCE_MODE
        foreach (var batchUpdateBehavioursBucket in _slicedUpdateBehavioursBuckets)
        {
            batchUpdateBehavioursBucket.Remove(batchUpdateBehaviour);
        }
#else
        foreach (var keyValue in _slicedUpdateBehavioursBuckets)
        {
            var type = batchUpdateBehaviour.GetType();
            if (keyValue.ContainsKey(type))
            {
                keyValue[type].Remove(batchUpdateBehaviour);
            }
        }
#endif
    }
    
    private void DeregisterBatchLateUpdate_Internal(IBatchLateUpdate batchLateUpdateBehaviour)
    {
#if PERFORMANCE_MODE
        foreach (var batchLateUpdateBehaviourBucket in _slicedLateUpdateBehavioursBuckets)
        {
            batchLateUpdateBehaviourBucket.Remove(batchLateUpdateBehaviour);
        }
#else
        foreach (var keyValue in _slicedLateUpdateBehavioursBuckets)
        {
            var type = batchLateUpdateBehaviour.GetType();
            if (keyValue.ContainsKey(type))
            {
                keyValue[type].Remove(batchLateUpdateBehaviour);
            }
        }
#endif
    }
    
    private void DeregisterBatchFixedUpdate_Internal(IBatchFixedUpdate batchFixedUpdateBehaviour)
    {
#if PERFORMANCE_MODE
        foreach (var batchFixedUpdateBehaviourBucket in _slicedFixedUpdateBehavioursBuckets)
        {
            batchFixedUpdateBehaviourBucket.Remove(batchFixedUpdateBehaviour);
        }
#else
        foreach (var keyValue in _slicedFixedUpdateBehavioursBuckets)
        {
            var type = batchFixedUpdateBehaviour.GetType();
            if (keyValue.ContainsKey(type))
            {
                keyValue[type].Remove(batchFixedUpdateBehaviour);
            }
            
        }
#endif
    }
}
}