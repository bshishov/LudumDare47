using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UIPlayerPointer : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPointerUp;
        [SerializeField] private GameObject _playerPointerDown;

        private UILevelInCenterOfMap _uILevelInCenterOfMap;

        private void Awake()
        {
            _playerPointerUp.SetActive(false);
            _playerPointerDown.SetActive(false);
        }

        private void Start()
        {
            _uILevelInCenterOfMap = FindObjectOfType<UILevelInCenterOfMap>();

            _playerPointerUp.GetComponent<Button>().onClick.AddListener(ShowLastLevelIncenter);
            _playerPointerDown.GetComponent<Button>().onClick.AddListener(ShowLastLevelIncenter);
        }

        private void ShowLastLevelIncenter()
        {
            _uILevelInCenterOfMap.SetLevelIncenter();
            HidePointer();
        }

        public void ShowUpPointer()
        {
            _playerPointerUp.SetActive(true);
            _playerPointerDown.SetActive(false);
        }
        public void ShowDownPointer()
        {
            _playerPointerUp.SetActive(false);
            _playerPointerDown.SetActive(true);
        }

        public void HidePointer()
        {
            _playerPointerUp.SetActive(false);
            _playerPointerDown.SetActive(false);
        }
    }
}