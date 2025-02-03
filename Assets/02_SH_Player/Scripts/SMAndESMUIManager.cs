using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMAndESMUIManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] RectTransform eSMListContent;

    List<Image> ownedESMList = new();
}
