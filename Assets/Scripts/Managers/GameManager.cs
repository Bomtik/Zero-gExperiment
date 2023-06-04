using Enums;
using Signals;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SelfVariables

    #region SeriliezedVariables

    [SerializeField] private GameState states;

    #endregion

    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        //CoreGameSignals.Instance.onChangeGameState += OnChangeGameState;
    }

    private void UnSubscribedEvents()
    {
        //CoreGameSignals.Instance.onChangeGameState -= OnChangeGameState;
    }

    private void OnDisable()
    {
        UnSubscribedEvents();
    }

    private void OnChangeGameState(GameState state)
    {
        states = state;
    }
}