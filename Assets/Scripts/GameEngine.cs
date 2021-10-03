using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    public Slider Balance;
    public PlayerController PlayerController;

    private int _previousRegion;
    private float _regionTime;

    private int _score;
    private int _previousDeltaScore;
    private int _previousDeltaDamage;
    public Text ScoreUI;

    private float _initialTimeToHitInterval = 3f;
    private float _timeToHitInterval = 0f;
    private float _elapsedTimeToHit = 0f;

    private bool hasStarted = false;

    public List<BalanceRegion> BalanceRegions = new List<BalanceRegion>();

    Coroutine _coroutine;
    // Start is called before the first frame update
    void Start()
    {
        _timeToHitInterval = _initialTimeToHitInterval;
        // NB (lac): manual init for the PlayerCOntroller, which sets the Balance values, as we can't rely on the Start's non-deterministic order
        PlayerController.Initialize();

        // NB (lac): 10%
        float minBalanceForHit = 10 * Balance.maxValue / 100;

        // NB (lac): 90%
        float maxBalanceForHit = 9 * 10 * Balance.maxValue / 100;

        int validInterval = (int)(Balance.maxValue - 2 * minBalanceForHit);

        validInterval = validInterval / 3;

        /*

        BalanceRegions = new List<BalanceRegion>()
        {
            new BalanceRegion{index = 0,
                minValue=0,
                maxValue = (int)minBalanceForHit,
                damage = 1,
            },
            new BalanceRegion{index = 1,
                minValue=(int)minBalanceForHit+1,
                maxValue=(int)minBalanceForHit+1+validInterval,
                score = 1,
            },
            new BalanceRegion{index = 2,
                minValue=(int)minBalanceForHit+1+validInterval+1,
                maxValue=(int)minBalanceForHit+1+validInterval+1+validInterval,
                score = 1,
            },
            new BalanceRegion{index = 3,
                minValue=(int)minBalanceForHit+1+validInterval+1+validInterval+1,
                maxValue=(int)minBalanceForHit+1+validInterval+1+validInterval+1+validInterval,
                score = 1,
            },
            new BalanceRegion{index = 4,
                minValue=(int)maxBalanceForHit,
                maxValue=(int)Balance.maxValue,
                damage = 1,
            },
        };
        */
    }

    private void Update()
    {
        //NB (lac): it will never be false again after turning to true :)
        hasStarted |= PlayerController.HasInput();
        if(_coroutine == null)
            _coroutine = StartCoroutine("SetStance");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hasStarted)
        {
            handleUpdateScoreUI();
            return;
        }
        CalculateTimeHit(Time.fixedDeltaTime);
        handleCheckBalance(Time.fixedDeltaTime);
        handleUpdateScoreUI();
    }

    void handleCheckBalance(float elapsedTime)
    {
        int region = getBalanceRegion(Balance.value);

        if (_previousRegion == region)
        {
            _regionTime += elapsedTime;
        }
        else
            _regionTime = 0f;

        CalculateDamage(_regionTime, BalanceRegions[region].damage);

        if(PlayerController.GetPlayerStance() == 0 && region == 2) // NB (lac): Mabu stance, middle region, 2/5
        {
            CalculateScore(_regionTime, BalanceRegions[region].score);
        }

        _previousRegion = region;
    }

    void CalculateTimeHit(float elapsedTime)
    {
        _elapsedTimeToHit += elapsedTime;
        if (_elapsedTimeToHit > _timeToHitInterval)
        {
            PlayerController.TakeHit(PlayerController.MaxBalanceSpeed * 1);
            _elapsedTimeToHit = 0f;

            //  NB (lac): decrease the interval, up to each second;
            _timeToHitInterval--;
            if (_timeToHitInterval <= 0)
                _timeToHitInterval = 1;
        }
    }

    void CalculateDamage(float totalTime, int baseDamage)
    {
        int deltaDamage = (int)totalTime * baseDamage;
        if (_previousDeltaDamage != deltaDamage)
        {
            PlayerController.TakeHit(PlayerController.MaxBalanceSpeed * 1);
            // NB (lac): increase speed
            PlayerController.MaxBalanceSpeed -= 1;
            if (PlayerController.MaxBalanceSpeed <= 0)
                PlayerController.MaxBalanceSpeed = 1;
        }
        _previousDeltaDamage = deltaDamage;
    }

    void CalculateScore(float totalTime, int baseScore)
    {
        int deltaScore = (int)totalTime * baseScore;        
        if (_previousDeltaScore != deltaScore)
        {
            _score += deltaScore;
            // NB (lac): increase speed
            PlayerController.MaxBalanceSpeed += 1;
            _elapsedTimeToHit = 0f;
            //  NB (lac): reset the timeToHit interval;
            _timeToHitInterval = _initialTimeToHitInterval;
        }
        _previousDeltaScore = deltaScore;
    }

    void handleUpdateScoreUI()
    {
        ScoreUI.text = _score.ToString();
    }

    int getBalanceRegion(float value)
    {
        for(int i = 0; i < BalanceRegions.Count;i++)
        {
            if (value >= BalanceRegions[i].minValue
                && value <= BalanceRegions[i].maxValue)
                return BalanceRegions[i].index;
        }
        throw new ApplicationException(String.Format("value {0} not available in any region", value));
    }

    IEnumerator SetStance()
    {
        yield return new WaitForSeconds(5f);
        int stance = UnityEngine.Random.Range(-1, 3);
        if(stance >= 0)
        {
            Debug.Log("Changing stance");
            PlayerController.SetStance(stance);
        }
        _coroutine = null;
    }

}
