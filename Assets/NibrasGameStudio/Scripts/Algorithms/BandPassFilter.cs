using UnityEngine;

public class BandPassFilter
{
    private Vector3 prevLPOutput = Vector3.zero;
    private Vector3 prevHPOutput = Vector3.zero;

    public Vector3 Filter(Vector3 input, float LPFilterStrength=0.1F, float HPFilterStrength=0.9F)
    {
        Vector3 LPFiltered = Vector3.Lerp(prevLPOutput, input, LPFilterStrength); //removing HF noise
        prevLPOutput = LPFiltered;
        Vector3 HPFiltered = (input - prevLPOutput) * HPFilterStrength + prevHPOutput; //removing LF noise
        prevHPOutput = HPFiltered;
        return HPFiltered;
    }
}
