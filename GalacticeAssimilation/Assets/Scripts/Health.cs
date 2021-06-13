using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int _maxHealth = 100;
    [SerializeField] HealthBar _healthBar;

    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar?.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        _healthBar?.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
}
