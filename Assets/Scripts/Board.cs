using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Card[,] cardLayout;
    private Card[] selectedCards;

    [SerializeField]
    private Card cardPrefab;
    [SerializeField]
    private GameObject linePrefab;

    [SerializeField]
    private Camera mainCam;

    Queue<Point> q = new Queue<Point>();
    List<Point> visitOrder = new List<Point>();
    List<Point> visit = new List<Point>();

    private int[,] dir = {{ 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }};

    private const int MAP_SIZE = 4;

    private bool SameCard => selectedCards[0].CardSO.id == selectedCards[1].CardSO.id;


    private void Awake()
    {
        PoolManager.CreatePool<Line>(linePrefab, transform, 10);
    }

    private void Start()
    {
        cardLayout = new Card[MAP_SIZE + 2, MAP_SIZE + 2]; //맵은 1부터 시작

        for (int i = 0; i < cardLayout.GetLength(0); i++)
        {
            for (int j = 0; j < cardLayout.GetLength(1); j++)
            {
                Card card = Instantiate(cardPrefab, new Vector2(j * 1.2f, -i * 1.2f), Quaternion.identity);
                card.transform.SetParent(transform);
                card.Init(CardManager.Instance.GetTestCardSO(0));
                if (i == 0 || j == 0 || i >= cardLayout.GetLength(0) - 1 || j >= cardLayout.GetLength(1) - 1)
                {
                    card.GetComponent<SpriteRenderer>().color = Color.black;
                }

                if (j == 2 && i == 1) card.GetComponent<SpriteRenderer>().color = Color.red;

                if (i == 4 && j == 4) card.GetComponent<SpriteRenderer>().color = Color.blue;

                cardLayout[j, i] = card;
            }
        }

        selectedCards = new Card[2];

        cardLayout[2, 1].Init(CardManager.Instance.GetTestCardSO());
        cardLayout[4, 4].Init(CardManager.Instance.GetTestCardSO());
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            Collider2D col = Physics2D.OverlapPoint(mousePos);

            if(col.CompareTag("Card"))
            {
                Card card = col.GetComponent<Card>();

                if (card.CardSO.id == -1) return;

                if(selectedCards[0] != null)
                {
                    selectedCards[1] = card;

                    //검사
                    if(SameCard)
                    {
                        (int startX, int startY) = GetTargetXY(selectedCards[0]);
                        (int nextX, int nextY) = GetTargetXY(selectedCards[1]);

                        FindPath(startX, startY, nextX, nextY);
                    }

                    for (int i = 0; i < selectedCards.Length; i++)
                    {
                        selectedCards[i] = null;
                    }
                }
                else
                {
                    selectedCards[0] = card;
                }
            }
        }
    }

    public void InitSelectedCard()
    {
        for (int i = 0; i < selectedCards.Length; i++)
        {
            selectedCards[i] = null;
        }
    }

    public (int,int) GetTargetXY(Card card)
    {
        for (int i = 1; i < cardLayout.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < cardLayout.GetLength(1) - 1; j++)
            {
                if (cardLayout[j, i] == card) return (j, i);
            }
        }

        return (-1, -1);
    }

    public void FindPath(int startX,int startY, int targetX, int targetY)
    {
        q.Clear();
        visit.Clear();
        visitOrder.Clear();

        Point start =  new Point(startX, startY, -1, 0);
        start.before = null;
        q.Enqueue(start);

        while (q.Count > 0)
        {
            Point p = q.Dequeue();
            visit.Add(p);

            if (p.x == targetX && p.y == targetY)
            {
                print("찾음");
                break;
            }

          
            for (int i = 0; i < dir.GetLength(0); i++)
            {
                if (p.dir != -1 && Mathf.Abs(p.dir - i) == 2) continue;

                int nextX = p.x + dir[i, 0];
                int nextY = p.y + dir[i, 1];
                int nextCnt = (p.dir == i || p.dir == -1) ? p.turnCount : p.turnCount + 1; //방향이 다를 시

                if (nextX < 0 || nextY < 0 || nextX >= MAP_SIZE + 2 || nextY >= MAP_SIZE + 2 
                    || (cardLayout[nextX, nextY].CardSO.id != -1 && cardLayout[nextX,nextY].CardSO.id != cardLayout[targetX,targetY].CardSO.id)) continue;

                if(nextCnt <= 2)
                {
                    Point curPoint = new Point(nextX, nextY, i, nextCnt);
                    curPoint.before = p;
                    q.Enqueue(curPoint);
                  
                }
            }
        }

        Point last = visit[visit.Count - 1];

        visitOrder.Add(last);

        int cnt = 0;
        while (last.before != null)
        {
            last = last.before;
            visitOrder.Add(last);
            cnt++;
            if(cnt >= 10000)
            {
                Debug.LogError("asd");
                break;
            }
        }

        visitOrder.Reverse();
        Line line = PoolManager.GetItem<Line>();
        line.lineRenderer.positionCount = visitOrder.Count;

        for (int i = 0; i < visitOrder.Count; i++)
        {
            line.lineRenderer.SetPosition(i, new Vector3(visitOrder[i].x * 1.2f, visitOrder[i].y * -1.2f, -1));
        }
       
    }
}
