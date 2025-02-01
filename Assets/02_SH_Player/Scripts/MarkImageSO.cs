using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkImageSO", menuName = "MarkImageSO")]
public class MarkImageSO : ScriptableObject
{
    [Serializable]
    public class MarkImage
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite image;

        public string Name { get { return name; } }
        public Sprite Image { get { return image; } }
    }

    [SerializeField] private List<MarkImage> markImageList = new();

    public IReadOnlyList<MarkImage> MarkImageList { get { return markImageList; } }
}
