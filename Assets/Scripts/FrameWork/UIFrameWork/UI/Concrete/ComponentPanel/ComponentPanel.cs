using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class ComponentPanel : BasePanel
    {
        //组件面板
        private const string Path = "Prefabs/UI/Panel/ComponentCanvas";
        private readonly ComponentManager _componentManager = ComponentManager.Instance;

        private Dictionary<string, Sprite> _dictionary;

        //public UIType uiType = new UIType(path);
        //开始主面板
        public ComponentPanel() : base(new UIType(Path))
        {
        }

        public override void onEnter()
        {
            // 组件图标加载
            String img = "Img";
            //获取resource的资源
            Sprite[] sprite = Resources.LoadAll<Sprite>(img);
            // Debug.Log(sprite+"     "+img);
            _dictionary = DisposeAtlas(sprite);
            LoadComponentIcon(ComponentManager.ComponentClasses.All);


            // 下拉菜单，图标分类刷新
            Dropdown dropdown = UITool.GetComponentInChildren<Dropdown>("OptionDropDown");
            dropdown.onValueChanged.AddListener(index =>
            {
                Debug.Log(Enum.Parse(typeof(ComponentManager.ComponentClasses), index.ToString()));
                var components = UITool.FindComponentsInChild<SelectImage>(false);
                foreach (var component in components)
                {
                    Object.Destroy(component.gameObject);
                }

                LoadComponentIcon((ComponentManager.ComponentClasses)index);
            });
        }

        //整理资源将名字以及对应精灵图以字典形式返回
        private static Dictionary<string, Sprite> DisposeAtlas(Sprite[] headAtlas)
        {
            Dictionary<string, Sprite> spriteRes = new Dictionary<string, Sprite>();
            for (var i = 0; i < headAtlas.Length; i++)
            {
                spriteRes.Add(headAtlas[i].name, headAtlas[i]);
            }

            return spriteRes;
        }

        private void LoadComponentIcon(ComponentManager.ComponentClasses componentClass)
        {
            foreach (var component in _componentManager.compoDict.Values)
            {
                if (!_dictionary.ContainsKey(component.name)) continue;
                if (componentClass != ComponentManager.ComponentClasses.All &&
                    component.componentType != componentClass)
                {
                    //Debug.Log("找不到" + component.name);
                    continue;
                }

                GameObject componentPrefab = UITool.FindChildGameObject("Component"); //在预制体中获取子对象位置
                GameObject componentBtn = Object.Instantiate(componentPrefab, componentPrefab.transform.parent);
                componentBtn.name = component.name;

                componentBtn.transform.GetChild(0).GetComponent<Image>().sprite =
                    _dictionary[component.name]; //根据整理好的字典，获取精灵图
                componentBtn.transform.GetChild(0).GetComponent<Image>().SetNativeSize(); //设置原生大小
                componentBtn.SetActive(true);
            }

            // 滑动条隐藏
            Scrollbar scrollbar = UITool.GetComponentInChildren<Scrollbar>("Scrollbar Vertical");
            if (scrollbar != null && Math.Abs(scrollbar.size - 1) > 0.001f)
            {
                scrollbar.interactable = false;
            }
            else
                scrollbar.interactable = true;
        }
    }
}