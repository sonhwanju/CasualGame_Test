using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    private List<CardSO> cardSOList;

    [SerializeField]
    private List<Card> cardList;

    private void Awake()
    {
        Instance = this;

        cardSOList = Resources.LoadAll<CardSO>("CardSO/").ToList();
    }

    public CardSO GetTestCardSO(int idx = 1)
    {
        return cardSOList[idx];
    }
}
