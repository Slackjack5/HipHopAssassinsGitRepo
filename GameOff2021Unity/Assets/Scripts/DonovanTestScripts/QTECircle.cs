using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTECircle : MonoBehaviour
{
    public GameObject timingRing;
    public GameObject startingPosition;
    public GameObject endingPosition;
    private float ringMaxScale = 1.5f;
    private float ringMinScale = .85f;

    //Starting Lerp Value
    static float t = 0.0f;
    private float ringDuration = 5f;
    private float time;
    private int destructionDelay;

    public int destructionDelayMax = 10;

    // Start is called before the first frame update
    private bool start;

    void Start()
    {
        timingRing.transform.localScale = new Vector3(ringMaxScale, ringMaxScale, ringMaxScale);
        time = ringDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (start == true)
        {
            // .. and increase the t interpolater
            time -= Time.deltaTime;
            t = Mathf.InverseLerp(0, ringDuration, time);
            //timingRing.transform.localScale = new Vector3(Mathf.Lerp(ringMinScale, ringMaxScale, t), Mathf.Lerp(ringMinScale, ringMaxScale, t), Mathf.Lerp(ringMinScale, ringMaxScale, t));

            if (time <= 0)
            {
                if (destructionDelay <= destructionDelayMax)
                {
                    destructionDelay += 1;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void StartQTE(float Leaniancy)
    {
        if (start == false)
        {
            ringDuration = Leaniancy;
            time = ringDuration;
            timingRing.transform.localScale = new Vector3(ringMaxScale, ringMaxScale, ringMaxScale);
            start = true;
        }
    }
}