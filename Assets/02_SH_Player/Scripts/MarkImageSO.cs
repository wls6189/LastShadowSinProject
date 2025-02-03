using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkImageSO", menuName = "MarkImageSO")]
public class MarkImageSO : ScriptableObject
{
    [Serializable]
    public class ESMImage
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite image;

        public string Name { get { return name; } }
        public Sprite Image { get { return image; } }
    }

    [SerializeField] private List<ESMImage> eSMImageList = new();

    public IReadOnlyList<ESMImage> ESMImageList { get { return eSMImageList; } }

    [SerializeField]
    public class SpiritMarkImage
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite image;

        public string Name { get { return name; } }
        public Sprite Image { get { return image; } }
    }

    [SerializeField] private List<ESMImage> spiritMarkImageList = new();

    public IReadOnlyList<ESMImage> SpiritMarkImageList { get { return spiritMarkImageList; } }

    [SerializeField]
    public class MarkImage
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite image;

        public string Name { get { return name; } }
        public Sprite Image { get { return image; } }
    }

    [SerializeField] private List<ESMImage> markImageList = new();

    public IReadOnlyList<ESMImage> MarkImageList { get { return markImageList; } }
}
