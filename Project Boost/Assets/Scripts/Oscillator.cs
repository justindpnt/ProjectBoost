using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    // Configuration Parameters
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;
    [SerializeField] float direction = 1f;

    float movementFactor; // 0 for not moved, 1 for fully moved
    Vector3 startingPos;

    // Start is called before the first frame update
    void Start() {
        startingPos = transform.position;    
    }

    // Update is called once per frame
    void Update() {
        if(period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2; // ~6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // ranges from -1 to 1

        movementFactor = rawSinWave / 2f + .5f; //shift to 0 to 1
        Vector3 offset = movementFactor * movementVector * direction;
        transform.position = startingPos + offset;
    }
}
