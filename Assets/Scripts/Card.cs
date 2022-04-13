using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]
    private CardSO cardSO;
    public CardSO CardSO => cardSO;

    [SerializeField]
    private SpriteRenderer carRenderer;

    public void Init(CardSO so)
    {
        cardSO = so;

        //carRenderer.sprite = so.cardSprite;
    }

}
