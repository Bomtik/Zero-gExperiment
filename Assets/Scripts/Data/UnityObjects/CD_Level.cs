using UnityEngine;
using Data.ValueObjects;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Data.UnityObjects
{
    [CreateAssetMenu(fileName = "CD_Level", menuName = "Zero-G Experiment/CD_Level", order = 1)]
    public class CD_Level : SerializedScriptableObject
    {
        public List<LevelData> Levels = new List<LevelData>();
    }
}