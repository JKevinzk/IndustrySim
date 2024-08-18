using System;
using System.IO;
using UnityEngine;

namespace GameLogic.SaveAndLoad
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager _instance;

        public static SaveManager Instance
        {
            get { return _instance; }
        }

        private int _savedSceneCount = 0;
        private readonly string _savePath = Application.streamingAssetsPath;

        private void Awake()
        {
            _instance = this;
        }

        private Save CreatSaveGo(string sceneName, string sceneInfo)
        {
            Save save = new Save();

            ComponentManager.Instance.UpdateObjInfo();
            //save.nameString = ComponentManager.Instance.createdObjName;
            if (ComponentManager.Instance.createdObjPosXList.Count != 0)
            {
                save.sceneId = _savedSceneCount; //保存场景ID
                save.sceneName = sceneName; // 保存场景名称
                save.sceneInfo = sceneInfo; // 保存场景描述信息
                var pic = GameObject.Find("Main Camera").GetComponent<ScreenShot>().PicByte;
                string pictureString = Convert.ToBase64String(pic, 0, pic.Length);
                save.sceneShot = pictureString;
                // 存储位置
                foreach (var item in ComponentManager.Instance.createdObjPosXList)
                {
                    save.usingPlacedObjPositionX.Add(item);
                }

                foreach (var item in ComponentManager.Instance.createdObjPosYList)
                {
                    save.usingPlacedObjPositionY.Add(item);
                }

                foreach (var item in ComponentManager.Instance.createdObjPosZList)
                {
                    save.usingPlacedObjPositionZ.Add(item);
                }

                foreach (var item in ComponentManager.Instance.compoInSceneDic)
                {
                    save.usingGameObjString.Add(item.Value.name);
                }

                // 存储旋转
                foreach (var item in ComponentManager.Instance.createdObjRotXList)
                {
                    save.usingPlacedOjbRotationX.Add(item);
                }

                foreach (var item in ComponentManager.Instance.createdObjRotYList)
                {
                    save.usingPlacedOjbRotationY.Add(item);
                }

                foreach (var item in ComponentManager.Instance.createdObjRotZList)
                {
                    save.usingPlacedOjbRotationZ.Add(item);
                }
            }

            return save;
        }

        //通过读取创建对象
        private GameObject CreatLoadObj(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab);
            //改变层级便于后续拖动
            obj.layer = LayerMask.NameToLayer("Drag");
            obj.AddComponent<ObjectInfo>();
            return obj;
        }

        //通过读档重置场景状态
        private void SetGame(Save save)
        {
            ComponentManager.Instance.DestroyAllCreatedObj();
            Debug.Log("compoInSceneList.Count=" + ComponentManager.Instance.compoInSceneDic.Count);


            //暴力解决多一个1的问题...
            if (ComponentManager.Instance.compoInSceneDic.Count == 1)
            {
                Destroy(ComponentManager.Instance.compoInSceneDic[0]);
                ComponentManager.Instance.DelObjInfoByIndex(0);
            }

            for (int i = 0; i < save.usingGameObjString.Count; i++)
            {
                Vector3 pos = new Vector3();
                Vector3 rot = new Vector3();
                pos.x = (float)save.usingPlacedObjPositionX[i];
                pos.y = (float)save.usingPlacedObjPositionY[i];
                pos.z = (float)save.usingPlacedObjPositionZ[i];
                rot.x = (float)save.usingPlacedOjbRotationX[i];
                rot.y = (float)save.usingPlacedOjbRotationY[i];
                rot.z = (float)save.usingPlacedOjbRotationZ[i];

                GameObject obj = ComponentManager.Instance.CreatComponentByName(save.usingGameObjString[i]);
                obj.transform.position = pos;
                obj.transform.rotation = Quaternion.Euler(rot);
                ComponentManager.Instance.SaveObjInfo(obj);

                _savedSceneCount = save.sceneId;
            }
        }

        private void SaveByJson(string sceneName, string sceneInfo)
        {
            string filePath = Path.Combine(_savePath, sceneName + ".json");
            try
            {
                Save save = CreatSaveGo(sceneName, sceneInfo);
                _savedSceneCount++;
                
                //Debug.Log("SceneId" + savedSceneCount);
                //将对象转换为json格式的字符串
                string saveJsonStr = JsonUtility.ToJson(save);

                //将字符串写入文件
                StreamWriter sw = new StreamWriter(filePath);
                sw.WriteLine(saveJsonStr);
                //关闭StreamWriter,并将字符串写入
                sw.Close();
                Debug.Log("json文件保存成功,文件名为" + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save file from {filePath}. \n{e}");
            }
        }

        private void LoadByJson(string sceneName)
        {
            string filePath = Path.Combine(_savePath, sceneName + ".json");
            try
            {
                Debug.Log(filePath);
                if (File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath);

                    string jsonStr = sr.ReadToEnd();

                    sr.Close();

                    Save save = JsonUtility.FromJson<Save>(jsonStr);
                    SetGame(save);
                    Debug.Log($"读取存档成功，路径为{filePath}");
                }
                else
                {
                    Debug.Log("存档文件不存在");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load file from {filePath}. \n{e}");
            }
        }

        public void SaveScene(string sceneName, string sceneInfo)
        {
            SaveByJson(sceneName, sceneInfo);
        }

        public void LoadScene(string SceneName)
        {
            LoadByJson(SceneName);
        }
    }
}