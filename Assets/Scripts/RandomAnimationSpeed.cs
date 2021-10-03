using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
    private Animator _animator;
    public float MaxValue = .1f;
    public float MinValue = .1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = UnityEngine.Random.Range(MinValue, MaxValue);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
