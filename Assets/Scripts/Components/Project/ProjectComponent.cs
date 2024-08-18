using UnityEngine;

namespace Components.Project
{
    public class ProjectComponent : BaseComponent
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
