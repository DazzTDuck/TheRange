using System.Collections;
using TMPro;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    #region variables

    [SerializeField] DummyBodyPart[] allParts;
    [Space]
    [SerializeField] private float _damageToPointsMultiplier;
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private float _timeToReset = 3f;

    private int _totalDamageDone;
    private int _lastDamageDone;
    private float _timer;
    private bool hasBeenShot = false;

    #endregion

    private void Start()
    {
        //setup all parts
        foreach (var part in allParts)
        {
            part.SetDummy(this);
        }
    }

    private void Update()
    {
        //count down time if timer has value
        if(_timer > 0) 
        {
            _timer -= Time.deltaTime;
        }

        //if timer ran out reset damage display
        if(_timer < 0 && hasBeenShot)
        {
            //reset everything
            _timer = 0;
            hasBeenShot = false;

            _totalDamageDone = 0;
            _lastDamageDone = 0;

            _damageText.text = "";
        }

        //enable text when damage is done
        _damageText.enabled = hasBeenShot;
    }

    /// <summary>
    /// This handles the damage done to the dummy and displays it
    /// </summary>
    /// <param name="addDamage">damage done to the dummy</param>
    public void DoDamage(int addDamage)
    {
        _lastDamageDone = addDamage;
        _totalDamageDone += _lastDamageDone;

        _timer = _timeToReset;
        hasBeenShot = true;

        //set damage text
        _damageText.text = $"{_totalDamageDone} ({_lastDamageDone})";

        //add points to game manager, points based on damage
        GameManager.Instance.AddPoints(Mathf.RoundToInt(_lastDamageDone * _damageToPointsMultiplier));
    }
}
