using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Uninitialized,
    Customer,
    MyStore,
    OpponentStore
}

public enum DecisionType
{
    None,       // Store tiles must have this state
    Player,
    Opponent,
    Both
}

public enum VipType
{
    None,
    MyStore,
    OpponentStore
}

public enum SpecialTileType
{
    None = 1,
    HighOrder = 2,
    LowOrder = 0,
    OccasionalHighOrder = 3,
    RandomOrder = 4
}

public class Tile : MonoBehaviour
{
    private const int HighOrderCount = 20;
    private const int LowOrderCount = 6;
    private const int OccasionalOrderPeriod = 5;
    
    private SpecialTileType _specialType = SpecialTileType.None;
    
    private SpriteRenderer _spriteRenderer;
    
    public static readonly List<Tile> AllTiles = new();
    
    public int Q, R;
    
    public int S => -(Q + R);

    [HideInInspector]
    public VipType Vip = VipType.None;
    [HideInInspector]
    public bool IsPreferPermanent = false;

    public static TileTooltip Ttt;
    
   public SpecialTileType SpecialType
    {
        get => _specialType;
        set
        {
            _specialType = value;
            SetSpacialTileValues();
        }
    }

    public int PurchaseCount { get; set; } = 10;
    public decimal MaximumPrice { get; set; } = 20m;
     public TileType Type { get; set; }
     public DecisionType Decision { get; set; } = DecisionType.None;

    private static Vector2 C_VECTOR = new(0.86602540378f, 0);
    private static Vector2 R_VECTOR = new(-0.43301270189f, -0.75f);
    
    public void Init(int q, int r, TileType type)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Q = q;
        R = r;
        Type = type;

        var col = q + (r - (r & 1) / 2);

        transform.localPosition = col * C_VECTOR + r * R_VECTOR;
    }

    public void SetSpacialTileValues()
    {
        switch (SpecialType)
        {
            case SpecialTileType.None:
            case SpecialTileType.RandomOrder:
                break;
            
            case SpecialTileType.HighOrder:
                PurchaseCount = HighOrderCount;
                break;
            
            case SpecialTileType.LowOrder:
                PurchaseCount = LowOrderCount;
                break;
            
            case SpecialTileType.OccasionalHighOrder:
                PurchaseCount = (GameManager.Instance.Weeks % OccasionalOrderPeriod == 0) ? HighOrderCount : LowOrderCount;
                break;
            
            default:
                throw new ArgumentException();
        }
    }


    /* Helpers */
    public static Tile FindTile(int q, int r)
    {
        return AllTiles.Find(x => (x.Q == q && x.R == r));
    }

    public static void ShuffleTileList()
    {
        System.Random rng = new();
        int n = AllTiles.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (AllTiles[n], AllTiles[k]) = (AllTiles[k], AllTiles[n]);
        }
    }

    public static int GetDistance(Tile a, Tile b)
    {
        return Mathf.Max(Mathf.Abs(a.Q - b.Q), Mathf.Abs(a.R - b.R), Mathf.Abs(a.S - b.S));
    }

    private void OnMouseUpAsButton()
    {
        if (!GameManager.Instance.IsStorePositioned)
        {
            GameManager.Instance.MakeStore(this);
        }
    }

    private void OnMouseEnter()
    {
        void SwitchSpecialType(SpecialTileType stt)
        {
            switch (stt) 
            {
            }
        }

        Ttt.gameObject.SetActive(true);
        Ttt.SetPosition(transform.position, R);
        
        switch (Type)
        {
        case TileType.OpponentStore:
            Ttt.SetText("상대 가게", string.Empty);
            break;
        case TileType.MyStore:
            Ttt.SetText("우리 가게", string.Empty);
            break;
        case TileType.Customer:
        default:
            switch (_specialType)
            {
                case SpecialTileType.RandomOrder:
                    Ttt.SetText("부잣집", "치킨을 10마리 구매합니다. 가격은 고려하지 않고 구매합니다.");
                    break;
                case SpecialTileType.OccasionalHighOrder:
                    Ttt.SetText("교회", "치킨을 6마리 구매합니다. 5주에 1번은 치킨을 20마리 구매합니다.");
                    break;
                case SpecialTileType.LowOrder:
                    Ttt.SetText("저밀도 지구", "치킨을 6마리 구매합니다.");
                    break;
                case SpecialTileType.HighOrder:
                    Ttt.SetText("고밀도 지구", "치킨을 20마리 구매합니다.");
                    break;
                default:
                    Ttt.SetText("일반 지구", "치킨을 10마리 구매합니다.");
                    break;
            }
            break;
        //default:
            //Ttt.gameObject.SetActive(false);
            //break;
        }
    }

    private void OnMouseExit()
    {
        Ttt.gameObject.SetActive(false);
    }

    public void UpdateSprite()
    {
        switch (Type)
        {
            case TileType.Uninitialized:
            case TileType.Customer:
                var spriteIndex = (int)SpecialType;
                
                switch (Decision)
                {
                    case DecisionType.None:
                        _spriteRenderer.sprite = GameManager.Instance.normalTileSprites[spriteIndex];
                        break;
                    
                    case DecisionType.Player:
                        _spriteRenderer.sprite = GameManager.Instance.playerTileSprites[spriteIndex];
                        break;
                    
                    case DecisionType.Opponent:
                        _spriteRenderer.sprite = GameManager.Instance.enemyTileSprites[spriteIndex];
                        break;
                    
                    case DecisionType.Both:
                        _spriteRenderer.sprite = GameManager.Instance.bothTileSprites[spriteIndex];
                        break;
                    
                    default:
                        throw new ArgumentException();
                }
                break;
            
            case TileType.MyStore:
                _spriteRenderer.sprite = GameManager.Instance.playerStoreTileSprite;
                break;
            
            case TileType.OpponentStore:
                _spriteRenderer.sprite = GameManager.Instance.enemyStoreTileSprite;
                break;
            
            default:
                throw new ArgumentException();
        }
    }
}
