using UnityEngine;
using UnityEngine.Events;
using EditorAttributes;
using System;

public class PlayerCore : MonoBehaviour
{
    public UnityEvent OnHealthChanged;
    public UnityEvent OnManaChanged;
    public UnityEvent OnPointsChanged;

    [Header("Basic Stats")]
    [SerializeField, Clamp(1, Single.MaxValue)] private float _maxHealth;
    private float _currentHealth;
    public float PlayerHealth { get { return _currentHealth; } }

    [SerializeField, Clamp(1, Single.MaxValue)] private float _maxMana;
    private float _currentMana;
    public float PlayerMana { get { return _currentMana; } }

    private float _points = 0f;
    public float PlayerPoints { get { return _points; } }

    [Header("Progression Checks")]
    [SerializeField] private bool _hasSpeedBooster;
    public bool HasSpeedBooster { get { return _hasSpeedBooster; } }

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _currentMana = _maxMana;

        if (OnHealthChanged == null)
            OnHealthChanged = new UnityEvent();
    }
}
