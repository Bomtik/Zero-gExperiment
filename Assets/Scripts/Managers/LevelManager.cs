using Commands;
using Commands.Level;
using Data.UnityObjects;
using Signals;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public int levelValue;
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private int totalLevelCount, levelID;
        [SerializeField] private Transform levelHolder;

        #endregion

        #region Private Variables

        private OnLevelLoaderCommand _levelLoaderCommand;
        private OnLevelDestroyerCommand _levelDestroyerCommand;

        #endregion

        #endregion

        private void Awake()
        {
            levelID = GetActiveLevel();

            Init();
        }

        private int GetActiveLevel()
        {
            if (ES3.FileExists())
            {
                if (ES3.KeyExists("Level"))
                {
                    return ES3.Load<int>("Level");
                }
            }

            return levelValue;
        }

        private void Init()
        {
            _levelLoaderCommand = new OnLevelLoaderCommand(levelHolder);
            _levelDestroyerCommand = new OnLevelDestroyerCommand(levelHolder);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize += _levelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel += _levelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
            CoreGameSignals.Instance.onGetLevelValue += OnGetLevelValue;
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize -= _levelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel -= _levelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;
            CoreGameSignals.Instance.onGetLevelValue -= OnGetLevelValue;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void Start()
        {
            CoreGameSignals.Instance.onLevelInitialize?.Invoke(levelID % totalLevelCount);
        }

        private void OnNextLevel()
        {
            levelID++;
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke(levelID % totalLevelCount);
        }

        private void OnRestartLevel()
        {
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke(levelID % totalLevelCount);
        }

        private int OnGetLevelValue()
        {
            return levelID % totalLevelCount;
        }
    }
}