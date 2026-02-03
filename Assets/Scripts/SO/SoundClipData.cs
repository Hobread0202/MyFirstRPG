using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundClipData", menuName = "SO/SoundData")]
public class SoundClipData : ScriptableObject
{
    public List<AudioClip> audioClips;
}