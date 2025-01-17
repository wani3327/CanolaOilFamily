using System.Collections.Generic;
using System.Linq;

public class Store
{
    public decimal BaseRent = 300m;

    private decimal _price;

    public decimal Price
    {
        get => _price;
        set
        {
            if (value is < 0.5m or > 20)
            {
                return;
            }

            _price = value;

            if (this == GameManager.Instance.Player)
                GameManager.Instance.UpdatePriceUI();
        }
    }

    private decimal _money;

    public decimal Money
    {
        get { return _money; }
        set
        {
            _money = value;
            GameManager.Instance.UpdateMoneyUI(this);
        }
    }

    private decimal _delivFee;
    public decimal DeliveryFee => _delivFee;

    private decimal _ingCost;
    public decimal IngredientCost => _ingCost;

    public decimal Rent => BaseRent - Upgrade.RentCostDecrement;

    public Tile Position;

    public int Stock => Level switch
    {
        1 => 100,
        2 => 200,
        3 => 300,
        4 => 400,
        _ => 50
    };

    public int SaleVolume = 0; // Weekly
    public decimal Profit = 0; // Weekly
    public int Level = 0;
    private List<Upgrade> _upgrades = new();
    public UpgradeStat Upgrade = new();
    public ItemManager ItemManager;

    public Store()
    {
        ItemManager = new(this);
        _price = 15;
    }

    // This process should NOT be happened in constructor since it needs to occur UI text change.
    public void InitValues()
    {
        Price = 18;
        Money = 300;
        _delivFee = 1.5m;
        _ingCost = 10;
        GameManager.Instance.UpdateUpgradableStatUI();
    }

    public bool HasUpgrade(Upgrade upgrade) => _upgrades.Contains(upgrade);

    public bool IsNextUpgrade(Upgrade upgrade) =>
        upgrade.UpgradeConstraint == null ||
        _upgrades.Any(upg => upg.GetType() == upgrade.UpgradeConstraint);

    public bool IsUpgradeAvailable(Upgrade upgrade) =>
        upgrade.LvConstraint <= Level && (upgrade.UpgradeConstraint == null ||
                                          _upgrades.Any(upg => upg.GetType() == upgrade.UpgradeConstraint)) &&
        Money >= upgrade.Price;

    public bool BuyUpgrade(Upgrade upgrade)
    {
        if (!IsUpgradeAvailable(upgrade)) return false;
        Money -= upgrade.Price;
        if (upgrade.UpgradeConstraint != null)
        {
            var index = _upgrades.FindIndex(upg => upg.GetType() == upgrade.UpgradeConstraint);
            if (index < 0) return true;
            if (upgrade.IsReplaceConstraint)
                _upgrades.RemoveAt(index);
        }
        _upgrades.Add(upgrade);
        Upgrade = _upgrades.Aggregate(new UpgradeStat(), (stat, u) => stat + u.Stat);
        GameManager.Instance.UpdateUpgradableStatUI();
        return true;
    }


    public override string ToString()
    {
        return this == GameManager.Instance.Player ? "Player" : "Enemy";
    }

    public Store GetEnemy() => GameManager.Instance.FindMyEnemy(this);
}