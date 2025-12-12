using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public bool isAlive = true;
    public Vector2 spawnPoint;
    public float spawnRotation;
    public bool active = true;

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnTakeDamage(damage);
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void OnTakeDamage(int damage){ }

    public void ResetHealth()
    {
        health = maxHealth;
    }

    public virtual void Respawn()
    {
        ResetHealth();
        isAlive = true;
        transform.position = spawnPoint;
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);
        OnRespawn();
    }

    public virtual void OnRespawn(){ }

    public void Die()
    {
        isAlive = false;
        OnDeath();
    }

    public abstract void OnDeath();
}
