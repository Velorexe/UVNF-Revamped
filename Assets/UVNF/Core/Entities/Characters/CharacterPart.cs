using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVNF.Core.Entities.Characters
{
    public class CharacterPart : ScriptableObject
    {
        public string PartName
        {
            get { return _partName; }
            private set
            {
                if (!string.IsNullOrEmpty(value))
                    _partName = value;
            }
        }
        [SerializeField]
        private string _partName = "Default";

        //Default pose is always at index 0
        [SerializeField]
        private List<string> _poseNames = new List<string>();
        [SerializeField]
        private List<Sprite> _poseSprites = new List<Sprite>();

        public Rect SpriteRect
        {
            get { return _spriteRect; }
            private set { _spriteRect = value; }
        }
        [SerializeField]
        private Rect _spriteRect = new Rect(0f, 0f, 100f, 100f);

        //Expose the Get and Set only to the Unity Editor
#if UNITY_EDITOR
        public bool IsVisible = true;

        public int SpriteToShow = 0;

        public void SetSpriteRect(Rect newRect)
        {
            SpriteRect = newRect;
        }

        public List<string> PoseNames
        {
            get { return _poseNames; }
            set { _poseNames = value; }
        }

        public List<Sprite> PoseSprites
        {
            get { return _poseSprites; }
            set { _poseSprites = value; }
        }

        public void SetPartName(string newPartName)
        {
            PartName = newPartName;
        }

        public void AddPose()
        {
            _poseNames.Add("New Pose");
            _poseSprites.Add(null);
        }
#endif
    }
}