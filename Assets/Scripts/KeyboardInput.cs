using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour, IGameDataInput
{
    GameInput _input;

    void Start()
    {
        _input = new GameInput();
    }

    public bool InputLeft()
    {
        return _input.InputLeft;
    }

    public bool InputRight()
    {
        return _input.InputRight;
    }

    public bool InputStart()
    {
        return _input.InputStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            _input.InputLeft = true;
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            _input.InputLeft = false;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            _input.InputRight = true;
        if (Input.GetKeyUp(KeyCode.RightArrow))
            _input.InputRight = false;
        if(Input.GetKeyDown(KeyCode.Space))
            _input.InputStart = true;
        if (Input.GetKeyUp(KeyCode.Space))
            _input.InputStart = false;
    }

    public void AutoPress()
    {
        _input.InputStart = true;
    }
}
