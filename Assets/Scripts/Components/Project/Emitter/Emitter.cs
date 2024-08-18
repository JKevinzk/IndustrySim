using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Project.Emitter
{
    public class Emitter : MonoBehaviour

    {
        public List<string> baseObjects; // 生成的托盘
        public List<string> partObjects; // 生成的货物
        public int itemNumber; // 目前有多少item在生产范围内
        private GameObject _createObj;
        private float _timer; // 产生定时器
        private const float Interval = 1f; // 生产间隔时间

        private readonly ComponentManager _componentManager = ComponentManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;

        public void Start()
        {
            _timer = 0f;
            itemNumber = 0;
            
            baseObjects = new List<string>();
            partObjects = new List<string>();

            Random.InitState((int)System.DateTime.Now.Ticks);
            GetComponent<BaseComponent>().isEmit = true;
        }

        protected void Update()
        {
            if (_runScene.isRun)
            {
                if (itemNumber!=0) return;
                _timer += Time.deltaTime;
                if (_timer >= Interval) // 在生成范围内无物体的情况下，1s生产一个箱子
                {
                    if (!CreateItem())
                        StartCoroutine(DeleteNoneObject());
                    itemNumber++;
                    _timer = 0f;
                }
            }
            else
            {
                _timer = 0f;
                itemNumber = 0;
            }
        }

        private bool CreateItem()
        {
            var t = Random.Range(0, baseObjects.Count);
            if (baseObjects.Count != 0)
                if (baseObjects[t] != "None")
                {
                    _createObj = _componentManager.CreatComponentByTypeSO(_componentManager.compoDict[baseObjects[t]]);
                    _createObj.transform.position = transform.position;
                    _createObj.transform.parent = GameObject.Find("TemporaryObject").transform;
                    Debug.Log("生产一个平台");
                    return true;
                }
                else
                {
                    Debug.Log("生成一个空平台");
                    return false;
                }

            t = Random.Range(0, partObjects.Count);
            if (partObjects.Count != 0)
                if (partObjects[t] != "None")
                {
                    _createObj = _componentManager.CreatComponentByTypeSO(_componentManager.compoDict[partObjects[t]]);
                    _createObj.transform.position = transform.position;
                    _createObj.transform.parent = GameObject.Find("TemporaryObject").transform;
                    Debug.Log("生产一个货物");
                    return true;
                }
                else
                {
                    Debug.Log("生成一个空货物");
                    return false;
                }

            return false;
        }

        /// <summary>
        /// 减去生产空物体在探测范围内的残留痕迹
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeleteNoneObject()
        {
            yield return new WaitForSeconds(Interval);
            itemNumber--;
        }
    }
}