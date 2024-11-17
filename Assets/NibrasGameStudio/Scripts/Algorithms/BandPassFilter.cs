using UnityEngine;

public class BandPassFilter
{
    private Vector3 prevLPOutput = Vector3.zero;    

    private Vector3[] LowPassFilter(Vector3[] input, float dt, float RC=0.1F) // from Wikipedia, https://en.wikipedia.org/wiki/Low-pass_filter
    {
        Vector3[] output = new Vector3[input.Length];
        float alpha = dt / (RC + dt);
        output[0] = input[0] * alpha;
        for (int i = 1; i < input.Length; i++)
        {
            output[i] = (alpha * input[i]) + ((1 - alpha) * output[i - 1]);
        }
        return output;
    }

    private Vector3 prevHPOutput = Vector3.zero;

    private Vector3[] HighPassFilter(Vector3[] input, float dt, float RC = 0.1F) // from Wikipedia, https://en.wikipedia.org/wiki/High-pass_filter
    {
        Vector3[] output = new Vector3[input.Length];
        float alpha = RC / (RC + dt);
        output[0] = input[0] * alpha;
        for (int i = 1; i < input.Length; i++)
        {
            output[i] = (alpha * output[i-1]) + (alpha * (input[i] - output[i - 1]));
        }
        return output;
    }

    public Vector3 Filter(Vector3 input, float dt, float LPFilterStrength=0.1F, float HPFilterStrength=0.9F)
    {
        Vector3[] LPFilterInput = new Vector3[] { prevLPOutput, input };
        Vector3[] LPFilterOutput = LowPassFilter(LPFilterInput, dt, LPFilterStrength);
        prevLPOutput = LPFilterOutput[1];
        Vector3[] HPFilterInput = new Vector3[] { prevHPOutput, LPFilterOutput[1]};
        Vector3[] HPFilterOutput = HighPassFilter(HPFilterInput, dt, HPFilterStrength);
        prevHPOutput = HPFilterOutput[1];
        return HPFilterOutput[1];
    }
}