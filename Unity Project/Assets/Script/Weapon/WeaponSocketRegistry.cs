using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public class WeaponSocketRegistry : MonoBehaviour
    {
        [SerializeField] private List<WeaponSocketData> _list_WeaponSockets = new List<WeaponSocketData>();

        private readonly Dictionary<string, Transform> _dc_Sockets = new Dictionary<string, Transform>();

        private void Awake()
        {
            CacheSockets();
        }

        public bool TryGetSocket(string socketID, out Transform socketTransform)
        {
            socketTransform = null;

            if (string.IsNullOrWhiteSpace(socketID))
                return false;

            if (_dc_Sockets.Count == 0)
                CacheSockets();

            if (_dc_Sockets.TryGetValue(socketID, out socketTransform) && socketTransform != null)
                return true;

            socketTransform = FindChildRecursive(transform, socketID);

            if (socketTransform == null)
                return false;

            _dc_Sockets[socketID] = socketTransform;
            return true;
        }

        private void CacheSockets()
        {
            _dc_Sockets.Clear();

            for (int i = 0; i < _list_WeaponSockets.Count; i++)
            {
                WeaponSocketData socketData = _list_WeaponSockets[i];

                if (socketData == null || string.IsNullOrWhiteSpace(socketData.SocketID) || socketData.SocketTransform == null)
                    continue;

                _dc_Sockets[socketData.SocketID] = socketData.SocketTransform;
            }
        }

        private Transform FindChildRecursive(Transform parent, string childName)
        {
            if (parent == null)
                return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                if (child.name == childName)
                    return child;

                Transform foundChild = FindChildRecursive(child, childName);

                if (foundChild != null)
                    return foundChild;
            }

            return null;
        }
    }

    [Serializable]
    public class WeaponSocketData
    {
        public string SocketID;
        public Transform SocketTransform;
    }
}
