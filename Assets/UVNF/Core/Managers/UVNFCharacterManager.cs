using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UVNF.Core.Entities.Characters;
using UVNF.Core.Entities.ScriptLines;
using System.Linq;

namespace UVNF.Core.Canvas
{
    public class UVNFCharacterManager : UVNFManager
    {
        [SerializeField]
        private RectTransform _characterCanvas;

        private Vector2 _canvasSize;

        private List<OnScreenCharacter> _activeCharacters = new List<OnScreenCharacter>();
        private List<GameObject> _cachedCharacters;

        private UVNFResources _resources;

        public override async UniTask Init(UVNFGameManager gameManager, CancellationToken token)
        {
            _characterCanvas = this.GetComponent<RectTransform>();
            _canvasSize = _characterCanvas.rect.size;

            _cachedCharacters = await gameManager.Resources.GetResourcesWithLabel<GameObject>("UVNF-Character");
        }

        /// <summary>
        /// Adds a character to the screen on the specified percentage.
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="screenPositionPercentage"></param>
        /// <returns></returns>
        public async UniTask AddCharacter(string characterName, string pose, Vector2 position, Vector2 scale)
        {
            GameObject characterObject = _cachedCharacters.FirstOrDefault(x => x.GetComponent<OnScreenCharacter>().Name == characterName);

            if (characterObject != null)
            {
                OnScreenCharacter character = characterObject.GetComponent<OnScreenCharacter>();

                if (character.Poses.Count == 0)
                    Debug.LogWarning("Character does not contain any poses.");

                RectTransform transform = Instantiate(character.gameObject, _characterCanvas).GetComponent<RectTransform>();
                transform.localPosition = new Vector2(position.x * _canvasSize.x / 2f, position.y * _canvasSize.y);

                Vector3 cS = transform.localScale;
                transform.localScale = new Vector3(scale.x * cS.x, scale.y * cS.y, cS.z);

                character.SwitchToPose(pose);
            }
        }
    }
}