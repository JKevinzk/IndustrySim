using System.Collections.Generic;
using UnityEngine;

namespace Components.Project.Emitter
{
    public class CreateObject : MonoBehaviour
    {
        [SerializeReference]public List<PlacedObjectTypeSO> allAutoCreateObj;
    }
}
