using UnityEngine;

namespace Components
{
    [CreateAssetMenu()]
    public class PlacedObjectTypeSO : ScriptableObject
    {
        public string nameString;
        public GameObject prefab;
        //public Transform visual;
        public float width;
        public float length;
        //public string conponentType;

        public ComponentManager.ComponentClasses componentType;

    }
}
