using TMPro;
using UnityEngine;

public class TeacherPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip grabBookSound;
    [SerializeField] private AudioClip smartStudentSound;

    bool hasBook = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var book = collider.GetComponent<Book>();
        if (book)
        {
            if (!hasBook)
            {
                SoundManager.PlaySound(grabBookSound);
                hasBook = true;
                Destroy(book.gameObject);
            }
            return;
        }
        if (hasBook)
        {
            var student = collider.GetComponent<Student>();
            if ((student) && (!student.smart))
            {
                SoundManager.PlaySound(smartStudentSound);
                student.MakeSmart();
                hasBook = false;
            }
        }
    }
}
