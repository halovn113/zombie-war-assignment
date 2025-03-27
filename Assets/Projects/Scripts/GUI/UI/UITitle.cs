using PrimeTween;
using UnityEngine;

public class UITitle : MonoBehaviour
{
    public GameObject textPress;
    void Awake()
    {
        Sequence.Create(cycles: -1, cycleMode: CycleMode.Yoyo).Chain(Tween.Scale(textPress.transform, 1.1f, 0.5f)).Chain(Tween.Scale(textPress.transform, 1, 0.5f));

    }
    public void StartGame()
    {
        ServiceLocator.Get<GameStateManager>().ChangeState(GAME_STATE.LOAD_MAP);
    }
}
