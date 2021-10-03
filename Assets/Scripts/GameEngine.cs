using Assets.Scripts;
using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public AudioClip HitAudio;
    public AudioClip PowerUpAudio;
    public AudioClip PickUpAudio;
    private AudioSource _audioSource;

    Coroutine _coroutine;

    public GameObject StartPanelUI;
    public GameObject GameOverPanelUI;
    private bool _gameOver;

    public KeyboardInput _KeyboarInput;

    public Text ScoreSUI;
    public Text ScoreHSUI;
    public Text TotalTimeUI;

    private float _totalTimePlayed;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
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
        Debug.Log(String.Format("minDeadZone:{0} / maxDeadZone:{1}", minBalanceForHit, maxBalanceForHit));

        int validInterval = (int)(Balance.maxValue - 2 * minBalanceForHit);

        validInterval = validInterval / 3;


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

        foreach (BalanceRegion b in BalanceRegions)
        {
            Debug.Log(String.Format("region:{0}, min:{1}, max:{2}", b.index, b.minValue, b.maxValue));
        }
    }

    private void Update()
    {
        bool previousHasStarted = hasStarted;
        //NB (lac): it will never be false again after turning to true :)
        hasStarted |= PlayerController.HasInput();
        if(previousHasStarted != hasStarted)
        {
            CloseStartPanel();
        }

        if (PlayerController.GetLife() <= 0 && !_gameOver)
        {
            GameData gd = DataManagement.Load();
            // NB (lac): update saved data
            if (_score > gd.GetMaxScore())
                gd.SetMaxScore(_score);

            if (_totalTimePlayed > gd.GetMaxTrainingTime())
                gd.SetMaxTrainingTime((int)_totalTimePlayed);
            DataManagement.Save(gd);

            ScoreSUI.text = String.Format("Score: {0}", _score);
            ScoreHSUI.text = String.Format("Highscore: {0}", gd.GetMaxScore());
            TotalTimeUI.text = String.Format("Training time: {0}", (int)_totalTimePlayed);
            _gameOver = true;
            GameOverPanelUI.SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_gameOver)
            return;
        if (!hasStarted)
        {
            handleUpdateScoreUI();
            return;
        }

        _totalTimePlayed += Time.fixedDeltaTime;

        if (_coroutine == null && hasStarted && !_gameOver)
            _coroutine = StartCoroutine("SetStance");

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
        {
            _regionTime = 0f;
            _previousDeltaScore = 0;
        }   

        CalculateDamage(_regionTime, BalanceRegions[region].damage);

        if(PlayerController.GetPlayerStance() == 0 && region == 2   // NB (lac): Mabu stance, middle region, 3/5
            || PlayerController.GetPlayerStance() == 1 && region == 3   // NB (lac): Gongbu stance, left region, 4/5
            || PlayerController.GetPlayerStance() == 2 && region == 1   // NB (lac): Xabu stance, middle region, 2/5
            )
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

            // NB (lac): play hit sound
            _audioSource.clip = HitAudio;
            _audioSource.Play();

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

            // NB (lac): play hit sound
            _audioSource.clip = HitAudio;
            _audioSource.Play();

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
            Debug.Log(String.Format("TimeInZone:{0}", totalTime));
            // NB (lac): limit score multiplier
            if(deltaScore > 5)
            {
                _score += 5;
            }
            else
            {
                _score += deltaScore;
            }
            
            // NB (lac): increase speed
            PlayerController.IncreaseBalanceSpeed(1);
            _elapsedTimeToHit = 0f;
            //  NB (lac): reset the timeToHit interval;
            _timeToHitInterval = _initialTimeToHitInterval;

            // NB (lac): play hit sound
            _audioSource.clip = PickUpAudio;
            _audioSource.Play();
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
        if(_totalTimePlayed > 60f)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 2));
        }
        else
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 5));
        }

        int stance = UnityEngine.Random.Range(-1, 3);
        //stance = 1;
        if (stance >= 0 && stance != PlayerController.GetPlayerStance() && !_gameOver && hasStarted)
        {
            Debug.Log("Changing stance");
            PlayerController.SetStance(stance);
            _audioSource.clip = PowerUpAudio;
            _audioSource.Play();
        }
        _coroutine = null;
    }

    public void CloseStartPanel()
    {
        StartPanelUI.SetActive(false);
        hasStarted = true;
        _KeyboarInput.AutoPress();
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
