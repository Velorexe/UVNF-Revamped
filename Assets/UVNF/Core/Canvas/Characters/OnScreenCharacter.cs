using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVNF.Core.Entities.Characters;
using UnityEngine.UI;

namespace UVNF.Core.Canvas
{
    public class OnScreenCharacter : MonoBehaviour
    {
        public string Name
        {
            get { return _name; }
        }
        [SerializeField]
        private string _name;

        public Character OriginalCharacter;

#if UNITY_EDITOR
        public void Compile(Character character)
        {
            OriginalCharacter = character;
            _name = character.CharacterName;

            name = _name;

            foreach(CharacterPose pose in OriginalCharacter.Poses)
            {
                GameObject childPose = new GameObject(pose.PoseName, typeof(RectTransform));
                childPose.transform.SetParent(this.transform);

                Image poseImage = childPose.AddComponent<Image>();
                poseImage.sprite = pose.PoseSprite;

                RectTransform transform = childPose.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(pose.PoseSprite.texture.width, pose.PoseSprite.texture.height);

                foreach(CharacterPart part in pose.CharacterParts)
                {
                    GameObject childPart = new GameObject(part.PartName);
                    childPart.transform.parent = childPose.transform;

                    for(int i = 0; i < part.PoseSprites.Count; i++)
                    {
                        GameObject childPartSprite = new GameObject(part.PoseNames[i], typeof(RectTransform));
                        childPartSprite.transform.SetParent(childPart.transform);

                        Image partImage = childPartSprite.AddComponent<Image>();
                        partImage.sprite = part.PoseSprites[i];

                        RectTransform partTransform = childPartSprite.GetComponent<RectTransform>();
                        partTransform.sizeDelta = new Vector2(part.SpriteRect.width, part.SpriteRect.height);

                        partTransform.localPosition = part.SpriteRect.position;
                    }
                }
            }
        }
#endif
    }
}