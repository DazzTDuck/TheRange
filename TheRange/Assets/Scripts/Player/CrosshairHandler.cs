using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairHandler : MonoBehaviour
{
    #region variables 

    [SerializeField] private RectTransform[] _spreadIndicators = new RectTransform[4];
    public bool IndicateSpread { get => _indicateSpread; set { _indicateSpread = value; } }

    private bool _indicateSpread = true;
    private Vector3[] _originalPositionsSpreads = new Vector3[4];

    #endregion

    private void Start()
    {
        //set all original positions of crosshair lines
        for (int i = 0; i < _spreadIndicators.Length; i++)
        {
            _originalPositionsSpreads[i] = _spreadIndicators[i].localPosition;
        }

    }

    private void Update()
    {
        if (!_indicateSpread)
            return;

        for (int i = 0; i < _spreadIndicators.Length; i++)
        {
            //always update spread indicators (crosshair)
            if (GunHandler.Instance.GetSpreadAmount() > 0)
            {
                //i % 2, the second in the array needs to go in the negative direction
                float spreadAmount = i % 2 == 0 ? -GunHandler.Instance.GetSpreadAmount() : GunHandler.Instance.GetSpreadAmount();
                //i >= 2, the top and bottom crosshair lines need to move via the Y axis
                Vector3 newVector = i >= 2 ? new Vector3(0, spreadAmount, 0) : new Vector3(spreadAmount, 0, 0);

                //apply per indicator
                Vector3 newPos = _originalPositionsSpreads[i] + newVector;
                _spreadIndicators[i].localPosition = newPos;
            }
            else
            {
                //reset when no spread
                _spreadIndicators[i].localPosition = _originalPositionsSpreads[i];
            }
        }
    }
}
