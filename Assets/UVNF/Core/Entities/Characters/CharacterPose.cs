using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVNF.Core.Entities.Characters
{
    public class CharacterPose : ScriptableObject
    {
        public string PoseName
        {
            get { return _poseName; }
            private set
            {
                if (!string.IsNullOrEmpty(value))
                    _poseName = value;
            }
        }
        [SerializeField]
        private string _poseName = "New Pose";

        public Sprite PoseSprite
        {
            get => _poseSprite;
            private set => _poseSprite = value;
        }
        [SerializeField]
        private Sprite _poseSprite;

        [SerializeField]
        private List<CharacterPart> _characterParts = new List<CharacterPart>();

#if UNITY_EDITOR
        public CharacterPart[] CharacterParts
        {
            get { return _characterParts.ToArray(); }
        }

        public void SetPoseSprite(Sprite newSprite)
        {
            PoseSprite = newSprite;
        }

        public void SetPoseName(string newPoseName)
        {
            PoseName = newPoseName;
        }

        public void AddCharacterPart(CharacterPart part)
        {
            _characterParts.Add(part);
        }

        public void RemoveCharacterPart(int partIndex)
        {
            _characterParts.RemoveAt(partIndex);
        }
#endif
    }
}