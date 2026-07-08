using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
   // [SerializeField] private GameObject cameraShake;

    [SerializeField]private float duration=1f;
    [SerializeField]private float shakeMagnitude=2f;
    [SerializeField]private float dampingSpeed = 1f;
    

    private float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
    public IEnumerator ShakingTime()
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.localPosition;
        while (elapsedTime < duration)
        {
            float magnitude = shakeMagnitude * Mathf.Exp(-dampingSpeed * elapsedTime);
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(initialPosition.x+xOffset,initialPosition.y+yOffset,initialPosition.z);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialPosition;
    }
}
