using System.Transactions;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] private float amplitude = 10.0f;

    Vector3 basePos;

    void Start()
    {
        basePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = basePos + Vector3.up * amplitude * Mathf.Sin(Time.time * 5.0f);
    }
}
