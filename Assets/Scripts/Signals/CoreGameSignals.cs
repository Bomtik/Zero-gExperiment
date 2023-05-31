using System;
using Extensions;
using Enums;
using UnityEngine.Events;

namespace Signals
{
    public class CoreGameSignals : MonoSingleton<CoreGameSignals>
    {
        public UnityAction<GameState> onChangeGameState = delegate { };
        public UnityAction<int> onLevelInitialize = delegate { };
        public UnityAction onClearActiveLevel = delegate { };
        public UnityAction onLevelSuccessful = delegate { };
        public UnityAction onStageAreaEntered = delegate { };
        public UnityAction onLevelFailed = delegate { };
        public UnityAction onRestartLevel = delegate { };

        public UnityAction onPlay = delegate { };
        public UnityAction onReset = delegate { };
        public Func<int> onGetLevelValue = delegate { return 0; };
    }
}