using System.Collections.Generic;
using UnityEngine;

public class FoodMeter : MonoBehaviour
{
    [SerializeField] private Cat player;
    List<Transform> images;

    void Start()
    {
        images = new List<Transform>();
        foreach (Transform t in transform)
        {
            images.Add(t);
            t.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].gameObject.SetActive(i == player.nFlies);
        }
    }
}
