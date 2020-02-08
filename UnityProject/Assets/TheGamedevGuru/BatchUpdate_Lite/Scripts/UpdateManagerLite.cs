// Copyright 2019 The Gamedev Guru (http://thegamedev.guru)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Collections.Generic;
using UnityEngine;

namespace TheGamedevGuru
{
public class UpdateManagerLite : MonoBehaviour
{
    public enum UpdateMode { BucketA, BucketB, Always }
    public static UpdateManagerLite Instance { get; private set; }
    private readonly HashSet<IBatchUpdate> _slicedUpdateBehavioursBucketA = new HashSet<IBatchUpdate>();
    private readonly HashSet<IBatchUpdate> _slicedUpdateBehavioursBucketB = new HashSet<IBatchUpdate>();
    private bool _isCurrentBucketA;

    public void RegisterSlicedUpdate(IBatchUpdate slicedUpdateBehaviour, UpdateMode updateMode)
    {
        if (updateMode == UpdateMode.Always)
        {
            _slicedUpdateBehavioursBucketA.Add(slicedUpdateBehaviour);
            _slicedUpdateBehavioursBucketB.Add(slicedUpdateBehaviour);
        }
        else
        {
            var targetUpdateFunctions = updateMode == UpdateMode.BucketA ? _slicedUpdateBehavioursBucketA : _slicedUpdateBehavioursBucketB;
            targetUpdateFunctions.Add(slicedUpdateBehaviour);
        }
    }
    
    public void DeregisterSlicedUpdate(IBatchUpdate slicedUpdateBehaviour)
    {
        _slicedUpdateBehavioursBucketA.Remove(slicedUpdateBehaviour);
        _slicedUpdateBehavioursBucketB.Remove(slicedUpdateBehaviour);
    }
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        var targetUpdateFunctions = _isCurrentBucketA ? _slicedUpdateBehavioursBucketA : _slicedUpdateBehavioursBucketB;
        foreach (var slicedUpdateBehaviour in targetUpdateFunctions)
        {
            slicedUpdateBehaviour.BatchUpdate();
        }
        _isCurrentBucketA = !_isCurrentBucketA;
    }
}
}
