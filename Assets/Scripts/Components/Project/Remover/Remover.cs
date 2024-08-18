using System;
using System.Collections;
using System.Linq;
using Components.Items;
using UnityEditor;
using UnityEngine;

namespace Components.Project.Remover
{
    public class Remover : MonoBehaviour
    {
        private readonly ComponentManager _componentManager = ComponentManager.Instance;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Drag") &&
                ComponentManager.Instance.compoDict[other.gameObject.name].componentType ==
                ComponentManager.ComponentClasses.ItemComponent)

                StartCoroutine(DestroyItem(other.gameObject));
        }

        private IEnumerator DestroyItem(GameObject item)
        {
            yield return new WaitForSeconds(1);
            if (item == null) yield break;
            int id = item.GetComponentInChildren<ObjectInfo>().ObjId;
            ItemComponent itemComponent = item.GetComponentInChildren<ItemComponent>();
            
            // 清除与传送带的连接
            if (itemComponent.bearObject != null)
            {
                itemComponent.bearObject.GetComponentInChildren<BearComponent>().DeleteLoadCmp(itemComponent);
            }
            // 清除场景中的信息
            if (_componentManager.compoInSceneDic.ContainsKey(id))
            {
                for (var i = 0; i < _componentManager.compoInSceneDic.Count; i++)
                {
                    var obj = _componentManager.compoInSceneDic.ElementAt(i).Value;
                    if (obj.GetComponentInChildren<ObjectInfo>().ObjId == id)
                    { 
                        _componentManager.DelObjInfoByIndex(i);
                    }
                }
                _componentManager.compoInSceneDic.Remove(id);
            }
            
            Destroy(item);
        }
    }
}