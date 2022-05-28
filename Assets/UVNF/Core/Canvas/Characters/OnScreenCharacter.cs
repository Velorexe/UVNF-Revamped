using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVNF.Core.Entities.Characters;
using UnityEngine.UI;
using UVNF.Utilities;

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

        public Dictionary<string, RectTransform> Poses
        {
            get { return _poses; }
        }
        [SerializeField]
        private SerializableDictionary<string, RectTransform> _poses = new SerializableDictionary<string, RectTransform>();

        [SerializeField]
        private string _defaultPose = string.Empty;
        [SerializeField, HideInInspector]
        private string _currentPose = string.Empty;

        public void SwitchToPose(string newPose)
        {
            if (string.IsNullOrEmpty(newPose))
            {
                Debug.LogWarning("Got an empty pose, picking default (first).");

                _poses[_currentPose].gameObject.SetActive(false);
                _poses[_defaultPose].gameObject.SetActive(true);
            }
            else if (!newPose.Equals(_currentPose))
            {
                _poses[_currentPose].gameObject.SetActive(false);
                _currentPose = newPose;

                _poses[_currentPose].gameObject.SetActive(true);
            }
        }

#if UNITY_EDITOR
        public void Compile(Character character)
        {
            _name = character.CharacterName;
            this.name = _name;

            RectTransform currentTransform = GetComponent<RectTransform>();

            currentTransform.anchorMin = new Vector2(0.5f, 0f);
            currentTransform.anchorMax = new Vector2(0.5f, 0f);

            foreach (CharacterPose pose in character.Poses)
            {
                GameObject childPose = new GameObject(pose.PoseName, typeof(RectTransform));
                childPose.transform.SetParent(this.transform);

                Image poseImage = childPose.AddComponent<Image>();
                poseImage.sprite = pose.PoseSprite;

                RectTransform transform = childPose.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(pose.PoseSprite.texture.width, pose.PoseSprite.texture.height);

                transform.anchorMin = new Vector2(0.5f, 0f);
                transform.anchorMax = new Vector2(0.5f, 0f);

                transform.position = new Vector2(0f, pose.PoseSprite.texture.height / 2f);

                _poses.Add(pose.PoseName, transform);

                foreach (CharacterPart part in pose.CharacterParts)
                {
                    GameObject childPart = new GameObject(part.PartName, typeof(RectTransform));
                    childPart.transform.SetParent(childPose.transform);

                    RectTransform childTransform = childPart.GetComponent<RectTransform>();
                    childTransform.position = new Vector2(-pose.PoseSprite.texture.width / 2f, pose.PoseSprite.texture.height / 2f);

                    childTransform.anchorMin = new Vector2(0.5f, 0f);
                    childTransform.anchorMax = new Vector2(0.5f, 0f);

                    childTransform.position = new Vector2(0f, 0f);

                    for (int i = 0; i < part.PoseSprites.Count; i++)
                    {
                        GameObject childPartSprite = new GameObject(part.PoseNames[i], typeof(RectTransform));
                        childPartSprite.transform.SetParent(childPart.transform);

                        Image partImage = childPartSprite.AddComponent<Image>();
                        partImage.sprite = part.PoseSprites[i];

                        RectTransform partTransform = childPartSprite.GetComponent<RectTransform>();
                        partTransform.sizeDelta = new Vector2(part.SpriteRect.width, part.SpriteRect.height);

                        partTransform.anchorMin = new Vector2(0.5f, 0f);
                        partTransform.anchorMax = new Vector2(0.5f, 0f);

                        // Bottom middle is 0,0
                        partTransform.localPosition = new Vector2(part.SpriteRect.x, part.SpriteRect.y);
                    }
                }

                transform.gameObject.SetActive(false);
            }

            if (character.Poses.Length > 0)
            {
                _currentPose = character.Poses[0].PoseName;
                _defaultPose = _currentPose;

                _poses[_currentPose].gameObject.SetActive(true);
            }
        }
#endif
    }
}