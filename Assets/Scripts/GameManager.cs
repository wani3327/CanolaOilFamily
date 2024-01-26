using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour // I AM SINGLETON!
{
    static public GameManager Instance;

    public Tile[][] grid;

    [SerializeField]
    private Button _simulateButton;
    [SerializeField]
    private TMPro.TMP_Text _priceText;
    [SerializeField]
    private Button _increasePrice;
    [SerializeField]
    private Button _decreasePrice;

    
    public int Weeks; // ���� ����

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //_increasePrice.onClick.AddListener(() => Price += 100);
        //_decreasePrice.onClick.AddListener(() => Price -= 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
