// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace AH4063
// {
//     public class SteamLaunchForce : GlobalTimerBehaviour
//     {
//         [SerializeField] private AnimationCurve steamScaleCurve;
//         [SerializeField] private float force = 0.0f;
//         
//         [SerializeField] private Transform steamScaleObject;
//     
//         private BoxCollider2D _boxCollider;
//         private bool _isActive = false;
//         private float _currentTime;
//     
//         private void Awake()
//         {
//             GetComponent<BoxCollider2D>();
//         }
//
//         void FixedUpdate()
//         {
//             if (_isActive)
//             {
//                 _currentTime += Time.fixedDeltaTime;
//                 force = _currentTime * 0.01f;
//                 //force = steamScaleCurve.Evaluate(_currentTime);
//                 steamScaleObject.localScale += new Vector3(0, force, 0);
//             }
//             else
//             {
//                 _currentTime -= Time.fixedDeltaTime;
//                 force = _currentTime * 0.01f;
//                 //force = steamScaleCurve.Evaluate(_currentTime);
//                 steamScaleObject.localScale -= new Vector3(0, force, 0);
//             }
//         }
//
//         public IEnumerator TurnUp()
//         {
//             _currentTime += Time.fixedDeltaTime;
//             
//             
//             yield return null;
//         }
//         
//         public IEnumerator TurnDown()
//         {
//             _currentTime += Time.fixedDeltaTime;
//             
//             
//             yield return null;
//         }
//
//         protected override void OnGlobalCycle()
//         {
//             _isActive = _isActive != true;
//         }
//
//         private void OnCollisionEnter2D(Collision2D collision)
//         {
//             if (collision.gameObject.CompareTag("Player"))
//             {
//                 Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
//                 Debug.Log("Force !!!");
//                 playerRb.AddForce(new Vector2(0, 500));
//             }
//         }
//     }
// }

using System.Collections;
using UnityEngine;

namespace AH4063
{
    public class SteamLaunchForce : GlobalTimerBehaviour
    {
        [SerializeField] private AnimationCurve steamScaleCurve;
        [SerializeField] private Transform steamScaleObject;
        [SerializeField] private float baseForce = 100f;

        private BoxCollider2D _boxCollider;
        private bool _isActive = false;
        private float _currentTime = 0f;
        private float _curveVar = 0f;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void FixedUpdate()
        {
            if (_isActive)
                _currentTime += Time.fixedDeltaTime;
            else
                _currentTime -= Time.fixedDeltaTime;

            _currentTime = Mathf.Clamp01(_currentTime);

            _curveVar = Mathf.Clamp01(steamScaleCurve.Evaluate(_currentTime));

            steamScaleObject.localScale = new Vector3(
                steamScaleObject.localScale.x,
                _curveVar,
                steamScaleObject.localScale.z
            );
        }

        protected override void OnGlobalCycle()
        {
            _isActive = !_isActive;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    float finalForce = baseForce * _curveVar;
                    Debug.Log($"Force !!! CurveVar: {_curveVar:F2}, FinalForce: {finalForce:F1}");
                    playerRb.AddForce(new Vector2(0, finalForce));
                }
            }
        }
    }
}
