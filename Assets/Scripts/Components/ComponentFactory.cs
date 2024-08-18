using System;
using System.Diagnostics;
using Components.Items;
using Components.Project;
using Components.Project.Emitter;
using Components.Project.Remover;
using Components.Sensors;
using Components.Stations.Palletizer;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Components
{
    public static class ComponentUtil
    {
        //创建对象
        public static GameObject CreatComponentByName(string name)
        {
            if (ComponentManager.Instance.compoDict.ContainsKey(name))
            {
                GameObject gameObject = ComponentManager.Instance.compoDict[name].prefab;
                return gameObject;
            }
            else
            {
                return null;
            }
        }

        //添加公用组件到对象
        public static GameObject AddCommonCompo(GameObject gameObject, string name)
        {
            gameObject.layer = LayerMask.NameToLayer("Drag");
            if (gameObject.GetComponent<ObjectInfo>() == null)
            {
                gameObject.AddComponent<ObjectInfo>().ObjId = ComponentManager.Instance.ObjId;
            }

            if (gameObject.GetComponent<DrawLine>() == null)
            {
                gameObject.AddComponent<DrawLine>();
            }

            if (gameObject.GetComponent<ObjectBound>() == null)
            {
                gameObject.AddComponent<ObjectBound>();
            }

            if (gameObject.GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
            }

            return gameObject;
        }

        // 设置组件大小
        public static void SetComponentSize(GameObject component)
        {
            string name = component.name;
            if (ComponentManager.Instance.compoDict[name].componentType ==
                ComponentManager.ComponentClasses.ProjectComponent)
                return;
            float width = ComponentManager.Instance.compoDict[name].width;
            float length = ComponentManager.Instance.compoDict[name].length;

            component.transform.localScale = Vector3.one;

            if (width == 0 || length == 0)
            {
                Debug.LogWarning("当前组件(" + name + ")未设置长宽");
                return;
            }

            Vector3 scale = Vector3.one;
            var bounds = ObjectBound.GetBounds(component);
            double TOLERANCE = 1e-8f;
            if (Math.Abs(width - length) < TOLERANCE && Math.Abs(length - 0.125f) < TOLERANCE)
            {
                scale *= 0.125f / math.max(bounds.size.x, bounds.size.z);
            }
            else
            {
                scale.x *= width / bounds.size.x;
                scale.z *= length / bounds.size.z;
                scale.y = scale.z;
            }

            // foreach (Transform trChild in component.GetComponentInChildren<Transform>())
            // {
            //     if (trChild.GetComponent<Renderer>())
            //         trChild.localScale = scale;
            // }
            // if (component.GetComponent<Renderer>())
            component.transform.localScale = scale;
        }
    }


    public static class ComponentFactory
    {
        public static IComponentModel GetModelTypeByName(string name)
        {
            string[] componentType =
            {
                "ItemComponent", "BearComponent",
                "SensorComponent", "StationComponent", "WarningComponent", "WalkWayComponent", "ProjectComponent"
            };

            string gameObjectType;
            if (ComponentManager.Instance.compoDict.ContainsKey(name))
            {
                gameObjectType = ComponentManager.Instance.compoDict[name].componentType.ToString();
                //Debug.Log($"已在compoDict中找到{name}对象,类型为{gameObjectType}");
            }
            else
            {
                Debug.LogWarning($"compoDict中找不到{name}子对象");
                return null;
            }

            if (gameObjectType.Equals(componentType[0]))
            {
                return new ItemComponentModel();
            }

            if (gameObjectType.Equals(componentType[1]))
            {
                return new BearComponentModel();
            }

            if (gameObjectType.Equals(componentType[2]))
            {
                return new SensorComponentModel();
            }

            if (gameObjectType.Equals(componentType[3]))
            {
                return new StationComponentModel();
            }

            if (gameObjectType.Equals(componentType[4]))
            {
                return new WarningComponentModel();
            }

            if (gameObjectType.Equals(componentType[5]))
            {
                return new WalkwayComponentModel();
            }

            if (gameObjectType.Equals(componentType[6]))
            {
                return new ProjectComponentModel();
            }

            return null;
        }
    }

    

    public interface IComponentModel
    {
        GameObject Creat(string name);
        void AddComponent(ref GameObject obj, string name);
    }

    public class BearComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            GameObject obj = ComponentUtil.CreatComponentByName(name);

            return obj;
        }

        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            if (obj.GetComponent<BearComponent>() == null)
                obj.AddComponent<BearComponent>();
            //obj.GetComponent<BearComponent>().OnEnter();
            if (name.Equals("BeltConveyor4M"))
            {
                //添加BeltConveyor对应组件
            }
        }
    }

    public class ItemComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            var obj = ComponentUtil.CreatComponentByName(name);

            return obj;
        }

        //对同类型下的不同功能部件添加特定功能组件
        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            if (obj.GetComponent<ItemComponent>() == null)
                obj.AddComponent<ItemComponent>();

            switch (name)
            {
                case "PalletizingBox":
                    obj.AddComponent<PalletizingBox>();
                    break;
                case "SquarePallet":
                    obj.AddComponent<SquarePallet>();
                    break;
            }
        }
    }

    public class WalkwayComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            var obj = ComponentUtil.CreatComponentByName(name);

            return obj;
        }

        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            if (obj.GetComponent<WalkwayComponent>() == null)
                obj.AddComponent<WalkwayComponent>();
        }
    }
    public class WarningComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            var obj = ComponentUtil.CreatComponentByName(name);

            return obj;
        }

        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            obj.AddComponent<BaseComponent>();
        }
    }

    public class StationComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            var obj = ComponentUtil.CreatComponentByName(name);

            return obj;
        }

        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            switch (name)
            {
                case "Palletizer":
                    obj.AddComponent<Palletizer>();
                    break;
                case "ArmRob":
                    obj.AddComponent<ArmRob>();
                    break;
                default:
                    obj.AddComponent<StationComponent>();
                    break;
            }
        }
    }

    public class SensorComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            var obj = ComponentUtil.CreatComponentByName(name);

            return obj;
        }

        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            switch (name)
            {
                case "RetroreflectiveSensor":
                    obj.AddComponent<Sensor>();
                    break;
                case "Reflector":
                    obj.AddComponent<Reflector>();
                    break;
                default:
                    obj.AddComponent<SenserComponent>();
                    break;
            }
        }
    }

    public class ProjectComponentModel : IComponentModel
    {
        public GameObject Creat(string name)
        {
            var obj = ComponentUtil.CreatComponentByName(name);
            return obj;
        }

        public void AddComponent(ref GameObject obj, string name)
        {
            obj = ComponentUtil.AddCommonCompo(obj, name);
            if (obj.GetComponent<ProjectComponent>() == null)
            {
                obj.AddComponent<ProjectComponent>();
            }

            switch (name)
            {
                case "Emitter":
                    obj.AddComponent<Emitter>();
                    break;
                case "Remover":
                    obj.AddComponent<Remover>();
                    break;
            }
        }
    }
}