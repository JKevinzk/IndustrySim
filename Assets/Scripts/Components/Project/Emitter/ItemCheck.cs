using System;
using UnityEngine;

namespace Components.Project.Emitter
{
    public class ItemCheck : MonoBehaviour
    {
        private Emitter _emitter;

        private void Start()
        {
            _emitter = transform.parent.GetComponent<Emitter>();
        }

        // 检测发射器产生的item是否离开范围
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<BaseComponent>() != null &&
                other.gameObject.layer == LayerMask.NameToLayer("Drag") &&
                ComponentManager.Instance.compoDict[other.gameObject.name].componentType ==
                ComponentManager.ComponentClasses.ItemComponent)
            {
                _emitter.itemNumber--;
                //Debug.Log("isExit");
            }
        }
        
        // 检测发射器是否有item进入
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.GetComponent<BaseComponent>() != null &&
        //         other.gameObject.layer == LayerMask.NameToLayer("Drag") &&
        //         ComponentManager.Instance.compoDict[other.gameObject.name].componentType ==
        //         PlacedObjectTypeSO.ComponentType.ItemComponent)
        //     {
        //         _emitter.itemNumber++;
        //         Debug.Log("isEnter");
        //     }
        // }
    }
}