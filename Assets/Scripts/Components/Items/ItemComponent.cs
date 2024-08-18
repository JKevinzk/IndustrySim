using UnityEngine;

namespace Components.Items
{
    public class ItemComponent : BaseComponent
    {
        public string Name { get; set; }
        public GameObject bearObject;
        private Rigidbody _rigidbody;

        protected override void Start()
        {
            base.Start();
            _rigidbody = GetComponent<Rigidbody>();
        }
    
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (bearObject != null)
            {
                bearObject.GetComponent<BearComponent>().DeleteLoadCmp(this);
            }
        }

        public override void OnFreeze()
        {
            base.OnFreeze();
        }

        public override void UnFreeze()
        {
            base.UnFreeze();
        
        
        }

        public override void OnStart()
        {
            base.OnStart();
            if (RunScene.Instance.isRun)
            {
                _rigidbody.constraints = RigidbodyConstraints.None;
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
            }
            else
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
            }
        }

        public override void OnPause()
        {
            base.OnPause();
        }


    
    }
}