using UnityEngine;
using NaughtyAttributes;

public class Student : MonoBehaviour
{
    [SerializeField] private Color  colorSmart = Color.white;
    [SerializeField] private Sprite spriteSmart;

    public bool smart = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeSmart()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.color = colorSmart;
        sr.sprite = spriteSmart;

        smart = true;
    }
}
