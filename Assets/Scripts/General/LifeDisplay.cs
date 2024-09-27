using UnityEngine;

public class LifeDisplay : MonoBehaviour
{
    public int lives = 0;

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(i < lives);
            i++;
        }
    }
}
