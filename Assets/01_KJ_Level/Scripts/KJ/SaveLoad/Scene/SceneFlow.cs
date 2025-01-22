using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneFlow", menuName = "ScriptableObjects/SceneFlow", order = 0)]
public class SceneFlow : ScriptableObject
{
    public string currentSceneName;                                // æ¿ ¿Ã∏ß
    public string previousScene;
    public string nextScene;



}
