using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] DummyBodyPart[] allParts;
    [Space]
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private float _timeToReset = 3f;

    private int _totalDamageDone;
    private int _lastDamageDone;
    private float _timer;
    private bool hasBeenShot = false;

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
        if(_timer > 0) 
        {
            _timer -= Time.deltaTime;
        }

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

    public void DoDamage(int addDamage)
    {
        _lastDamageDone = addDamage;
        _totalDamageDone += _lastDamageDone;

        _timer = _timeToReset;
        hasBeenShot = true;

        //set damage text
        _damageText.text = $"{_totalDamageDone} ({_lastDamageDone})";
    }
}
