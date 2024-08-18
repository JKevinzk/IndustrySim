using System.Collections.Generic;
using Components.Items;
using UnityEngine;

namespace Components.Stations.Palletizer
{
    /// <summary>
    /// 传送带运动逻辑
    /// 保护成员
    /// - loadSet负载对象池
    /// 保护方法
    /// - MoveObj(GameObject)
    /// </summary>
    public class LoadBelt : MonoBehaviour
    {
        /// <summary>
        /// 承载项目对象池，存储承载的所有项目对象
        /// </summary>
        protected HashSet<ItemComponent> loadSet;

        private Rigidbody _rigidbody;
        [SerializeField] private float moveSpeed = 0.6f;


        private void Awake()
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        public virtual void Start()
        {
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _rigidbody.isKinematic = true;
            loadSet = new HashSet<ItemComponent>();
        }

        /// <summary>
        /// 在项目组件与负载组件发生碰撞时，加入到负载对象池
        /// </summary>
        /// <param name="collisionInfo"></param>
        private void OnCollisionStay(Collision collisionInfo)
        {
            GameObject loadObject = collisionInfo.gameObject;
            if (ComponentManager.Instance.compoDict.ContainsKey(loadObject.name) && 
                ComponentManager.Instance.compoDict[loadObject.name].componentType ==
                ComponentManager.ComponentClasses.ItemComponent &&
                loadObject.GetComponent<ItemComponent>().bearObject == null
               )
            {
                loadSet.Add(loadObject.GetComponent<ItemComponent>());
                loadObject.GetComponent<ItemComponent>().bearObject = transform.gameObject;
                Debug.Log("有物体进入码垛机传送带");
            }
        }

        private void OnCollisionExit(Collision other)
        {
            ItemComponent loadComponent = other.gameObject.GetComponent<ItemComponent>();
            if (loadComponent == null || !loadSet.Contains(loadComponent)) return;
            if (loadComponent.bearObject == gameObject)
            {
                loadComponent.bearObject = null;
            }

            loadSet.Remove(loadComponent);
            //Debug.Log("有物体离开码垛机传送带");
        }

        /// <summary>
        /// 使物体运动
        /// </summary>
        /// <param name="bearObject"></param>
        protected void MoveObj(GameObject bearObject)
        {
            // Vector3 dir = Vector3.forward;

            // foreach (var loadObject in loadSet)
            // {
            //     if (loadObject.bearObject == bearObject)
            //     {
            //         // loadObject.transform.position += dir * moveSpeed * Time.deltaTime;
            //         // loadObject.GetComponent<Rigidbody>().velocity = dir;
            //         // loadObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * moveSpeed);
            //         var pos = loadObject.GetComponent<Rigidbody>().position;
            //         loadObject.GetComponent<Rigidbody>().MovePosition(pos+Vector3.forward*moveSpeed*Time.deltaTime);
            //     }
            // }
            var pos = _rigidbody.position;
            _rigidbody.position += Vector3.back * moveSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(pos);
        }
    }
}