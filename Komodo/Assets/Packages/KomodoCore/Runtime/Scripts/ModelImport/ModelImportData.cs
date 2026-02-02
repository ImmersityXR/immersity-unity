using UnityEngine;

namespace Komodo.AssetImport
{
    [System.Serializable]
    public struct ModelImportData

    {
        public string name;
        public int id;
        public string url;

        public float scale;
        public Vector3 position;
        public Vector3 euler_rotation;

        public bool isWholeObject;
    }
}