using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AudioContainer", order = 1)]

public class AudioContainer : ScriptableObject
{
    public AudioInfo[] audioInfos;
}
