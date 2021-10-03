using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const float MAX_BALANCE = 1000f;
    private const int MAX_SPEED = 2;
    private IGameDataInput _input;
    public Slider Balance;
    public Slider Life;

    public int _balanceSpeed;
    public int MaxBalanceSpeed;
    private int _deltaBalance;
    private int _life;

    int MaxLife = 50;
    public List<SpriteRenderer> Stances;
    private Animator _animator;
    private int _stance = 0;

    private float _balance;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<IGameDataInput>();
    }
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetLife() <= 0)
            return;
        handleUpdateInputs();
        handleUpdateBalanceSpeed(Time.deltaTime);
        handleUpdateBalanceUI(Time.deltaTime);
        handleLifUI();
    }

    public void Initialize()
    {
        _life = MaxLife;
        Life.maxValue = MaxLife;

        Balance.maxValue = MAX_BALANCE;
        Balance.value = Balance.maxValue / 2;

        _balance = Balance.value;
    }

    void handleUpdateInputs()
    {
        if (_input.InputLeft())
            _deltaBalance = -1 * _balanceSpeed;
        else if (_input.InputRight())
            _deltaBalance = 1 * _balanceSpeed;
        else
            _deltaBalance = 0;

        _balance += _deltaBalance;
        if (_balance < 0)
            _balance = 0;
        if (_balance > MAX_BALANCE)
            _balance = MAX_BALANCE;
    }

    void handleUpdateBalanceSpeed(float elapsedTime)
    {
        if (_balanceSpeed < MaxBalanceSpeed)
            _balanceSpeed += 1;
        else if (_balanceSpeed > MaxBalanceSpeed)
            _balanceSpeed -= 1;
    }

    void handleUpdateBalanceUI(float elapsedTime)
    {
        Balance.value = _balance;
    }

    void handleLifUI()
    {
        Life.value = _life;
    }

    public void TakeHit(int value)
    {
        _life -= value;
    }

    public int GetLife()
    {
        return _life;
    }

    public int GetPlayerStance()
    {
        return _stance;
    }

    public bool HasInput()
    {
        return _input.InputLeft() || _input.InputRight() || _input.InputStart();
    }

    public void SetStance(int stance)
    {
        if (stance == 0)
            _animator.Play("MabuIdle");
        else if (stance == 1)
            _animator.Play("GongbuIdle");
        else if (stance == 2)
            _animator.Play("XabuIdle");
        _stance = stance;
    }

    public void IncreaseBalanceSpeed(int v)
    {
        MaxBalanceSpeed += v;
        if (MaxBalanceSpeed > MAX_SPEED)
            MaxBalanceSpeed = MAX_SPEED;
    }
}
