using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    [Serializable]
    public class FxObject
    {
        [FormerlySerializedAs("Object")]
        public GameObject FX;
        public bool IsPrefab = true;
        public bool IsParticleSystem = false;
        public Vector3 Offset = Vector3.zero;
        public Vector3 RotationOffset = Vector3.zero;
        public bool AttachToParent = false;
        public float Delay = 0f;
        public bool SingleInstance = true;
        public float Scale = 1f;

        private GameObject _instance;

        public void Trigger(Transform parent)
        {
            if(FX == null)
                return;

            _instance = GetOrCreateObject(parent);
            Stop();

            if (IsParticleSystem)
            {
                var ps = _instance.GetComponent<ParticleSystem>();
                ps.Play();
            }
        }

        public void Stop()
        {
            if (_instance != null)
            {
                if (IsParticleSystem)
                    _instance.GetComponent<ParticleSystem>().Stop();
                else
                    UnityEngine.Object.Destroy(_instance);
            }
        }

        GameObject GetOrCreateObject(Transform parent)
        {
            if (SingleInstance && _instance != null)
            {
                return _instance;
            }

            if (!IsPrefab)
            {
                _instance = FX;
                return _instance;
            }
                
            var obj = UnityEngine.Object.Instantiate(
                this.FX, 
                parent.position + parent.rotation * Offset, 
                parent.rotation * Quaternion.Euler(RotationOffset));
            obj.transform.localScale *= Scale;
            if(AttachToParent)
                obj.transform.SetParent(parent, worldPositionStays: true);
            return obj;
        }
    }
}