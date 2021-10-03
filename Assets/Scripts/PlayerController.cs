using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private IGameDataInput _input;
    public Slider Balance;
    public Slider Life;

    public int _balanceSpeed;
    public int MaxBalanceSpeed;
    private int _deltaBalance;
    private int _life;

    int MaxLife = 100;
    public List<SpriteRenderer> Stances;
    private Animator _animator;

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
        handleUpdateInputs();
        handleUpdateBalanceSpeed(Time.deltaTime);
        handleUpdateBalanceUI();
        handleLifUI();
    }

    public void Initialize()
    {
        _life = MaxLife;
        Life.maxValue = MaxLife;

        Balance.maxValue = 1000;
        Balance.value = Balance.maxValue / 2;
    }

    void handleUpdateInputs()
    {
        if (_input.InputLeft())
            _deltaBalance = -1 * _balanceSpeed;
        else if (_input.InputRight())
            _deltaBalance = 1 * _balanceSpeed;
    }

    void handleUpdateBalanceSpeed(float elapsedTime)
    {
        if (_balanceSpeed < MaxBalanceSpeed)
            _balanceSpeed += 1;
        else if (_balanceSpeed > MaxBalanceSpeed)
            _balanceSpeed -= 1;
    }

    void handleUpdateBalanceUI()
    {
        Balance.value += _deltaBalance;
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
        return 0;
    }

    public bool HasInput()
    {
        return _input.InputLeft() || _input.InputRight();
    }

    public void SetStance(int stance)
    {
        if (stance == 0)
            _animator.Play("MabuIdle");
        else if (stance == 1)
            _animator.Play("GongbuIdle");
        else if (stance == 2)
            _animator.Play("XabuIdle");
    }
}
