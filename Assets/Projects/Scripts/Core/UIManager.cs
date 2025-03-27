using AYellowpaper.SerializedCollections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIEnum defaultLayer;
    public float layoutDuration = 0.3f;

    [SerializeField] public SerializedDictionary<UIEnum, GameObject> listUI = new SerializedDictionary<UIEnum, GameObject>();

    void Awake()
    {
        ServiceLocator.Register(this);
        foreach (var key in listUI.Keys)
        {
            listUI[key].gameObject.SetActive(key == defaultLayer ? true : false);
        }
    }

    public void RegisterUI(UIEnum UIName, GameObject screen)
    {
        if (!listUI.ContainsKey(UIName))
            listUI.Add(UIName, screen);
    }

    public GameObject ShowUI(UIEnum UIName)
    {
        if (listUI.ContainsKey(UIName))
        {
            listUI[UIName].SetActive(true);
            return listUI[UIName];
        }
        return null;
    }

    public GameObject HideUI(UIEnum UIName)
    {
        if (listUI.ContainsKey(UIName))
        {
            listUI[UIName].SetActive(false);
            return listUI[UIName];
        }
        return null;
    }

    public GameObject ShowUIAndHideOther(UIEnum UIName)
    {
        foreach (var screen in listUI.Values)
        {
            screen.SetActive(false);
        }

        if (listUI.ContainsKey(UIName))
        {
            listUI[UIName].SetActive(true);
            return listUI[UIName];
        }
        return null;
    }

    public GameObject GetUI(UIEnum UIName)
    {
        if (listUI.ContainsKey(UIName))
        {
            return listUI[UIName];
        }
        return null;
    }
}
