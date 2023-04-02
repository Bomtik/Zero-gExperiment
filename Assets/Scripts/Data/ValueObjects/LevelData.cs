using System.Collections.Generic;
using System;
using UnityEngine;

namespace Data.ValueObjects
{

    [Serializable]
    public class LevelData
    {
        public List<KeysData> KeysList = new List<KeysData>();
    }

    [Serializable]
    public struct KeysData
    {
        public sbyte RequiredKeysCount;
    }

}