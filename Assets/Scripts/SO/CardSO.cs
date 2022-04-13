using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CardSO", menuName = "SO/Create CardSO")]
public class CardSO : ScriptableObject
{
    public int id;
    public Sprite cardSprite;
}
