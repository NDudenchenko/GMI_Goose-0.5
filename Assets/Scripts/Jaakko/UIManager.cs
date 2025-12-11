using TMPro;
using System;
using UnityEngine;
using Cysharp.Text;

namespace AG3958
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _pointsText;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _manaText;
        [SerializeField] private GameObject _loseText;
        [SerializeField] private GameObject _winText;
        private PlayerCore _playerCore;

        private void Awake()
        {
            PlayerCore.PointChangeEvent += UpdatePoints;
            PlayerCore.HealthChangeEvent += UpdateHealth;
            PlayerCore.ManaChangeEvent += UpdateMana;
            PlayerCore.PlayerDeathEvent += OnPlayerDeath;
            PlayerCore.PlayerRespawnEvent += OnPlayerRespawn;
            GameStateManager.WinGameEvent += OnWinGame;
        }

        private void Start()
        {
            _playerCore = FindFirstObjectByType<PlayerCore>();
            ResetValues();
        }

        private void UpdatePoints(float value)
        {
            using var sb = ZString.CreateStringBuilder();
            sb.Append("Score: " + (Single.Parse(_pointsText.text.Substring(7)) + value));
            _pointsText.SetText(sb);
        }

        private void UpdateHealth(float value, bool unused)
        {
            float hpVal = (Single.Parse(_healthText.text.Substring(4)) + value);
            if (hpVal > _playerCore.MaxHealth) { hpVal = _playerCore.MaxHealth; }
            else if (hpVal < 0) { hpVal = 0; }
            using var sb = ZString.CreateStringBuilder();
            sb.Append("HP: " + hpVal);
            _healthText.SetText(sb);
        }

        private void UpdateMana(float value)
        {
            float mpVal = (Single.Parse(_manaText.text.Substring(4)) + value);
            if (mpVal > _playerCore.MaxMana) { mpVal = _playerCore.MaxMana; }
            else if (mpVal < 0) { mpVal = 0; }
            using var sb = ZString.CreateStringBuilder();
            sb.Append("MP: " + mpVal);
            _manaText.SetText(sb);
        }

        private void ResetValues()
        {
            using var sb = ZString.CreateStringBuilder();
            sb.Append("Score: " + _playerCore.PlayerPoints);
            _pointsText.SetText(sb);
            sb.Clear();
            sb.Append("HP: " + _playerCore.PlayerHealth);
            _healthText.SetText(sb);
            sb.Clear();
            sb.Append("MP: " + _playerCore.PlayerMana);
            _manaText.SetText(sb);
        }

        private void OnPlayerDeath()
        {
            _loseText.SetActive(true);
        }

        private void OnPlayerRespawn()
        {
            _loseText.SetActive(false);
        }

        private void OnWinGame()
        {
            _winText.SetActive(true);
        }
    }
}