using UnityEngine;

namespace TestingMod.TestingMod.Core
{
    internal class TagComponent : MonoBehaviour
    {
        public string stringTag {  get; set; }
        public float timeCreated {  get; private set; }

        void Start()
        {
            timeCreated = Time.timeSinceLevelLoad;
        }
    }
}
