using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public bool isAlive = true;
    public Vector2 spawnPoint;
    public float spawnRotation;
    public bool active = true;

    // Method to apply damage to the entity
    public void TakeDamage(int damage)
    {
        health -= damage;
        OnTakeDamage(damage);
        if (health <= 0)
        {
            Die();
        }
    }

    // Hook for additional behavior when taking damage
    public virtual void OnTakeDamage(int damage){ }

    // Method to reset health to maximum
    public void ResetHealth()
    {
        health = maxHealth;
    }

    // Method to respawn the entity at its spawn point
    public virtual void Respawn()
    {
        ResetHealth();
        isAlive = true;
        transform.position = spawnPoint;
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);
        OnRespawn();
    }

    // Hook for additional behavior upon respawn
    public virtual void OnRespawn(){ }

    // Method to handle entity death
    public void Die()
    {
        isAlive = false;
        OnDeath();
    }

    // Hook for additional behavior upon death
    public abstract void OnDeath();
}
