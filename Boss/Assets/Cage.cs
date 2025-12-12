using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Cage : MonoBehaviour
{
    public List<Hunter> _hunters;
    public Monster monster;

    public bool finished = false;
    public float cageDuration = 9000f; // 15 minutes
    public float timer = 0f;
    public int cageGrade = 0;

    private void Update()
    {
        if (finished) return;
        timer += Time.deltaTime;
        if (timer > cageDuration)
        {
            Finish();
            cageGrade -= 50;
            return;
        }

        // Hunter win condition
        if (!monster.isAlive)
        {
            Finish();
            cageGrade += 100;
            return;
        }

        // Monster win condition
        foreach (var hunter in _hunters)
        {
            if (hunter.isAlive)
            {
                return;
            }
        }
        Finish();
    }

    public void Finish()
    {
        finished = true;
        monster.active = false;
        foreach (var hunter in _hunters)
        {
            hunter.active = false;
        }
    }

    public void ResetCage()
    {
        monster.active = true;
        monster.Respawn();
        foreach (var hunter in _hunters)
        {
            hunter.active = true;
            hunter.Respawn();
            if(hunter.TryGetComponent(out HunterBrain brain))
            {
                brain.chromosome.Reset();
            }
        }
        timer = 0;
        cageGrade = 0;
        finished = false;
    }

    public int GetGrade()
    {
        int grade = cageGrade;

        foreach (var hunter in _hunters)
        {
            if (hunter.TryGetComponent(out HunterBrain brain))
            {
                grade += brain.chromosome.grade;
            }
        }
        return grade;
    }
}
