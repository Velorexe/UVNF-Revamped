using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace UVNF.Core.Entities.Characters
{
    public class Character : ScriptableObject
    {
        public string CharacterName
        {
            get { return _characterName; }
            private set
            {
                if (!string.IsNullOrEmpty(value))
                    _characterName = value;
            }
        }
        [SerializeField]
        private string _characterName = "NewCharacter";

        [SerializeField]
        private List<CharacterPose> _poses = new List<CharacterPose>();

        public GameObject PrecompiledCharacter = null;

        //Expose the Get and Set only to the Unity Editor
#if UNITY_EDITOR
        public CharacterPose[] Poses
        {
            get { return _poses.ToArray(); }
        }

        public void AddPose(CharacterPose pose)
        {
            _poses.Add(pose);
        }

        public void RemovePoseAt(int index)
        {
            _poses.RemoveAt(index);
            _poses.RemoveAll(x => x == null);
        }

        public void SetCharacterName(string newCharacterName)
        {
            CharacterName = newCharacterName;
        }
#endif

        public Sprite GetPose(string poseName)
        {
            //if (Poses.ContainsKey(poseName))
            //    return Poses[poseName];
            ////TODO: Convert to async
            //return Poses.First(x => x.Value != null).Value;
            return null;
        }
    }
}